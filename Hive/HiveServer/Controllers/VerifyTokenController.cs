using HiveServer.Repository;
using HiveServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace HiveServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerifyTokenController : ControllerBase
    {
        private readonly ILogger<VerifyTokenController> _logger;

        private readonly IRedisDB _redisDB;


        public VerifyTokenController(ILogger<VerifyTokenController> logger, IAccountDB accountDB, IRedisDB redisDB)
        {
            this._logger = logger;
            this._redisDB = redisDB;
        }

        [HttpPost]
        [Route("verify")]
        public async Task<ResVerifyAuthToken> Verify([FromBody] ReqVerifyAuthToken request)
        {
            ErrorCode result =  await _redisDB.VerifyToken(request.Email, request.AuthToken);

            ResVerifyAuthToken res = new ResVerifyAuthToken
            {
                Result = result
            };

            if (result != ErrorCode.None)
            {
                _logger.ZLogError($"{0} : 토큰 검증 에러 발생", request.Email);
            }

            return res;
        }
    }
}
