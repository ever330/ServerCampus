using CloudStructures;
using MemoryPack;
using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class RedisDBHandler
    {
        RedisDB _redisDB;
        ILog _logger;
        Action<OmokBinaryRequestInfo> _sendToPP;

        public void Init(RedisDB redisDB, ILog logger, Func<string, byte[], bool> sendFunc, Action<OmokBinaryRequestInfo> sendToPP)
        {
            _redisDB = redisDB;
            _logger = logger;
            _sendToPP = sendToPP;
        }

        public void RegistPacketHandler(Dictionary<short, Action<RedisConnection, DBRequestInfo>> packetHandlers)
        {
            packetHandlers.Add((short)PACKET_ID.REQ_CHECK_AUTHTOKEN, CheckAuthToken);
        }

        public void CheckAuthToken(RedisConnection redisConnection, DBRequestInfo req)
        {
            var checkToken = MemoryPackSerializer.Deserialize<ReqCheckAuthToken>(req.Body);

            var result = _redisDB.CheckAuthToken(redisConnection, checkToken.UserId, checkToken.AuthToken, _logger);

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
            _sendToPP(reqInfo);
        }
    }
}
