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
            packetHandlers.Add((short)PACKET_ID.REQ_CHECK_AUTHTOKEN, CheckAuthToken);
        }

        public void CheckAuthToken(DBRequestInfo req)
        {
            var checkToken = MemoryPackSerializer.Deserialize<ReqCheckAuthToken>(req.Body);
            var result = _dbManager.CheckAuthToken(checkToken.UserId, checkToken.AuthToken, _logger);

            var res = new ResCheckAuthToken();
            res.UserId = checkToken.UserId;
            res.AuthToken = checkToken.AuthToken;

            if (result != ERROR_CODE.NONE)
            {
                _logger.Error($"{checkToken.UserId} : 인증 토큰 검사 에러");
                res.Result = false;
            }
            else
            {
                res.Result = true;
            }

            var resData = MemoryPackSerializer.Serialize(res);
            var reqInfo = new OmokBinaryRequestInfo((short)(resData.Length + OmokBinaryRequestInfo.HEADER_SIZE), (short)PACKET_ID.RES_CHECK_AUTHTOKEN, resData);
            reqInfo.SessionId = req.SessionId;
            _sendToPP(reqInfo);
        }
    }
}
