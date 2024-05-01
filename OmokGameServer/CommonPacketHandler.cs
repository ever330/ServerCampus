using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;

namespace OmokGameServer
{
    public class CommonPacketHandler : PacketHandler
    {
        public void RegistPacketHandler(Dictionary<short, Action<OmokBinaryRequestInfo>> packetHandlers)
        {
            packetHandlers.Add((short)PACKET_ID.REQ_LOGIN, ReqUserLogin);
            packetHandlers.Add((short)PACKET_ID.RES_HEART_BEAT, ResHeartBeat);
        }

        public void ReqUserLogin(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 로그인 시도");

            var req = MemoryPackSerializer.Deserialize<ReqLoginPacket>(packet.Body);

            var result = _userManager.UserLogin(packet.SessionId, req.Id, req.AuthToken);

            _logger.Debug($"로그인 결과 : {result}");
            _logger.Info($"현재 유저 수 : {_userManager.GetUserCount()}");
        }

        public void ResHeartBeat(OmokBinaryRequestInfo packet)
        {
            //_logger.Info($"{packet.SessionId} 하트비트 도착");

            _userManager.GetUser(packet.SessionId).HeartBeatTime = DateTime.Now;
        }
    }
}
