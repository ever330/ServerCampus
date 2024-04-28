using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatServer;
using MemoryPack;

namespace OmokGameServer
{
    public class LobbyPacketHandler : PacketHandler
    {
        public void RegistPacketHandler(Dictionary<short, Action<OmokBinaryRequestInfo>> packetHandlers)
        {
            packetHandlers.Add((short)PACKET_ID.REQ_LOGIN, ReqUserLogin);
        }

        public void ReqUserLogin(OmokBinaryRequestInfo packet)
        {
            _logger.LogInformation($"{packet.SessionId} 로그인 시도");

            ReqLoginPacket req = MemoryPackSerializer.Deserialize<ReqLoginPacket>(packet.Body);

            _logger.LogInformation($"아이디 : {req.Id}");

            _userManager.AddUser(req.Id, packet.SessionId);

            ResLoginPacket res = new ResLoginPacket();
            res.Result = true;
            byte[] data = MemoryPackSerializer.Serialize(res);
            short packetId = (short)PACKET_ID.RES_LOGIN;
            short packetSize = (short)(PacketDefine.PACKET_HEADER + data.Length);

            byte[] sendData = new byte[packetSize];
            Array.Copy(BitConverter.GetBytes(packetSize), 0, sendData, 0, 2);
            Array.Copy(BitConverter.GetBytes(packetId), 0, sendData, 2, 2);
            Array.Copy(data, 0, sendData, 4, data.Length);
            bool sendResult = _sendFunc(packet.SessionId, sendData);

            _logger.LogInformation($"로그인 결과 전송 {sendResult}");
        }

    }
}
