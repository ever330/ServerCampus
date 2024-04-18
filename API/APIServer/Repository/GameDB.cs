namespace APIServer.Repository
{
    using APIServer.Models;
    using Dapper;
    using MySqlConnector;
    using SqlKata;
    using SqlKata.Compilers;
    using SqlKata.Execution;
    using System.Reflection;

    public class GameDB : IGameDB
    {
        private IConfiguration _config;
        private MySqlConnection _connection;
        private MySqlCompiler _compiler;
        private QueryFactory _queryFactory;

        public GameDB(IConfiguration config)
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
                    return ErrorCode.CreateUserDataError;
                }

                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.CreateUserDataError;
            }
        }

        public async Task<Tuple<ErrorCode, UserGameData?>> GetUserGameData(string email)
        {
            try
            {
                var userData = await _queryFactory.Query("usergamedata").Select().Where(new
                {
                    Email = email
                }).FirstOrDefaultAsync<UserGameData>();

                if (userData == null || userData.Level == 0)
                {
                    return new Tuple<ErrorCode, UserGameData?>(ErrorCode.UserDataNotExist, userData);
                }
                return new Tuple<ErrorCode, UserGameData?>(ErrorCode.None, userData);
            }
            catch
            {
                return new Tuple<ErrorCode, UserGameData?>(ErrorCode.GetUserDataError, null);
            }
        }
    }
}
