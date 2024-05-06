using APIServer.Repository;
using APIServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;
using System.Reflection;
using APIServer.Models.DAO;

namespace APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        private readonly IGameDB _gameDB;
        private readonly IRedisDB _redisDB;

        public LoginController(ILogger<LoginController> logger, IGameDB accountDB, IRedisDB redisDB)
        {
            this._logger = logger;
            this._gameDB = accountDB;
            this._redisDB = redisDB;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ResLoginToGame> Login([FromBody] ReqLoginToGame request)
        {
            var res = await TryVerifyToken(request.Id, request.AuthToken);

            ResLoginToGame resLogin = new ResLoginToGame
            {
                Result = res
            };

            if (res == ERROR_CODE.VerifyTokenError)
            {
                _logger.ZLogError($"{request.Id} : 토큰 검증 실패");
                return resLogin;
            }

            _logger.ZLogInformation($"{request.Id} : 토큰 유효");
            _redisDB.SetAuthToken(request.Id, request.AuthToken);

            var checkRes = await CheckUserData(request.Id);

            if (resLogin.Result != ERROR_CODE.None || checkRes.Item2 == null)
            {
                _logger.ZLogError($"{request.Id} : 유저 데이터 생성 실패");
                return resLogin;
            }

            resLogin.Result = checkRes.Item1;
            resLogin.Level = checkRes.Item2.Level;
            resLogin.Exp = checkRes.Item2.Exp;
            resLogin.WinCount = checkRes.Item2.WinCount;
            resLogin.LoseCount = checkRes.Item2.LoseCount;
            resLogin.Money = checkRes.Item2.Money;
            resLogin.Ip = "127.0.0.1";
            resLogin.Port = 3030;

            return resLogin;
        }

        private async Task<ERROR_CODE> TryVerifyToken(string id, string token)
        {
            var request = new ReqVerifyAuthToken
            {
                Email = id,
                AuthToken = token,
            };

            var client = new HttpClient();

            var response = await client.PostAsJsonAsync("http://localhost:5229/api/VerifyToken/verify", request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.ZLogInformation($"토큰 검증 시도 실패");
                return ERROR_CODE.VerifyTokenError;
            }

            ResVerifyAuthToken? res = await response.Content.ReadFromJsonAsync<ResVerifyAuthToken>();

            if (res == null)
            {
                _logger.ZLogInformation($"토큰 검증 시도 실패");
                return ERROR_CODE.VerifyTokenError;
            }

            return res.Result;
        }

        private async Task<Tuple<ERROR_CODE, UserGameData?>> CheckUserData(string email)
        {
            var getRes = await _gameDB.GetUserGameData(email);

            if (getRes.Item1 == ERROR_CODE.UserDataNotExist)
            {
                var createRes = await _gameDB.CreateUserGameData(email);

                if (createRes == ERROR_CODE.CreateUserDataError)
                {
                    return new Tuple<ERROR_CODE, UserGameData?>(ERROR_CODE.CreateUserDataError, null);
                }
                
                UserGameData userData = new UserGameData
                {
                    Level = 1,
                    Exp = 0,
                    WinCount = 0,
                    LoseCount = 0
                };

                return new Tuple<ERROR_CODE, UserGameData?>(ERROR_CODE.None, userData);
            }

            return getRes;
        }
    }
}
