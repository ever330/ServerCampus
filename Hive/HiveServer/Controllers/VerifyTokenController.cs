using HiveServer.DB;
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
        public async Task<ResVerifyAuthToken> Verify([FromBody] ReqVerifyAuthToken model)
        {
            bool result =  await _redisDB.VerifyToken(model.Email, model.AuthToken);

            ResVerifyAuthToken res = new ResVerifyAuthToken();
            if (result)
            {
                res.Result = ErrorCode.None;
            }
            else
            {
                res.Result = ErrorCode.VerifyTokenError;
            }

            return res;
        }
    }
}
