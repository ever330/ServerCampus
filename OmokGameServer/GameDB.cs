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
                var userData = queryFactory.Query("userGameData").Select().Where("id", id).FirstOrDefaultAsync<UserGameData>();

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

        public ERROR_CODE UpdateGameResult(QueryFactory queryFactory, string id, int winCount, int loseCount)
        {
            try
            {
                var userData = queryFactory.Query("userGameData").Where("id", id).UpdateAsync(new
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
    }
}
