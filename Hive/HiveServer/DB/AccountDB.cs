using Microsoft.AspNetCore.Hosting.Server;

namespace HiveServer.DB
{
    using HiveServer.Models;
    using Dapper;
    using MySqlConnector;
    using SqlKata;
    using SqlKata.Compilers;
    using SqlKata.Execution;
    using System.Reflection;

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

                var count = await _queryFactory.Query("hiveusers").InsertAsync(new
                {
                    Email = email,
                    Password = encryptPassword,
                    Salt = salt
                });

                if (count != 1)
                {
                    return ErrorCode.CreateAccountError;
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
            var userInfo = await _queryFactory.Query("hiveusers").Select().Where(new
            {
                Email = email,
                Password = password
            }).FirstOrDefaultAsync();

            if (userInfo == null)
            {
                return ErrorCode.AccountNOTExist;
            }

            return ErrorCode.None;
        }

        public async Task<Tuple<ErrorCode, bool>> EmailCheck(string email)
        {
            var userInfo = await _queryFactory.Query("hiveusers")
                                  .Select()
                                  .Where(new { Email = email })
                                  .FirstOrDefaultAsync();

            if (userInfo != null)
            {
                // 요소가 존재하는 경우의 처리
                return new Tuple<ErrorCode, bool>(ErrorCode.EmailCheckError, false);
            }
            else
            {
                // 요소가 존재하지 않는 경우의 처리
                return new Tuple<ErrorCode, bool>(ErrorCode.None, true);
            }
        }
    }
}
