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
            packetHandlers.Add((short)PACKET_ID.REQ_ENTER_ROOM, ReqEnterRoom);
            packetHandlers.Add((short)PACKET_ID.REQ_LEAVE_ROOM, ReqLeaveRoom);
            packetHandlers.Add((short)PACKET_ID.REQ_ROOM_CHAT, ReqChat);
        }

        public void ReqEnterRoom(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 방 입장 시도");

            var req = MemoryPackSerializer.Deserialize<ReqEnterRoomPacket>(packet.Body);

            var result = _roomManager.EnterRoom(_userManager.GetUser(packet.SessionId));

            if (!result)
            {
                _logger.Error($"{packet.SessionId} 방 입장 실패");
            }
        }

        public void ReqLeaveRoom(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 방 퇴장 시도");

            var req = MemoryPackSerializer.Deserialize<ReqLeaveRoomPacket>(packet.Body);

            var result = _roomManager.LeaveRoom(_userManager.GetUser(packet.SessionId), req.RoomNumber);

            if (!result)
            {
                _logger.Error($"{packet.SessionId} 방 퇴장 에러 발생");
            }
        }

        public void ReqChat(OmokBinaryRequestInfo packet)
        {
            var req = MemoryPackSerializer.Deserialize<ReqChatPacket>(packet.Body);

            _logger.Info($"{packet.SessionId} 채팅 수신 : {req.Chat}");

            var tempUser = _userManager.GetUser(packet.SessionId);
            _roomManager.BroadCastChat(tempUser.RoomNumber, tempUser.UserId, req.Chat);
        }
    }
}
