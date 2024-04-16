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
        private MySqlConnection connection;
        private MySqlCompiler compiler;
        private QueryFactory queryFactory;

        public OmokDB()
        {
            connection = new MySqlConnection("Server=localhost;Port=3306;Database=omokDB;Uid=root;Pwd=1234");
            compiler = new MySqlCompiler();

            connection.Open();
            queryFactory = new QueryFactory(connection, compiler);
        }

        public void Dispose()
        {
            connection.Close();
        }

        public async Task<ErrorCode> CreateUserGameData(string email)
        {
            try
            {
                var count = await queryFactory.Query("usergamedata").InsertAsync(new
                {
                    Email = email,
                    Level = 1,
                    exp = 0,
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
