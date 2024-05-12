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
        public Tuple<ERROR_CODE, UserGameData?> GetUserData(QueryFactory queryFactory, string id)
        {
            try
            {
                var userData = queryFactory.Query("userGameData").Select().Where("id", id).FirstOrDefault<UserGameData>();

                if (userData == null)
                {
                    return new Tuple<ERROR_CODE, UserGameData?>(ERROR_CODE.USER_DATA_NOT_EXIST, null);
                }
                return new Tuple<ERROR_CODE, UserGameData?>(ERROR_CODE.NONE, userData);
            }
            catch
            {
                return new Tuple<ERROR_CODE, UserGameData?>(ERROR_CODE.GET_USER_DATA_ERROR, null);
            }
        }

        public ERROR_CODE UpdateGameResult(QueryFactory queryFactory, string id, int win, int lose)
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
                    return ERROR_CODE.UPDATE_USER_INFO_ERROR;
                }
                return ERROR_CODE.NONE;
            }
            catch
            {
                return ERROR_CODE.UPDATE_USER_INFO_ERROR;
            }
        }
    }
}
