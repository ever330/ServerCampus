using APIServer.DB;
using APIServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        private readonly IOmokDB _omokDB;
        private readonly IRedisDB _redisDB;


        public LoginController(ILogger<LoginController> logger, IOmokDB accountDB, IRedisDB redisDB)
        {
            this._logger = logger;
            this._omokDB = accountDB;
            this._redisDB = redisDB;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ResLoginToAPI> Login([FromBody] ReqLoginToAPI model)
        {
            var client = new HttpClient();

            var req = new ReqVerifyAuthToken
            {
                Email = model.Email,
                AuthToken = model.AuthToken,
            };

            var response = await client.PostAsJsonAsync("https://localhost:44349/api/VerifyToken/verify", model);

            if (!response.IsSuccessStatusCode)
            {
                _logger.ZLogInformation($"토큰 검증 시도 실패");
            }

            ResVerifyAuthToken res = await response.Content.ReadFromJsonAsync<ResVerifyAuthToken>();

            ResLoginToAPI resLogin = new ResLoginToAPI
            {
                Result = res.Result
            };


            if (res.Result == ErrorCode.None)
            {
                _logger.ZLogInformation($"{model.Email} : 토큰 유효");
                _redisDB.SetAuthToken(model.Email, model.AuthToken);
                Tuple<ErrorCode, UserGameData> getUserData = await _omokDB.GetUserGameData(model.Email);

                if (getUserData.Item1 == ErrorCode.UserDataNotExist)
                {
                    await _omokDB.CreateUserGameData(model.Email);
                    resLogin.GameData = new UserGameData
                    {
                        Level = 1,
                        Exp = 0,
                        WinCount = 0,
                        LoseCount = 0
                    };
                }
                else
                {
                    resLogin.GameData = new UserGameData
                    {
                        Level = getUserData.Item2.Level,
                        Exp = getUserData.Item2.Exp,
                        WinCount = getUserData.Item2.WinCount,
                        LoseCount = getUserData.Item2.LoseCount
                    };
                }
            }
            else
            {
                _logger.ZLogInformation($"{model.Email} : 토큰이 잘못되었습니다.");
            }

            return resLogin;
        }
    }
}
