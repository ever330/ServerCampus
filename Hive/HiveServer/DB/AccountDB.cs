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
        private MySqlConnection connection;
        private MySqlCompiler compiler;
        private QueryFactory queryFactory;

        public AccountDB()
        {
            connection = new MySqlConnection("Server=localhost;Port=3306;Database=accountDB;Uid=root;Pwd=1234");
            compiler = new MySqlCompiler();

            connection.Open();
            queryFactory = new QueryFactory(connection, compiler);
        }

        public void Dispose()
        {
            connection.Close();
        }

        public async Task<ErrorCode> CreateAccount(string email, string password)
        {
            try
            {
                var count = await queryFactory.Query("hiveusers").InsertAsync(new
                {
                    Email = email,
                    Password = password
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
            try
            {
                var userInfo = await queryFactory.Query("hiveusers").Select().Where(new
                {
                    Email = email,
                    Password = password
                }).FirstAsync<ReqCreateAccount>();

                if (userInfo == null)
                {
                    return ErrorCode.AccountNOTExist;
                }

                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.LoginAccountError;
            }
        }

        public async Task<Tuple<ErrorCode, bool>> EmailCheck(string email)
        {
            try
            {
                var userInfo = await queryFactory.Query("hiveusers").Select().Where(new
                {
                    Email = email
                }).FirstAsync<ReqCreateAccount>();

                if (userInfo != null)
                {
                    return new Tuple<ErrorCode, bool>(ErrorCode.EmailCheckError, false);
                }

                return new Tuple<ErrorCode, bool>(ErrorCode.None, true);
            }
            catch
            {
                return new Tuple<ErrorCode, bool>(ErrorCode.EmailCheckError, false);
            }
        }
    }
}
