using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;

namespace OmokGameServer
{
    public class RoomPacketHandler : PacketHandler
    {
        public void RegistPacketHandler(Dictionary<short, Action<OmokBinaryRequestInfo>> packetHandlers)
        {
            //packetHandlers.Add((short)PacketId.ReqEnterRoom, ReqEnterRoom);
            packetHandlers.Add((short)PacketId.ReqLeaveRoom, ReqLeaveRoom);
            packetHandlers.Add((short)PacketId.ReqRoomChat, ReqChat);
            packetHandlers.Add((short)PacketId.ReqReady, ReqReady);
            packetHandlers.Add((short)PacketId.ReqNotReady, ReqNotReady);
            packetHandlers.Add((short)PacketId.ReqPutStone, ReqPutStone);
            packetHandlers.Add((short)PacketId.ReqCheckRoom, ReqCheckRoom);
            packetHandlers.Add((short)PacketId.ReqMatchUsersEnter, ReqMatchUsersEnter);
        }

        //public void ReqEnterRoom(OmokBinaryRequestInfo packet)
        //{
        //    _logger.Info($"{packet.SessionId} 방 입장 시도");

        //    var req = MemoryPackSerializer.Deserialize<ReqEnterRoomPacket>(packet.Body);

        //    var result = _roomManager.EnterRoom(_userManager.GetUserBySessionId(packet.SessionId));

        //    if (!result)
        //    {
        //        _logger.Error($"{packet.SessionId} 방 입장 실패");
        //    }
        //}

        public void ReqLeaveRoom(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 방 퇴장 시도");

            var req = MemoryPackSerializer.Deserialize<ReqLeaveRoomPacket>(packet.Body);

            var result = _roomManager.LeaveRoom(_userManager.GetUserBySessionId(packet.SessionId), req.RoomNumber);

            if (!result)
            {
                _logger.Error($"{packet.SessionId} 방 퇴장 에러 발생");
            }
        }

        public void ReqChat(OmokBinaryRequestInfo packet)
        {
            var req = MemoryPackSerializer.Deserialize<ReqChatPacket>(packet.Body);

            _logger.Info($"{packet.SessionId} 채팅 수신 : {req.Chat}");

            var tempUser = _userManager.GetUserBySessionId(packet.SessionId);
            var chatPacket = new NtfChatPacket();
            chatPacket.Id = tempUser.UserId;
            chatPacket.Chat = req.Chat;

            var data = MemoryPackSerializer.Serialize(chatPacket);
            var sendData = ClientPacket.MakeClientPacket(PacketId.NtfRoomChat, data);

            _roomManager.BroadCast(tempUser.RoomNumber, packet.SessionId, sendData);
        }

        public void ReqReady(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 준비 완료 수신");

            _roomManager.UserStateChange(_userManager.GetUserBySessionId(packet.SessionId), PacketId.ReqReady, packet.Body);
        }

        public void ReqNotReady(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 준비 해제 수신");

            _roomManager.UserStateChange(_userManager.GetUserBySessionId(packet.SessionId), PacketId.ReqNotReady, packet.Body);
        }

        public void ReqPutStone(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 돌 놓기 수신");

            var req = MemoryPackSerializer.Deserialize<ReqPutStonePacket>(packet.Body);

            _roomManager.PutStone(req.RoomNumber, req.PosX, req.PosY);
        }

        public void ReqCheckRoom(OmokBinaryRequestInfo packet)
        {
            var req = MemoryPackSerializer.Deserialize<ReqCheckRoomPacket>(packet.Body);

            _roomManager.CheckRoomState(req.CurrentIndex);
        }

        public void ReqMatchUsersEnter(OmokBinaryRequestInfo packet)
        {
            var req = MemoryPackSerializer.Deserialize<ReqMatchUsersEnterPacket>(packet.Body);

            var userA = _userManager.GetUserByUserId(req.UserA);
            var userB = _userManager.GetUserByUserId(req.UserB);

            var result = _roomManager.MatchUsers(userA, userB, req.RoomNumber);

            if (result != ErrorCode.None)
            {
                _logger.Error($"유저 방 매칭 실패");
            }
        }
    }
}
