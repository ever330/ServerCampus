using HiveServer.Repository;
using HiveServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace HiveServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        private readonly IAccountDB _accountDB;
        private readonly IRedisDB _redisDB;


        public LoginController(ILogger<LoginController> logger, IAccountDB accountDB, IRedisDB redisDB)
        {
            this._logger = logger;
            this._accountDB = accountDB;
            this._redisDB = redisDB;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ResLoginToHive> Login([FromBody] ReqLoginToHive request)
        {
            var result = await _accountDB.AccountLogin(request.Email, request.Password);

            ResLoginToHive res = new ResLoginToHive
            {
                Result = result
            };

            if (res.Result == ErrorCode.None)
            {
                _logger.ZLogInformation($"로그인 성공 : {request.Email}");

                string authToken = Security.CreateAuthToken();
                res.AuthToken = authToken;

                res.Result = _redisDB.SetAuthToken(request.Email, authToken);
            }
            else
            {
                _logger.ZLogError($"로그인 실패 : {request.Email}, {res.Result}");

                res.AuthToken = "";
            }

            return res;
        }

        [HttpPost]
        [Route("logintest")]
        public async Task<ResLoginToHive> Login([FromBody] string Email, string Password)
        {
            var result = await _accountDB.AccountLogin(Email, Password);

            ResLoginToHive res = new ResLoginToHive
            {
                Result = result
            };

            if (res.Result == ErrorCode.None)
            {
                _logger.ZLogInformation($"로그인 성공 : {Email}");

                string authToken = Security.CreateAuthToken();
                res.AuthToken = authToken;

                res.Result = _redisDB.SetAuthToken(Email, authToken);
            }
            else
            {
                _logger.ZLogError($"로그인 실패 : {Email}, {res.Result}");

                res.AuthToken = "";
            }

            return res;
        }
    }
}
