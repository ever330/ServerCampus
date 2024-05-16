using MemoryPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OmokGameServer
{
    public class RoomManager
    {
        List<Room> _roomList = new List<Room>();
        ConcurrentQueue<int> _emptyRoomQueue = new ConcurrentQueue<int>();

        Func<string, byte[], bool> _sendFunc;
        Action<DBRequestInfo> _reqToGameDB;

        int _roomMaxCount = 0;
        int _roomUserMax = 0;

        const int RoomGamingLimitMinite = 60;
        const int TurnLimitSecond = 30;

        public void Init(int roomMaxCount, int roomUserMax, Func<string, byte[], bool> sendFunc, Action<DBRequestInfo> reqToGameDB, int roomStartNumber)
        {
            _roomMaxCount = roomMaxCount;
            _roomUserMax = roomUserMax;
            for (int i = 0; i < _roomMaxCount; i++)
            {
                var room = new Room(roomStartNumber + i, roomUserMax);
                _roomList.Add(room);
                _emptyRoomQueue.Enqueue(roomStartNumber + i);
            }
            _sendFunc = sendFunc;
            _reqToGameDB = reqToGameDB;
        }

        public bool HasEmptyRoom()
        {
            if (!_emptyRoomQueue.IsEmpty)
            {
                return true;
            }
            return false;
        }

        public int GetEmptyRoomIndex()
        {
            int index = -1;
            if (!_emptyRoomQueue.IsEmpty)
            {
                _emptyRoomQueue.TryDequeue(out index);
            }
            return index;
        }

        public ErrorCode EnterRoom(User user, int roomNumber)
        {
            var curRoom = _roomList.Find(x => x.GetRoomNumber() == roomNumber);

            var enterRes = curRoom.EnterRoom(user);

            if (enterRes != ErrorCode.None)
            {
                return ErrorCode.RoomUserMax;
            }


            if (curRoom.GetUserCount() > 1)
            {
                var res = new ResEnterRoomPacket();
                res.OtherUserId = curRoom.GetOtherUserId(user.UserId);
                var data = MemoryPackSerializer.Serialize(res);
                var sendData = ClientPacket.MakeClientPacket(PacketId.ResEnterRoom, data);
                _sendFunc(user.SessionId, sendData);

                var ntf = new NtfNewUserPacket();
                ntf.Id = user.UserId;
                var ntfData = MemoryPackSerializer.Serialize(ntf);
                var ntfSendData = ClientPacket.MakeClientPacket(PacketId.NtfNewUser, ntfData);
                BroadCast(roomNumber, user.SessionId, ntfSendData);
            }

            return ErrorCode.None;
        }

        public ErrorCode MatchUsers(User userA, User userB, int roomNumber)
        {
            var curRoom = _roomList.Find(x => x.GetRoomNumber() == roomNumber);
            var enterResA = curRoom.EnterRoom(userA);
            var enterResB = curRoom.EnterRoom(userB);

            if (enterResA != ErrorCode.None || enterResB != ErrorCode.None)
            {
                return ErrorCode.RoomUserMax;
            }

            return ErrorCode.None;
        }

        public bool LeaveRoom(User user, int roomNumber)
        {
            var result = _roomList[roomNumber].LeaveRoom(user.UserId);
            if (result == ErrorCode.None)
            {
                user.LeaveRoom();
            }
            _emptyRoomQueue.Enqueue(roomNumber);

            var res = new ResLeaveRoomPacket();

            if (result != ErrorCode.None)
            {
                res.Result = false;
                return false;
            }
            res.Result = true;

            var data = MemoryPackSerializer.Serialize(res);
            var sendData = ClientPacket.MakeClientPacket(PacketId.ResLeaveRoom, data);
            _sendFunc(user.SessionId, sendData);

            var ntfLeave = new NtfLeaveUserPacket();
            ntfLeave.Id = user.UserId;
            var ntf = MemoryPackSerializer.Serialize(ntfLeave);
            var ntfData = ClientPacket.MakeClientPacket(PacketId.NtfLeaveUser, ntf);
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

        public void UserStateChange(User user, PacketId packetId, byte[] packet)
        {
            if (packetId == PacketId.ReqReady)
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
                    var resData = ClientPacket.MakeClientPacket(PacketId.ResReady, res);
                    _sendFunc(user.SessionId, resData);

                    var ntfPac = new NtfReadyStatePacket();
                    ntfPac.Id = user.UserId;
                    ntfPac.Result = true;
                    var ntf = MemoryPackSerializer.Serialize(ntfPac);
                    var ntfData = ClientPacket.MakeClientPacket(PacketId.NtfReadyState, ntf);

                    BroadCast(pac.RoomNumber, user.SessionId, ntfData);
                }
                else
                {
                    var startPac = new NtfGameStartPacket();
                    startPac.StartPlayer = _roomList[pac.RoomNumber].GameStart();

                    var ntf = MemoryPackSerializer.Serialize(startPac);
                    var ntfData = ClientPacket.MakeClientPacket(PacketId.NtfGameStart, ntf);

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
                var ntfData = ClientPacket.MakeClientPacket(PacketId.NtfReadyState, ntf);

                BroadCast(pac.RoomNumber, user.SessionId, ntfData);
            }
        }

        public void PutStone(int roomNumber, int posX, int posY)
        {
            var curRoom = _roomList[roomNumber];

            var curUser = curRoom.GetUserList()[curRoom.CurrentPlayerIndex];
            var curUserStone = curRoom.GetUserList()[curRoom.CurrentPlayerIndex].Stone;
            var checkResult = curRoom.CheckStoneCount(curUserStone, posX, posY);

            // 놓을 수 없는 곳에 놓았을 경우
            if (checkResult == PUT_RESULT.ERROR)
            {
                var errorPac = new ResPutStonePacket();
                errorPac.Result = false;
                var error = MemoryPackSerializer.Serialize(errorPac);
                var errorData = ClientPacket.MakeClientPacket(PacketId.ResPutStone, error);
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
                var resData = ClientPacket.MakeClientPacket(PacketId.ResPutStone, res);
                _sendFunc(curUser.SessionId, resData);

                var ntfPac = new NtfPutStonePacket();
                ntfPac.Stone = (int)curUserStone;
                ntfPac.PosX = posX;
                ntfPac.PosY = posY;
                var ntf = MemoryPackSerializer.Serialize(ntfPac);
                var ntfData = ClientPacket.MakeClientPacket(PacketId.NtfPutStone, ntf);
                BroadCast(roomNumber, curUser.SessionId, ntfData);
            }
            // 돌을 놓아서 게임 승리 시
            else if (checkResult == PUT_RESULT.WIN)
            {
                var winPac = new NtfWinPacket();
                winPac.Stone = (int)curUserStone;
                winPac.PosX = posX;
                winPac.PosY = posY;
                winPac.Id = curUser.UserId;
                var win = MemoryPackSerializer.Serialize(winPac);
                var winData = ClientPacket.MakeClientPacket(PacketId.NtfWinGame, win);
                BroadCast(roomNumber, null, winData);

                curRoom.EndGame(_reqToGameDB, curRoom.CurrentPlayerIndex);
            }
        }

        public void CheckRoomState(int checkRoomIndex)
        {
            for (int i = (checkRoomIndex * _roomMaxCount) / 4; i < ((checkRoomIndex + 1) * _roomMaxCount) / 4; i++)
            {
                if (_roomList[i].RoomState != ROOM_STATE.PLAYING)
                    continue;

                var difMinite = DateTime.Now - _roomList[i].StartTime;
                if (difMinite.TotalMinutes > RoomGamingLimitMinite)
                {
                    var draw = new NtfDrawPacket();
                    var ntf = MemoryPackSerializer.Serialize(draw);
                    var ntfData = ClientPacket.MakeClientPacket(PacketId.NtfDraw, ntf);
                    BroadCast(_roomList[i].GetRoomNumber(), null, ntfData);
                    continue;
                }

                var difSecond = DateTime.Now - _roomList[i].TurnTime;
                if (difSecond.TotalSeconds > TurnLimitSecond)
                {
                    var users = _roomList[i].GetUserList();
                    _roomList[i].TimeOut();
                    if (users[_roomList[i].CurrentPlayerIndex].TimeOutCount >= 6)
                    {
                        var winPac = new NtfTimeOutWinPacket();
                        if (_roomList[i].CurrentPlayerIndex == 0)
                        {
                            winPac.Id = users[1].UserId;
                            _roomList[i].EndGame(_reqToGameDB, 1);
                        }
                        else
                        {
                            winPac.Id = users[0].UserId;
                            _roomList[i].EndGame(_reqToGameDB, 0);
                        }
                        var win = MemoryPackSerializer.Serialize(winPac);
                        var winData = ClientPacket.MakeClientPacket(PacketId.NtfTimeOutWin, win);
                        BroadCast(_roomList[i].GetRoomNumber(), null, winData);
                    }
                    else
                    {
                        var timeOut = new NtfTimeOutPacket();
                        timeOut.Stone = (int)users[_roomList[i].CurrentPlayerIndex].Stone;
                        var ntf = MemoryPackSerializer.Serialize(timeOut);
                        var ntfData = ClientPacket.MakeClientPacket(PacketId.NtfTimeOut, ntf);
                        BroadCast(_roomList[i].GetRoomNumber(), null, ntfData);
                    }
                }
            }
        }
    }
}
