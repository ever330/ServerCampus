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
            packetHandlers.Add((short)PACKET_ID.REQ_SET_TOKEN, SetAuthToken);
            packetHandlers.Add((short)PACKET_ID.REQ_CHECK_AUTHTOKEN, CheckAuthToken);
        }

        public void SetAuthToken(DBRequestInfo req)
        {
            var setToken = MemoryPackSerializer.Deserialize<ReqSetToken>(req.Body);
            var result = _dbManager.SetAuthToken(setToken.UserId, setToken.AuthToken);

            var ntfLogin = new ResSetToken();
            ntfLogin.UserId = setToken.UserId;
            ntfLogin.AuthToken = setToken.AuthToken;
            if (result != ERROR_CODE.NONE)
            {
                ntfLogin.Result = false;
            }
            else
            {
                ntfLogin.Result = true;
            }
            var ntfData = MemoryPackSerializer.Serialize(ntfLogin);
            var reqInfo = new OmokBinaryRequestInfo((short)(ntfData.Length + OmokBinaryRequestInfo.HEADER_SIZE), (short)PACKET_ID.RES_SET_TOKEN, ntfData);
            reqInfo.SessionId = req.SessionId;
            _sendToPP(reqInfo);
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
