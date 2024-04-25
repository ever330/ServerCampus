using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;

namespace ChatServer
{
    public class CommomPacketHandler : PacketHandler
    {
        public void RegistPacketHandler(Dictionary<int, Action<ServerPacketData>> packetHandlerMap)
        {
            packetHandlerMap.Add((int)PACKET_ID.NTF_IN_CONNECT_CLIENT, NotifyInConnectClient);
            packetHandlerMap.Add((int)PACKET_ID.NTF_IN_DISCONNECT_CLIENT, NotifyInDisConnectClient);
            packetHandlerMap.Add((int)PACKET_ID.REQ_LOGIN, RequestLogin);
        }

        public void NotifyInConnectClient(ServerPacketData requestData)
        {
            MainServer.MainLogger.Debug($"Current Connected Session Count: {_serverNetwork.SessionCount}");
        }

        public void NotifyInDisConnectClient(ServerPacketData requestData)
        {
            var sessionId = requestData.SessionId;
            var user = _userManager.GetUser(sessionId);

            if (user != null)
            {
                var roomNum = user.RoomNumber;

                if (roomNum != PacketDefine.INVALID_ROOM_NUMBER)
                {
                    var packet = new InternalNtfRoomLeavePacket()
                    {
                        RoomNumber = roomNum,
                        UserId = user.ID(),
                    };

                    var packetBodyData = MemoryPackSerializer.Serialize(packet);
                    var internalPacket = new ServerPacketData();
                    internalPacket.Assign(sessionId, (Int16)PACKET_ID.NTF_IN_ROOM_LEAVE, packetBodyData);

                    _serverNetwork.Distribute(internalPacket);
                }

                _userManager.RemoveUser(sessionId);
            }

            MainServer.MainLogger.Debug($"Current Connected Session Count: {_serverNetwork.SessionCount}");
        }


        public void RequestLogin(ServerPacketData packetData)
        {
            var sessionId = packetData.SessionId;
            MainServer.MainLogger.Debug("로그인 요청 받음");

            try
            {
                if (_userManager.GetUser(sessionId) != null)
                {
                    ResponseLoginToClient(ERROR_CODE.LOGIN_ALREADY_WORKING, packetData.SessionId);
                    return;
                }

                var reqData = MemoryPackSerializer.Deserialize<ReqLoginPacket>(packetData.Body);
                var errorCode = _userManager.AddUser(reqData.UserId, sessionId);
                if (errorCode != ERROR_CODE.NONE)
                {
                    ResponseLoginToClient(errorCode, packetData.SessionId);

                    if (errorCode == ERROR_CODE.LOGIN_FULL_USER_COUNT)
                    {
                        NotifyMustCloseToClient(ERROR_CODE.LOGIN_FULL_USER_COUNT, packetData.SessionId);
                    }

                    return;
                }

                ResponseLoginToClient(errorCode, packetData.SessionId);

                MainServer.MainLogger.Debug("로그인 요청 답변 보냄");

            }
            catch (Exception ex)
            {
                MainServer.MainLogger.Error(ex.ToString());
            }
        }

        public void ResponseLoginToClient(ERROR_CODE errorCode, string sessionId)
        {
            var resLogin = new ResLoginPacket()
            {
                Result = (short)errorCode
            };

            var bodyData = MemoryPackSerializer.Serialize(resLogin);
            var sendData = PacketSerializer.Serialize(PACKET_ID.RES_LOGIN, bodyData);

            _serverNetwork.SendData(sessionId, sendData);
        }

        public void NotifyMustCloseToClient(ERROR_CODE errorCode, string sessionId)
        {
            var resLogin = new NtfMustClosePacket()
            {
                Result = (short)errorCode
            };

            var bodyData = MemoryPackSerializer.Serialize(resLogin);
            var sendData = PacketSerializer.Serialize(PACKET_ID.NTF_MUST_CLOSE, bodyData);

            _serverNetwork.SendData(sessionId, sendData);
        }
    }
}
