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
            packetHandlers.Add((short)PACKET_ID.REQ_READY, ReqReady);
            packetHandlers.Add((short)PACKET_ID.REQ_NOT_READY, ReqNotReady);
            packetHandlers.Add((short)PACKET_ID.REQ_PUT_STONE, ReqPutStone);
            packetHandlers.Add((short)PACKET_ID.REQ_SEND_CHECK_ROOM, ReqSendCheckRoom);
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
            var chatPacket = new NtfChatPacket();
            chatPacket.Id = tempUser.UserId;
            chatPacket.Chat = req.Chat;

            var data = MemoryPackSerializer.Serialize(chatPacket);
            var sendData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_ROOM_CHAT, data);

            _roomManager.BroadCast(tempUser.RoomNumber, packet.SessionId, sendData);
        }

        public void ReqReady(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 준비 완료 수신");

            _roomManager.UserStateChange(_userManager.GetUser(packet.SessionId), PACKET_ID.REQ_READY, packet.Body);
        }

        public void ReqNotReady(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 준비 해제 수신");

            _roomManager.UserStateChange(_userManager.GetUser(packet.SessionId), PACKET_ID.REQ_NOT_READY, packet.Body);
        }

        public void ReqPutStone(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 돌 놓기 수신");

            var req = MemoryPackSerializer.Deserialize<ReqPutStonePacket>(packet.Body);

            _roomManager.PutStone(req.RoomNumber, req.PosX, req.PosY);
        }

        public void ReqSendCheckRoom(OmokBinaryRequestInfo packet)
        {
            var req = MemoryPackSerializer.Deserialize<ReqSendCheckRoomPacket>(packet.Body);
            _logger.Info($"{req.CurrentIndex} 방 상태 확인 ");

            _roomManager.CheckRoomState(req.CurrentIndex);
        }
    }
}
