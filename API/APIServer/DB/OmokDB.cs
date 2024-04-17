namespace APIServer.DB
{
    using APIServer.Models;
    using Dapper;
    using MySqlConnector;
    using SqlKata;
    using SqlKata.Compilers;
    using SqlKata.Execution;
    using System.Reflection;

    public class OmokDB : IOmokDB
    {
        private IConfiguration _config;
        private MySqlConnection _connection;
        private MySqlCompiler _compiler;
        private QueryFactory _queryFactory;

        public OmokDB(IConfiguration config)
        {
            _config = config;

            _connection = new MySqlConnection(_config.GetConnectionString("OmokDB"));
            _compiler = new MySqlCompiler();

            _connection.Open();
            _queryFactory = new QueryFactory(_connection, _compiler);
        }

        public void Dispose()
        {
            _connection.Close();
        }

        public async Task<ErrorCode> CreateUserGameData(string email)
        {
            try
            {
                var count = await _queryFactory.Query("usergamedata").InsertAsync(new
                {
                    Email = email,
                    Level = 1,
                    Exp = 0,
                    WinCount = 0,
                    LoseCount = 0
                }) ;

                if (count != 1)
                {
                    return ErrorCode.CreateGameDataError;
                }

                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.CreateGameDataError;
            }
        }

        public async Task<Tuple<ErrorCode, UserGameData>> GetUserGameData(string email)
        {
            var userInfo = await _queryFactory.Query("usergamedata").Select().Where(new
            {
                Email = email
            }).FirstOrDefaultAsync();

            UserGameData userData = new UserGameData
            {
                Email = email,
                Level = userInfo.Level,
                Exp = userInfo.Exp,
                WinCount = userInfo.WinCount,
                LoseCount = userInfo.LoseCount
            };

            if (userInfo == null || userInfo.Level == 0)
            {
                return new Tuple<ErrorCode, UserGameData>(ErrorCode.UserDataNotExist, userInfo);
            }

            return new Tuple<ErrorCode, UserGameData>(ErrorCode.None, userData);
        }
    }
}
