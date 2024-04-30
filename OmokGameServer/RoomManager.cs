using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class RoomManager
    {
        List<Room> _roomList = new List<Room>();
        Queue<int> _roomIndexQueue = new Queue<int>();

        protected Func<string, byte[], bool> _sendFunc;

        int _roomMaxCount = 0;
        int _roomUserMax = 0;

        public void Init(int roomMaxCount, int roomUserMax, Func<string, byte[], bool> sendFunc)
        {
            _roomMaxCount = roomMaxCount;
            _roomUserMax = roomUserMax;
            for (int i = 0; i < _roomMaxCount; i++)
            {
                var room = new Room(i, roomUserMax);
                _roomList.Add(room);
                _roomIndexQueue.Enqueue(i);
            }
            _sendFunc = sendFunc;
        }

        public bool EnterRoom(User user)
        {
            var res = new ResEnterRoomPacket();
            if (_roomIndexQueue.Count == 0)
            {
                res.RoomNumber = -1;
            }
            else
            {
                int roomNumber = _roomIndexQueue.Peek();
                var enterRes = ERROR_CODE.NONE;
                var users = _roomList[roomNumber].GetUserList();
                if (users.Count >= _roomUserMax)
                {
                    _roomIndexQueue.Dequeue();
                    roomNumber = _roomIndexQueue.Peek();
                    enterRes = _roomList[roomNumber].EnterRoom(user);
                    res.RoomNumber = roomNumber;
                }
                else
                {
                    enterRes = _roomList[roomNumber].EnterRoom(user);
                    res.RoomNumber = roomNumber;
                }

                for (int i = 0; i < users.Count; i++)
                {
                    if (user.SessionId != users[i].SessionId)
                    {
                        res.OtherUserId = users[i].UserId;
                    }
                }

                var ntfPacket = new NtfNewUserPacket();
                ntfPacket.Id = user.UserId;
                var ntf = MemoryPackSerializer.Serialize(ntfPacket);
                var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_NEW_USER, ntf);
                BroadCast(roomNumber, user.SessionId, ntfData);
            }

            var data = MemoryPackSerializer.Serialize(res);
            var sendData = ClientPacket.MakeClientPacket(PACKET_ID.RES_ENTER_ROOM, data);
            return _sendFunc(user.SessionId, sendData);
        }

        public bool LeaveRoom(User user, int roomNumber)
        {
            var result = _roomList[roomNumber].LeaveRoom(user.SessionId);
            _roomIndexQueue.Enqueue(roomNumber);

            var res = new ResLeaveRoomPacket();

            if (result != ERROR_CODE.NONE)
            {
                res.Result = false;
                return false;
            }
            res.Result = true;

            var data = MemoryPackSerializer.Serialize(res);
            var sendData = ClientPacket.MakeClientPacket(PACKET_ID.RES_LEAVE_ROOM, data);
            _sendFunc(user.SessionId, sendData);

            var ntfLeave = new NtfLeaveUserPacket();
            ntfLeave.Id = user.UserId;
            var ntf = MemoryPackSerializer.Serialize(ntfLeave);
            var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_LEAVE_USER, ntf);
            BroadCast(roomNumber, user.SessionId, ntfData);

            return true;
        }

        public void BroadCast(int roomNumber, string excludedSessionId, byte[] sendData)
        {
            var tempRoom = _roomList[roomNumber];
            var tempUserList = tempRoom.GetUserList();

            for (int i = 0; i < tempUserList.Count; i++)
            {
                if (excludedSessionId != tempUserList[i].SessionId)
                {
                    _sendFunc(tempUserList[i].SessionId, sendData);
                }
            }
        }

        public void UserStateChange(User user, PACKET_ID packetId, byte[] packet)
        {
            if (packetId == PACKET_ID.REQ_READY)
            {
                var pac = MemoryPackSerializer.Deserialize<ReqReadyPacket>(packet);

                user.State = USER_STATE.READY;

                _roomList[pac.RoomNumber].CheckRoomState();

                if (_roomList[pac.RoomNumber].RoomState == ROOM_STATE.NONE)
                {
                    var resPac = new ResReadyPacket();
                    resPac.Result = true;
                    var res = MemoryPackSerializer.Serialize(resPac);
                    var resData = ClientPacket.MakeClientPacket(PACKET_ID.RES_READY, res);
                    _sendFunc(user.SessionId, resData);

                    var ntfPac = new NtfReadyStatePacket();
                    ntfPac.Id = user.UserId;
                    ntfPac.Result = true;
                    var ntf = MemoryPackSerializer.Serialize(ntfPac);
                    var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_READY_STATE, ntf);

                    BroadCast(pac.RoomNumber, user.SessionId, ntfData);
                }
                else
                {
                    var startPac = new NtfGameStartPacket();

                    var tempUserList = _roomList[pac.RoomNumber].GetUserList();
                    startPac.StartPlayer = tempUserList[0].UserId;
                    tempUserList[0].Stone = STONE.BLACK;
                    tempUserList[0].State = USER_STATE.PLAYING;
                    tempUserList[1].Stone = STONE.WHITE;
                    tempUserList[1].State = USER_STATE.PLAYING;

                    var ntf = MemoryPackSerializer.Serialize(startPac);
                    var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_GAME_START, ntf);

                    BroadCast(pac.RoomNumber, null, ntfData);
                }
            }
            else
            {
                var pac = MemoryPackSerializer.Deserialize<ReqNotReadyPacket>(packet);

                user.State = USER_STATE.NONE;

                var ntfPac = new NtfReadyStatePacket();
                ntfPac.Id = user.UserId;
                ntfPac.Result = false;
                var ntf = MemoryPackSerializer.Serialize(ntfPac);
                var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_READY_STATE, ntf);

                BroadCast(pac.RoomNumber, user.SessionId, ntfData);
            }
        }

        public void PutStone(User user, int posX, int posY)
        {
            /// 흑돌 쌍3 쌍4 6목 처리 및 승리 확인
            var tempRoom = _roomList[user.RoomNumber];

            var checkResult = tempRoom.CheckStoneCount(user.Stone, posX, posY);

            if (checkResult == PUT_RESULT.ERROR)
            {
                var errorPac = new ResPutStonePacket();
                errorPac.Result = false;
                var error = MemoryPackSerializer.Serialize(errorPac);
                var errorData = ClientPacket.MakeClientPacket(PACKET_ID.RES_PUT_STONE, error);
                _sendFunc(user.SessionId, errorData);
                return;
            }
            else if (checkResult == PUT_RESULT.NONE)
            {
                tempRoom.PutStone(user.Stone, posX, posY);

                var resPac = new ResPutStonePacket();
                resPac.Result = true;
                var res = MemoryPackSerializer.Serialize(resPac);
                var resData = ClientPacket.MakeClientPacket(PACKET_ID.RES_PUT_STONE, res);
                _sendFunc(user.SessionId, resData);

                var ntfPac = new NtfPutStonePacket();
                ntfPac.Stone = (int)user.Stone;
                ntfPac.PosX = posX;
                ntfPac.PosY = posY;
                var ntf = MemoryPackSerializer.Serialize(ntfPac);
                var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_PUT_STONE, ntf);
                BroadCast(user.RoomNumber, user.SessionId, ntfData);
            }
            else if (checkResult == PUT_RESULT.WIN)
            {
                var winPac = new NtfWinPacket();
                winPac.Stone = (int)user.Stone;
                winPac.PosX = posX;
                winPac.PosY = posY;
                winPac.Id = user.UserId;
                var win = MemoryPackSerializer.Serialize(winPac);
                var winData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_WIN_GAME, win);
                BroadCast(user.RoomNumber, null, winData);
            }
        }
    }
}
