﻿using APIServer.Repository;
using APIServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;
using System.Reflection;

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
        public async Task<ResLoginToGame> Login([FromBody] ReqLoginToAPI request)
        {
            var res = await TryVerifyToken(request.Email, request.AuthToken);

            ResLoginToGame resLogin = new ResLoginToGame
            {
                Result = res
            };

            if (res == ErrorCode.VerifyTokenError)
            {
                _logger.ZLogError($"{request.Email} : 토큰 검증 실패");
                return resLogin;
            }

            _logger.ZLogInformation($"{request.Email} : 토큰 유효");
            _redisDB.SetAuthToken(request.Email, request.AuthToken);

            var checkRes = await CheckUserData(request.Email);

            resLogin.Result = checkRes.Item1;
            resLogin.GameData = checkRes.Item2;

            if (resLogin.Result != ErrorCode.None)
            {
                _logger.ZLogError($"{request.Email} : 유저 데이터 생성 실패");
            }

            return resLogin;
        }

        private async Task<ErrorCode> TryVerifyToken(string email, string token)
        {
            var request = new ReqVerifyAuthToken
            {
                Email = email,
                AuthToken = token,
            };

            var client = new HttpClient();

            var response = await client.PostAsJsonAsync("http://localhost:5229/api/VerifyToken/verify", request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.ZLogInformation($"토큰 검증 시도 실패");
                return ErrorCode.VerifyTokenError;
            }

            ResVerifyAuthToken? res = await response.Content.ReadFromJsonAsync<ResVerifyAuthToken>();

            if (res == null)
            {
                _logger.ZLogInformation($"토큰 검증 시도 실패");
                return ErrorCode.VerifyTokenError;
            }

            return res.Result;
        }

        private async Task<Tuple<ErrorCode, UserGameData?>> CheckUserData(string email)
        {
            var getRes = await _gameDB.GetUserGameData(email);

            if (getRes.Item1 == ErrorCode.UserDataNotExist)
            {
                var createRes = await _gameDB.CreateUserGameData(email);

                if (createRes == ErrorCode.CreateUserDataError)
                {
                    return new Tuple<ErrorCode, UserGameData?>(ErrorCode.CreateUserDataError, null);
                }
                
                UserGameData userData = new UserGameData
                {
                    Level = 1,
                    Exp = 0,
                    WinCount = 0,
                    LoseCount = 0
                };

                return new Tuple<ErrorCode, UserGameData?>(ErrorCode.None, userData);
            }

            return getRes;
        }
    }
}
