using CloudStructures.Structures;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class DBManager : IDisposable
    {
        MySqlConnection _mySqlConnection;
        MySqlCompiler _compiler;
        QueryFactory _queryFactory;

        CloudStructures.RedisConnection _redisConnection;
        RedisString<RedisUserInfo> _redis;

        public DBManager(ServerOption serverOption)
        {
            _mySqlConnection = new MySqlConnection(serverOption.GameDB);
            _compiler = new MySqlCompiler();

            _mySqlConnection.Open();
            _queryFactory = new QueryFactory(_mySqlConnection, _compiler);

            var conf = new CloudStructures.RedisConfig("HiveUsers", serverOption.RedisDB);
            _redisConnection = new CloudStructures.RedisConnection(conf);
        }

        public void Dispose()
        {
            _mySqlConnection.Close();
            _redisConnection.GetConnection().Close();
        }

        public Tuple<ERROR_CODE, UserGameData?> GetUserData(string id)
        {
            try
            {
                var userData = _queryFactory.Query("user_game_data").Select().Where("id", id).FirstOrDefaultAsync<UserGameData>();

                if (userData == null)
                {
                    return new Tuple<ERROR_CODE, UserGameData?>(ERROR_CODE.USER_DATA_NOT_EXIST, null);
                }
                return new Tuple<ERROR_CODE, UserGameData?>(ERROR_CODE.NONE, userData.Result);
            }
            catch
            {
                return new Tuple<ERROR_CODE, UserGameData?>(ERROR_CODE.GET_USER_DATA_ERROR, null);
            }
        }

        public ERROR_CODE UpdateGameResult(string id, int winCount, int loseCount)
        {
            try
            {
                var userData = _queryFactory.Query("user_game_data").Where("id", id).UpdateAsync(new
                    {
                        winCunt = winCount,
                        loseCount = loseCount
                    });

                if (userData == null)
                {
                    return ERROR_CODE.USER_DATA_NOT_EXIST;
                }
                return ERROR_CODE.NONE;
            }
            catch
            {
                return ERROR_CODE.GET_USER_DATA_ERROR;
            }
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
                _redis = new RedisString<RedisUserInfo>(_redisConnection, "UID" + id, defaultExpiry);
                _redis.SetAsync(newUser).Wait();

                return ERROR_CODE.NONE;
            }
            catch
            {
                return ERROR_CODE.SET_GAME_SERVER_TOKEN_ERROR;
            }
        }

        public ERROR_CODE CheckAuthToken(string id, string authToken)
        {

            var defaultExpiry = TimeSpan.FromDays(1);
            _redis = new RedisString<RedisUserInfo>(_redisConnection, "UID" + id, defaultExpiry);

            try
            {
                var result = _redis.GetAsync();

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
