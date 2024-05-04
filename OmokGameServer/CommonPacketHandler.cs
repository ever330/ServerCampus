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
            packetHandlers.Add((short)PACKET_ID.RES_SET_TOKEN, ResSetToken);
            packetHandlers.Add((short)PACKET_ID.RES_USER_DATA, ResUserData);
            packetHandlers.Add((short)PACKET_ID.RES_HEART_BEAT, ResHeartBeat);
            packetHandlers.Add((short)PACKET_ID.REQ_SEND_HEART_BEAT, ReqSendHeartBeat);
            packetHandlers.Add((short)PACKET_ID.REQ_SEND_CHECK_SESSION, ReqSendCheckSession);
        }

        public void ReqUserLogin(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 로그인 시도");

            var req = MemoryPackSerializer.Deserialize<ReqLoginPacket>(packet.Body);

            var setToken = new ReqSetToken();
            setToken.UserId = req.Id;
            setToken.AuthToken = req.AuthToken;

            _sendToDB(DBRequest.MakeRequest((short)PACKET_ID.REQ_SET_TOKEN, packet.SessionId, MemoryPackSerializer.Serialize(setToken)));

            var getUserData = new ReqUserData();
            getUserData.UserId = req.Id;

            _sendToDB(DBRequest.MakeRequest((short)PACKET_ID.REQ_USER_DATA, packet.SessionId, MemoryPackSerializer.Serialize(getUserData)));
        }

        public void ResSetToken(OmokBinaryRequestInfo packet)
        {
            var resSet = MemoryPackSerializer.Deserialize<ResSetToken>(packet.Body);


            if (!resSet.Result && !_userManager.GetUsers().ContainsKey(packet.SessionId))
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

            if (!resUser.Result && !_userManager.GetUsers().ContainsKey(packet.SessionId))
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

        public void ResHeartBeat(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} : 하트비트 도착");
            _userManager.GetUser(packet.SessionId).HeartBeatTime = DateTime.Now;
        }

        public void ReqSendHeartBeat(OmokBinaryRequestInfo packet)
        {
            var req = MemoryPackSerializer.Deserialize<ReqSendHeartBeatPacket>(packet.Body);
            _userManager.CheckHeartBeat(req.CurrentIndex);
        }

        public void ReqSendCheckSession(OmokBinaryRequestInfo packet)
        {
            var req = MemoryPackSerializer.Deserialize<ReqSendCheckSessionPacket>(packet.Body);
            _userManager.CheckSession(req.CurrentIndex);
        }
    }
}
