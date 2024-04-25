using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;

namespace ChatServer
{
    public class RoomPacketHandler : PacketHandler
    {
        List<Room> _roomList = null;
        int _startRoomNumber;

        public void SetRooomList(List<Room> roomList)
        {
            _roomList = roomList;
            _startRoomNumber = roomList[0].Number;
        }

        public void RegistPacketHandler(Dictionary<int, Action<ServerPacketData>> packetHandlerMap)
        {
            packetHandlerMap.Add((int)PACKET_ID.REQ_ROOM_ENTER, RequestRoomEnter);
            packetHandlerMap.Add((int)PACKET_ID.REQ_ROOM_LEAVE, RequestLeave);
            packetHandlerMap.Add((int)PACKET_ID.NTF_IN_ROOM_LEAVE, NotifyLeaveInternal);
            packetHandlerMap.Add((int)PACKET_ID.REQ_ROOM_CHAT, RequestChat);
        }


        Room GetRoom(int roomNumber)
        {
            var index = roomNumber - _startRoomNumber;

            if (index < 0 || index >= _roomList.Count())
            {
                return null;
            }

            return _roomList[index];
        }

        (bool, Room, RoomUser) CheckRoomAndRoomUser(string userNetSessionId)
        {
            var user = _userManager.GetUser(userNetSessionId);
            if (user == null)
            {
                return (false, null, null);
            }

            var roomNumber = user.RoomNumber;
            var room = GetRoom(roomNumber);

            if (room == null)
            {
                return (false, null, null);
            }

            var roomUser = room.GetUserByNetSessionId(userNetSessionId);

            if (roomUser == null)
            {
                return (false, room, null);
            }

            return (true, room, roomUser);
        }



        public void RequestRoomEnter(ServerPacketData packetData)
        {
            var sessionId = packetData.SessionId;
            MainServer.MainLogger.Debug("RequestRoomEnter");

            try
            {
                var user = _userManager.GetUser(sessionId);

                if (user == null || user.IsConfirm(sessionId) == false)
                {
                    ResponseEnterRoomToClient(ERROR_CODE.ROOM_ENTER_INVALID_USER, sessionId);
                    return;
                }

                if (user.IsStateRoom())
                {
                    ResponseEnterRoomToClient(ERROR_CODE.ROOM_ENTER_INVALID_STATE, sessionId);
                    return;
                }

                var reqData = MemoryPackSerializer.Deserialize<ReqRoomEnterPacket>(packetData.Body);

                var room = GetRoom(reqData.RoomNumber);

                if (room == null)
                {
                    ResponseEnterRoomToClient(ERROR_CODE.ROOM_ENTER_INVALID_ROOM_NUMBER, sessionId);
                    return;
                }

                if (room.AddUser(user.ID(), sessionId) == false)
                {
                    ResponseEnterRoomToClient(ERROR_CODE.ROOM_ENTER_FAIL_ADD_USER, sessionId);
                    return;
                }


                user.EnteredRoom(reqData.RoomNumber);

                room.NotifyRoomUsers(sessionId);
                room.NofifyNewUser(sessionId, user.ID());

                ResponseEnterRoomToClient(ERROR_CODE.NONE, sessionId);

                MainServer.MainLogger.Debug("RequestEnterInternal - Success");
            }
            catch (Exception ex)
            {
                MainServer.MainLogger.Error(ex.ToString());
            }
        }

        void ResponseEnterRoomToClient(ERROR_CODE errorCode, string sessionID)
        {
            var resRoomEnter = new ResRoomEnterPacket()
            {
                Result = (short)errorCode
            };

            var bodyData = MemoryPackSerializer.Serialize(resRoomEnter);
            var sendData = PacketSerializer.Serialize(PACKET_ID.RES_ROOM_ENTER, bodyData);

            _serverNetwork.SendData(sessionID, sendData);
        }

        public void RequestLeave(ServerPacketData packetData)
        {
            var sessionId = packetData.SessionId;
            MainServer.MainLogger.Debug("로그인 요청 받음");

            try
            {
                var user = _userManager.GetUser(sessionId);
                if (user == null)
                {
                    return;
                }

                if (LeaveRoomUser(sessionId, user.RoomNumber) == false)
                {
                    return;
                }

                user.LeaveRoom();

                ResponseLeaveRoomToClient(sessionId);

                MainServer.MainLogger.Debug("Room RequestLeave - Success");
            }
            catch (Exception ex)
            {
                MainServer.MainLogger.Error(ex.ToString());
            }
        }

        bool LeaveRoomUser(string sessionId, int roomNumber)
        {
            MainServer.MainLogger.Debug($"LeaveRoomUser. SessionID:{sessionId}");

            var room = GetRoom(roomNumber);
            if (room == null)
            {
                return false;
            }

            var roomUser = room.GetUserByNetSessionId(sessionId);
            if (roomUser == null)
            {
                return false;
            }

            var userID = roomUser.UserId;
            room.RemoveUser(roomUser);

            room.NotifyLeaveUser(userID);
            return true;
        }

        void ResponseLeaveRoomToClient(string sessionId)
        {
            var resRoomLeave = new ResRoomLeavePacket()
            {
                Result = (short)ERROR_CODE.NONE
            };

            var bodyData = MemoryPackSerializer.Serialize(resRoomLeave);
            var sendData = PacketSerializer.Serialize(PACKET_ID.RES_ROOM_LEAVE, bodyData);

            _serverNetwork.SendData(sessionId, sendData);
        }

        public void NotifyLeaveInternal(ServerPacketData packetData)
        {
            var sessionId = packetData.SessionId;
            MainServer.MainLogger.Debug($"NotifyLeaveInternal. SessionId: {sessionId}");

            var reqData = MemoryPackSerializer.Deserialize<InternalNtfRoomLeavePacket>(packetData.Body);
            LeaveRoomUser(sessionId, reqData.RoomNumber);
        }

        public void RequestChat(ServerPacketData packetData)
        {
            var sessionId = packetData.SessionId;
            MainServer.MainLogger.Debug("Room RequestChat");

            try
            {
                var roomObject = CheckRoomAndRoomUser(sessionId);

                if (roomObject.Item1 == false)
                {
                    return;
                }


                var reqData = MemoryPackSerializer.Deserialize<ReqRoomChatPacket>(packetData.Body);

                var notifyPacket = new NtfRoomChatPacket()
                {
                    UserId = roomObject.Item3.UserId,
                    ChatMessage = reqData.ChatMessage
                };

                var Body = MemoryPackSerializer.Serialize(notifyPacket);
                var sendData = PacketSerializer.Serialize(PACKET_ID.NTF_ROOM_CHAT, Body);

                roomObject.Item2.Broadcast("", sendData);

                MainServer.MainLogger.Debug("Room RequestChat - Success");
            }
            catch (Exception ex)
            {
                MainServer.MainLogger.Error(ex.ToString());
            }
        }
    }
}
