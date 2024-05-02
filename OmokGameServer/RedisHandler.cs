using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;
using SuperSocket.SocketBase.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OmokGameServer
{
    public class RedisHandler : DBHandler
    {
        public void RegistPacketHandler(Dictionary<short, Action<DBRequestInfo>> packetHandlers)
        {
            packetHandlers.Add((short)PACKET_ID.REQ_REDIS_LOGIN, SetAuthToken);
            packetHandlers.Add((short)PACKET_ID.REQ_CHECK_AUTHTOKEN, CheckAuthToken);
        }

        public void SetAuthToken(DBRequestInfo req)
        {
            var setToken = MemoryPackSerializer.Deserialize<ReqSetAuthToken>(req.Body);
            var result = _dbManager.SetAuthToken(setToken.UserId, setToken.AuthToken);

            var res = new ResLoginPacket();
            if (result != ERROR_CODE.NONE)
            {
                res.Result = false;
            }
            else
            {
                res.Result = true;
                _userManager.UserLogin(req.SessionId, setToken.UserId, setToken.AuthToken);
            }

            var resData = MemoryPackSerializer.Serialize(res);
            var sendData = ClientPacket.MakeClientPacket(PACKET_ID.RES_LOGIN, resData);
            _sendFunc(req.SessionId, sendData);
        }

        public void CheckAuthToken(DBRequestInfo req)
        {
            var checkToken = MemoryPackSerializer.Deserialize<ReqCheckAuthToken>(req.Body);
            var result = _dbManager.CheckAuthToken(checkToken.UserId, checkToken.AuthToken);

            if (result != ERROR_CODE.NONE)
            {
                _logger.Error($"{checkToken.UserId} : 인증 토큰 검사 에러");
            }
        }
    }
}
