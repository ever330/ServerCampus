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
            packetHandlers.Add((short)PACKET_ID.RES_CHECK_AUTHTOKEN, ResCheckToken);
            packetHandlers.Add((short)PACKET_ID.RES_USER_DATA, ResUserData);
            packetHandlers.Add((short)PACKET_ID.REQ_HEART_BEAT, ReqHeartBeat);
            packetHandlers.Add((short)PACKET_ID.REQ_CHECK_HEART_BEAT, ReqCheckHeartBeat);
            packetHandlers.Add((short)PACKET_ID.REQ_CHECK_SESSION, ReqCheckSession);
        }

        public void ReqUserLogin(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 로그인 시도");

            var req = MemoryPackSerializer.Deserialize<ReqLoginPacket>(packet.Body);

            var checkToken = new ReqCheckAuthToken();
            checkToken.UserId = req.Id;
            checkToken.AuthToken = req.AuthToken;

            _sendToDB(DBRequest.MakeRequest((short)PACKET_ID.REQ_CHECK_AUTHTOKEN, packet.SessionId, MemoryPackSerializer.Serialize(checkToken)));

            var getUserData = new ReqUserData();
            getUserData.UserId = req.Id;

            _sendToDB(DBRequest.MakeRequest((short)PACKET_ID.REQ_USER_DATA, packet.SessionId, MemoryPackSerializer.Serialize(getUserData)));
        }

        public void ResCheckToken(OmokBinaryRequestInfo packet)
        {
            var resSet = MemoryPackSerializer.Deserialize<ResCheckAuthToken>(packet.Body);

            if (!resSet.Result && _userManager.GetUser(packet.SessionId) != null)
            {
                _userManager.RemoveUser(packet.SessionId);
                var res = new ResLoginPacket();
                res.Result = resSet.Result;
                var resData = MemoryPackSerializer.Serialize(res);
                var sendData = ClientPacket.MakeClientPacket(PACKET_ID.RES_LOGIN, resData);
                _sendFunc(packet.SessionId, sendData);
            }
        }

        public void ResUserData(OmokBinaryRequestInfo packet)
        {
            var resUser = MemoryPackSerializer.Deserialize<ResUserData>(packet.Body);

            if (!resUser.Result && _userManager.GetUser(packet.SessionId) != null)
            {
                _userManager.RemoveUser(packet.SessionId);
                var res = new ResLoginPacket();
                res.Result = resUser.Result; 
                var resData = MemoryPackSerializer.Serialize(res);
                var sendData = ClientPacket.MakeClientPacket(PACKET_ID.RES_LOGIN, resData);
                _sendFunc(packet.SessionId, sendData);
                return;
            }

            _userManager.UserLogin(packet.SessionId, resUser.UserId, resUser.WinCount, resUser.LoseCount);
        }

        public void ReqHeartBeat(OmokBinaryRequestInfo packet)
        {
            _userManager.GetUser(packet.SessionId).HeartBeatTime = DateTime.Now;

            var res = new ResHeartBeatPacket();
            var resData = MemoryPackSerializer.Serialize(res);
            var sendData = ClientPacket.MakeClientPacket(PACKET_ID.RES_HEART_BEAT, resData);
            _sendFunc(packet.SessionId, sendData);
        }

        public void ReqCheckHeartBeat(OmokBinaryRequestInfo packet)
        {
            var req = MemoryPackSerializer.Deserialize<ReqCheckHeartBeatPacket>(packet.Body);
            _userManager.CheckHeartBeat(req.CurrentIndex);
        }

        public void ReqCheckSession(OmokBinaryRequestInfo packet)
        {
            var req = MemoryPackSerializer.Deserialize<ReqCheckSessionPacket>(packet.Body);
            _userManager.CheckSession(req.CurrentIndex);
        }
    }
}
