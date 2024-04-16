using HiveServer.DB;
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
        public async Task<ResLoginToHive> Login([FromBody] ReqLoginToHive model)
        {
            var result = await _accountDB.AccountLogin(model.Email, model.Password);

            ResLoginToHive res = new ResLoginToHive
            {
                Result = result
            };

            if (res.Result == ErrorCode.None)
            {
                _logger.ZLogDebug($"로그인 성공 : {model.Email}");
                string authToken = Security.CreateAuthToken();
                res.AuthToken = authToken;
                _redisDB.SetAuthToken(model.Email, authToken);
            }
            else
            {
                _logger.ZLogDebug($"로그인 실패 : {model.Email}");
                res.AuthToken = "";
            }

            return res;
        }
    }
}
