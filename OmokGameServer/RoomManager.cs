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
        DBManager _dbManager;

        Func<string, byte[], bool> _sendFunc;
        Action<DBRequestInfo> _sendToDB;

        int _roomMaxCount = 0;
        int _roomUserMax = 0;

        const int RoomGamingLimitMinite = 60;
        const int TurnLimitSecond = 5;

        public void Init(DBManager dbManager, int roomMaxCount, int roomUserMax, Func<string, byte[], bool> sendFunc, Action<DBRequestInfo> sendToDB)
        {
            _dbManager = dbManager;
            _roomMaxCount = roomMaxCount;
            _roomUserMax = roomUserMax;
            for (int i = 0; i < _roomMaxCount; i++)
            {
                var room = new Room(i, roomUserMax);
                _roomList.Add(room);
                _roomIndexQueue.Enqueue(i);
            }
            _sendFunc = sendFunc;
            _sendToDB = sendToDB;
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
                    if (user.UserId != users[i].UserId)
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
            if (result == ERROR_CODE.NONE)
            {
                user.LeaveRoom();
            }
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

                var curUser = _roomList[pac.RoomNumber].GetUserList().Find(x => x.UserId == user.UserId);
                curUser.State = USER_STATE.READY;

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
                    startPac.StartPlayer = _roomList[pac.RoomNumber].GameStart();

                    var ntf = MemoryPackSerializer.Serialize(startPac);
                    var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_GAME_START, ntf);

                    BroadCast(pac.RoomNumber, null, ntfData);
                }
            }
            else
            {
                var pac = MemoryPackSerializer.Deserialize<ReqNotReadyPacket>(packet);

                var curUser = _roomList[pac.RoomNumber].GetUserList().Find(x => x.UserId == user.UserId);
                curUser.State = USER_STATE.NONE;

                var ntfPac = new NtfReadyStatePacket();
                ntfPac.Id = user.UserId;
                ntfPac.Result = false;
                var ntf = MemoryPackSerializer.Serialize(ntfPac);
                var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_READY_STATE, ntf);

                BroadCast(pac.RoomNumber, user.SessionId, ntfData);
            }
        }

        public void PutStone(int roomNumber, int posX, int posY)
        {
            /// 흑돌 쌍3 쌍4 6목 처리 및 승리 확인
            var curRoom = _roomList[roomNumber];

            var curUser = curRoom.GetUserList()[curRoom.CurrentPlayerIndex];
            var curUserStone = curRoom.GetUserList()[curRoom.CurrentPlayerIndex].Stone;
            var checkResult = curRoom.CheckStoneCount(curUserStone, posX, posY);

            if (checkResult == PUT_RESULT.ERROR)
            {
                var errorPac = new ResPutStonePacket();
                errorPac.Result = false;
                var error = MemoryPackSerializer.Serialize(errorPac);
                var errorData = ClientPacket.MakeClientPacket(PACKET_ID.RES_PUT_STONE, error);
                _sendFunc(curUser.SessionId, errorData);
                return;
            }
            else if (checkResult == PUT_RESULT.NONE)
            {
                curRoom.PutStone(curUserStone, posX, posY);
                curUser.TimeOutCount = 0;

                var resPac = new ResPutStonePacket();
                resPac.Result = true;
                var res = MemoryPackSerializer.Serialize(resPac);
                var resData = ClientPacket.MakeClientPacket(PACKET_ID.RES_PUT_STONE, res);
                _sendFunc(curUser.SessionId, resData);

                var ntfPac = new NtfPutStonePacket();
                ntfPac.Stone = (int)curUserStone;
                ntfPac.PosX = posX;
                ntfPac.PosY = posY;
                var ntf = MemoryPackSerializer.Serialize(ntfPac);
                var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_PUT_STONE, ntf);
                BroadCast(roomNumber, curUser.SessionId, ntfData);
            }
            else if (checkResult == PUT_RESULT.WIN)
            {
                var winPac = new NtfWinPacket();
                winPac.Stone = (int)curUserStone;
                winPac.PosX = posX;
                winPac.PosY = posY;
                winPac.Id = curUser.UserId;
                var win = MemoryPackSerializer.Serialize(winPac);
                var winData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_WIN_GAME, win);
                BroadCast(roomNumber, null, winData);
                curUser.TimeOutCount = 0;

                var winUser = new ReqUpdateWinLose();
                winUser.UserId = curUser.UserId;
                winUser.Result = true;

                _sendToDB(DBRequest.MakeRequest((short)PACKET_ID.REQ_UPDATE_RESULT, curUser.SessionId, MemoryPackSerializer.Serialize(winUser)));

                foreach (var tempUser in curRoom.GetUserList())
                {
                    if (curUser.UserId != tempUser.UserId)
                    {
                        var loseUser = new ReqUpdateWinLose();
                        loseUser.UserId = tempUser.UserId;
                        loseUser.Result = false;

                        _sendToDB(DBRequest.MakeRequest((short)PACKET_ID.REQ_UPDATE_RESULT, curUser.SessionId, MemoryPackSerializer.Serialize(loseUser)));

                        tempUser.TimeOutCount = 0;
                    }
                }
                curRoom.EndGame();
            }
        }

        public void CheckRoomState(int checkRoomIndex)
        {
            int counter = 0;
            foreach (var room in _roomList)
            {
                if (counter >= checkRoomIndex * ((float)_roomMaxCount / 4) && counter < (checkRoomIndex + 1) * ((float)_roomMaxCount / 4))
                {
                    if (room.RoomState != ROOM_STATE.PLAYING)
                        continue;

                    var difMinite = DateTime.Now - room.StartTime;
                    if (difMinite.TotalMinutes > RoomGamingLimitMinite)
                    {
                        var draw = new NtfDrawPacket();
                        var ntf = MemoryPackSerializer.Serialize(draw);
                        var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_DRAW, ntf);
                        BroadCast(room.GetRoomNumber(), null, ntfData);
                        continue;
                    }

                    var difSecond = DateTime.Now - room.TurnTime;
                    if (difSecond.TotalSeconds > TurnLimitSecond)
                    {
                        var userList = room.GetUserList();
                        userList[room.CurrentPlayerIndex].TimeOutCount++;
                        if (userList[room.CurrentPlayerIndex].TimeOutCount >= 6)
                        {
                            var winUser = new ReqUpdateWinLose();
                            winUser.Result = true;

                            var loseUser = new ReqUpdateWinLose();
                            loseUser.Result = false;

                            var winPac = new NtfTimeOutWinPacket();
                            if (room.CurrentPlayerIndex == 0)
                            {
                                winPac.Id = userList[1].UserId;
                                winUser.UserId = userList[1].UserId;
                                loseUser.UserId = userList[0].UserId;

                                _sendToDB(DBRequest.MakeRequest((short)PACKET_ID.REQ_UPDATE_RESULT, userList[1].SessionId, MemoryPackSerializer.Serialize(winUser)));

                                _sendToDB(DBRequest.MakeRequest((short)PACKET_ID.REQ_UPDATE_RESULT, userList[0].SessionId, MemoryPackSerializer.Serialize(loseUser)));
                            }
                            else
                            {
                                winPac.Id = userList[0].UserId;
                                winUser.UserId = userList[0].UserId;
                                loseUser.UserId = userList[1].UserId;

                                _sendToDB(DBRequest.MakeRequest((short)PACKET_ID.REQ_UPDATE_RESULT, userList[0].SessionId, MemoryPackSerializer.Serialize(winUser)));

                                _sendToDB(DBRequest.MakeRequest((short)PACKET_ID.REQ_UPDATE_RESULT, userList[1].SessionId, MemoryPackSerializer.Serialize(loseUser)));
                            }

                            var win = MemoryPackSerializer.Serialize(winPac);
                            var winData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_TIME_OUT_WIN, win);
                            BroadCast(room.GetRoomNumber(), null, winData);
                            userList[0].TimeOutCount = 0;
                            userList[1].TimeOutCount = 0;
                        }
                        else
                        {
                            var timeOut = new NtfTimeOutPacket();
                            timeOut.Stone = (int)userList[room.CurrentPlayerIndex].Stone;
                            var ntf = MemoryPackSerializer.Serialize(timeOut);
                            var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_TIME_OUT, ntf);
                            BroadCast(room.GetRoomNumber(), null, ntfData);
                        }

                        room.TurnTime = DateTime.Now;
                        if (room.CurrentPlayerIndex == 0)
                        {
                            room.CurrentPlayerIndex = 1;
                        }
                        else
                        {
                            room.CurrentPlayerIndex = 0;
                        }
                    }
                }
                counter++;
            }
        }
    }
}
