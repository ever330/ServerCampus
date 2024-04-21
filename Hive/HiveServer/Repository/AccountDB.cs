using Microsoft.AspNetCore.Hosting.Server;

namespace HiveServer.Repository
{
    using Dapper;
    using MySqlConnector;
    using SqlKata;
    using SqlKata.Compilers;
    using SqlKata.Execution;
    using System.Reflection;
    using HiveServer.Models.DAO;

    public class AccountDB : IAccountDB
    {
        private IConfiguration _config;
        private MySqlConnection _connection;
        private MySqlCompiler _compiler;
        private QueryFactory _queryFactory;

        public AccountDB(IConfiguration config)
        {
            _config = config;

            _connection = new MySqlConnection(_config.GetConnectionString("AccountDB"));
            _compiler = new MySqlCompiler();

            _connection.Open();
            _queryFactory = new QueryFactory(_connection, _compiler);
        }

        public void Dispose()
        {
            _connection.Close();
        }

        public async Task<ErrorCode> CreateAccount(string email, string password)
        {
            try
            {
                string salt = Security.GetRandomSalt();
                string encryptPassword = Security.Hasing(password, salt);

                var count = await _queryFactory.Query("hive_users").InsertAsync(new
                {
                    email = email,
                    password = encryptPassword,
                    salt = salt
                });

                if (count != 1)
                {
                    return ErrorCode.AccountAlreadyExist;
                }

                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.CreateAccountError;
            }
        }

        public async Task<ErrorCode> AccountLogin(string email, string password)
        {
            var userInfo = await _queryFactory.Query("hive_users").Select().Where(new
            {
                Email = email
            }).FirstOrDefaultAsync<AccountInfo>();

            if (userInfo == null)
            {
                return ErrorCode.AccountNotExist;
            }

            string encryptPassword = Security.Hasing(password, userInfo.Salt);

            if (encryptPassword != userInfo.Password)
            {
                return ErrorCode.LoginError;
            }

            return ErrorCode.None;
        }
    }
}
