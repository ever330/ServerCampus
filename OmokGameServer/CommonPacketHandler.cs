using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;
using CloudStructures.Structures;

namespace OmokGameServer
{
    public class CommonPacketHandler : PacketHandler
    {
        public void RegistPacketHandler(Dictionary<short, Action<OmokBinaryRequestInfo>> packetHandlers)
        {
            packetHandlers.Add((short)PacketId.ReqLogin, ReqUserLogin);
            packetHandlers.Add((short)PacketId.ResCheckAuthToken, ResCheckToken);
            packetHandlers.Add((short)PacketId.ResUserData, ResUserData);
            packetHandlers.Add((short)PacketId.ReqHeartBeat, ReqHeartBeat);
            packetHandlers.Add((short)PacketId.ReqCheckHeartBeat, ReqCheckHeartBeat);
            packetHandlers.Add((short)PacketId.ReqCheckSession, ReqCheckSession);
        }

        public void ReqUserLogin(OmokBinaryRequestInfo packet)
        {
            _logger.Info($"{packet.SessionId} 로그인 시도");

            var req = MemoryPackSerializer.Deserialize<ReqLoginPacket>(packet.Body);

            _userManager.UserLogin(packet.SessionId, req.Id, req.AuthToken);
        }

        public void ResCheckToken(OmokBinaryRequestInfo packet)
        {
            var resSet = MemoryPackSerializer.Deserialize<ResCheckAuthToken>(packet.Body);

            var user = _userManager.GetUserByUserId(resSet.UserId);
            if (!resSet.Result && user != null)
            {
                _userManager.RemoveUser(user.SessionId);
                var res = new ResLoginPacket();
                res.Result = resSet.Result;
                var resData = MemoryPackSerializer.Serialize(res);
                var sendData = ClientPacket.MakeClientPacket(PacketId.ResLogin, resData);
                _sendFunc(user.SessionId, sendData);
            }
        }

        public void ResUserData(OmokBinaryRequestInfo packet)
        {
            var resUser = MemoryPackSerializer.Deserialize<ResUserData>(packet.Body);

            var user = _userManager.GetUserByUserId(resUser.UserId);
            if (!resUser.Result && user != null)
            {
                _userManager.RemoveUser(user.SessionId);
                var res = new ResLoginPacket();
                res.Result = resUser.Result; 
                var resData = MemoryPackSerializer.Serialize(res);
                var sendData = ClientPacket.MakeClientPacket(PacketId.ResLogin, resData);
                _sendFunc(user.SessionId, sendData);
                return;
            }

            _userManager.SetUserData(user.SessionId, resUser.Level, resUser.Exp, resUser.WinCount, resUser.LoseCount);
        }

        public void ReqHeartBeat(OmokBinaryRequestInfo packet)
        {
            var user = _userManager.GetUserBySessionId(packet.SessionId);
            if (user == null)
            {
                _logger.Error($"{packet.SessionId} 하트비트 유저 없음");
                return;
            }
            user.HeartBeatTime = DateTime.Now;

            var res = new ResHeartBeatPacket();
            var resData = MemoryPackSerializer.Serialize(res);
            var sendData = ClientPacket.MakeClientPacket(PacketId.ResHeartBeat, resData);
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
