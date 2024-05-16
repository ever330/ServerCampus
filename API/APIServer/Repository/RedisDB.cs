using APIServer.Models.DAO;
using CloudStructures.Structures;

namespace APIServer.Repository
{
    public class RedisDB : IRedisDB
    {
        CloudStructures.RedisConnection _connection;
        RedisString<RedisUserInfo> _redis;
        CloudStructures.RedisConfig _conf;

        public RedisDB(IConfiguration config)
        {
            _conf = new CloudStructures.RedisConfig("HiveUsers", config.GetConnectionString("RedisDB"));
            _connection = new CloudStructures.RedisConnection(_conf);
        }

        public void Dispose()
        {
            _connection.GetConnection().Close();
        }

        public async Task<ErrorCode> SetAuthToken(string email, string authToken)
        {
            RedisUserInfo newUser = new RedisUserInfo
            {
                Email = email,
                AuthToken = authToken
            };

            try
            {
                if (!_connection.GetConnection().IsConnected)
                {
                    _connection = new CloudStructures.RedisConnection(_conf);
                }

                var defaultExpiry = TimeSpan.FromDays(1);
                _redis = new RedisString<RedisUserInfo>(_connection, "UID" + email, defaultExpiry);
                await _redis.SetAsync(newUser);

                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.SetGameServerTokenError;
            }
        }

        public async Task<ErrorCode> CheckAuthToken(string email, string authToken)
        {
            try
            {
                if (!_connection.GetConnection().IsConnected)
                {
                    _connection = new CloudStructures.RedisConnection(_conf);
                }

                var defaultExpiry = TimeSpan.FromDays(1);
                _redis = new RedisString<RedisUserInfo>(_connection, "UID" + email, defaultExpiry);

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
