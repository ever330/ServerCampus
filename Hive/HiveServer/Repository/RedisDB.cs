using System;
using CloudStructures.Structures;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using HiveServer.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using StackExchange.Redis;

namespace HiveServer.Repository
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

        public ErrorCode SetAuthToken(string email, string authToken)
        {
            RedisUserInfo newUser = new RedisUserInfo
            {
                Email = email,
                AuthToken = authToken
            };

            var defaultExpiry = TimeSpan.FromDays(1);

            try
            {
                if (!_connection.GetConnection().IsConnected)
                {
                    _connection = new CloudStructures.RedisConnection(_conf);
                }

                _redis = new RedisString<RedisUserInfo>(_connection, "UID" + email, defaultExpiry);
                _redis.SetAsync(newUser).Wait();

                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.SetTokenError;
            }
        }

        public async Task<ErrorCode> VerifyToken(string email, string authToken)
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
                    return ErrorCode.VerifyTokenError;
                }
            }
            catch
            {
                return ErrorCode.VerifyTokenError;
            }
        }
    }
}
