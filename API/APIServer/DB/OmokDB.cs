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
        private MySqlConnection _connection;
        private MySqlCompiler _compiler;
        private QueryFactory _queryFactory;

        public OmokDB()
        {
            _connection = new MySqlConnection("Server=localhost;Port=3306;Database=omokDB;Uid=root;Pwd=1234");
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
    }
}
