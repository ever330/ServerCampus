using CloudStructures.Structures;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class GameDB
    {
        public Tuple<ErrorCode, UserGameData?> GetUserData(QueryFactory queryFactory, string id)
        {
            try
            {
                var userData = queryFactory.Query("userGameData").Select().Where("id", id).FirstOrDefault<UserGameData>();

                if (userData == null)
                {
                    return new Tuple<ErrorCode, UserGameData?>(ErrorCode.UserDataNotExist, null);
                }
                return new Tuple<ErrorCode, UserGameData?>(ErrorCode.None, userData);
            }
            catch
            {
                return new Tuple<ErrorCode, UserGameData?>(ErrorCode.GetUserDataError, null);
            }
        }

        public ErrorCode UpdateGameResult(QueryFactory queryFactory, string id, int win, int lose)
        {
            try
            {
                var count = queryFactory.Query("userGameData").Where("id", id).Update(new
                {
                    winCount = win,
                    loseCount = lose
                });

                if (count == 0)
                {
                    return ErrorCode.UpdateUserInfoError;
                }
                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.UpdateUserInfoError;
            }
        }
    }
}
