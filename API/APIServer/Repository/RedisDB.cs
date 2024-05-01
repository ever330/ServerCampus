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

        public ERROR_CODE SetAuthToken(string id, string authToken)
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

                return ERROR_CODE.None;
            }
            catch
            {
                return ERROR_CODE.SetGameServerTokenError;
            }
        }

        public async Task<ERROR_CODE> CheckAuthToken(string id, string authToken)
        {

            var defaultExpiry = TimeSpan.FromDays(1);
            _redis = new RedisString<RedisUserInfo>(_connection, "UID" + id, defaultExpiry);

            try
            {
                var result = await _redis.GetAsync();

                if (authToken == result.Value.AuthToken)
                {
                    return ERROR_CODE.None;
                }
                else
                {
                    return ERROR_CODE.CheckTokenError;
                }
            }
            catch
            {
                return ERROR_CODE.CheckTokenError;
            }
        }
    }
}
