using APIServer.Models.DAO;
using CloudStructures.Structures;

namespace APIServer.Repository
{
    public class RedisDB : IRedisDB
    {
        private CloudStructures.RedisConnection _connection;
        private RedisString<RedisUserInfo> _redis;

        public RedisDB(IConfiguration config)
        {
            var conf = new CloudStructures.RedisConfig("HiveUsers", config.GetConnectionString("RedisDB"));
            _connection = new CloudStructures.RedisConnection(conf);
        }

        public void Dispose()
        {
            _connection.GetConnection().Close();
        }

        public ErrorCode SetAuthToken(string id, string authToken)
        {
            try
            {
                RedisUserInfo newUser = new RedisUserInfo
                {
                    Id = id,
                    AuthToken = authToken
                };

                var defaultExpiry = TimeSpan.FromDays(1);
                _redis = new RedisString<RedisUserInfo>(_connection, "UID" + id, defaultExpiry);
                _redis.SetAsync(newUser).Wait();

                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.SetGameServerTokenError;
            }
        }

        public async Task<ErrorCode> CheckAuthToken(string id, string authToken)
        {

            var defaultExpiry = TimeSpan.FromDays(1);
            _redis = new RedisString<RedisUserInfo>(_connection, "UID" + id, defaultExpiry);

            try
            {
                var result = await _redis.GetAsync();

                if (authToken == result.Value.AuthToken)
                {
                    return ErrorCode.None;
                }
                else
                {
                    return ErrorCode.CheckTokenError;
                }
            }
            catch
            {
                return ErrorCode.CheckTokenError;
            }
        }
    }
}
