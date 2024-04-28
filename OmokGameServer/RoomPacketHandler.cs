using ChatServer;
using MemoryPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class RoomPacketHandler : PacketHandler
    {
        public void RegistPacketHandler(Dictionary<short, Action<OmokBinaryRequestInfo>> packetHandlers)
        {
            packetHandlers.Add((short)PACKET_ID.REQ_ENTER_ROOM, ReqEnterRoom);
            packetHandlers.Add((short)PACKET_ID.REQ_ROOM_CHAT, ReqChat);
        }

        public void ReqEnterRoom(OmokBinaryRequestInfo packet)
        {
            _logger.LogInformation($"{packet.SessionId} 방 입장 시도");

            ReqEnterRoomPacket req = MemoryPackSerializer.Deserialize<ReqEnterRoomPacket>(packet.Body);

            int roomNumber = _roomManager.EnterRoom(_userManager.GetUser(packet.SessionId));
            
            ResEnterRoomPacket res = new ResEnterRoomPacket();
            res.RoomNumber = roomNumber;

            byte[] data = MemoryPackSerializer.Serialize(res);
            short packetId = (short)PACKET_ID.RES_ENTER_ROOM;
            short packetSize = (short)(PacketDefine.PACKET_HEADER + data.Length);

            byte[] sendData = new byte[packetSize];
            Array.Copy(BitConverter.GetBytes(packetSize), 0, sendData, 0, 2);
            Array.Copy(BitConverter.GetBytes(packetId), 0, sendData, 2, 2);
            Array.Copy(data, 0, sendData, 4, data.Length);
            bool sendResult = _sendFunc(packet.SessionId, sendData);

            _logger.LogInformation($"방 입장 결과 전송 {sendResult}");
        }

        public void ReqChat(OmokBinaryRequestInfo packet)
        {
            ReqChatPacket req = MemoryPackSerializer.Deserialize<ReqChatPacket>(packet.Body);

            _logger.LogInformation($"{packet.SessionId} 채팅 수신 : {req.Chat}");

            User tempUser = _userManager.GetUser(packet.SessionId);
            _roomManager.BroadCast(tempUser.RoomNumber, tempUser.UserId, req.Chat);
        }
    }
}
