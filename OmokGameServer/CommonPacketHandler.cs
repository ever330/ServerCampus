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

            var dbReq = new ReqSetAuthToken();
            dbReq.UserId = req.Id;
            dbReq.AuthToken = req.AuthToken;

            _sendToDB(DBRequest.MakeRequest((short)PACKET_ID.REQ_REDIS_LOGIN, packet.SessionId, MemoryPackSerializer.Serialize(dbReq)));
        }

        public void ResHeartBeat(OmokBinaryRequestInfo packet)
        {
            _userManager.GetUser(packet.SessionId).HeartBeatTime = DateTime.Now;
        }
    }
}
