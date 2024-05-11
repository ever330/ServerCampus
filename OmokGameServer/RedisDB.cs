using CloudStructures.Structures;
using CloudStructures;
using StackExchange.Redis;
using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class RedisDB
    {
        RedisString<RedisUserInfo> _redis;

        public ERROR_CODE SetAuthToken(RedisConnection redisConnection, string id, string authToken)
        {
            try
            {
                RedisUserInfo newUser = new RedisUserInfo
                {
                    Id = id,
                    AuthToken = authToken
                };

                var defaultExpiry = TimeSpan.FromDays(1);
                _redis = new RedisString<RedisUserInfo>(redisConnection, "UID" + id, defaultExpiry);
                _redis.SetAsync(newUser).Wait();

                return ERROR_CODE.NONE;
            }
            catch
            {
                return ERROR_CODE.CHECK_TOKEN_ERROR;
            }
        }

        public ERROR_CODE CheckAuthToken(RedisConnection redisConnection, string id, string authToken, ILog logger)
        {
            try
            {
                var defaultExpiry = TimeSpan.FromDays(1);
                _redis = new RedisString<RedisUserInfo>(redisConnection, "UID" + id, defaultExpiry);

                var result = _redis.GetAsync();
                logger.Info($"기존 토큰 {result.Result.Value.AuthToken}, 비교 토큰 {authToken}");
                if (authToken == result.Result.Value.AuthToken)
                {
                    return ERROR_CODE.NONE;
                }
                else
                {
                    return ERROR_CODE.CHECK_TOKEN_ERROR;
                }
            }
            catch
            {
                return ERROR_CODE.CHECK_TOKEN_ERROR;
            }
        }
    }
}
