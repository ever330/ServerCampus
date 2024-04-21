using APIServer.Models;
using APIServer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetMailController : ControllerBase
    {
        private readonly ILogger<GetMailController> _logger;

        private readonly IGameDB _gameDB;
        private readonly IRedisDB _redisDB;

        private const int RewardMoney = 100;

        public GetMailController(ILogger<GetMailController> logger, IGameDB accountDB, IRedisDB redisDB)
        {
            this._logger = logger;
            this._gameDB = accountDB;
            this._redisDB = redisDB;
        }


        [HttpPost]
        [Route("getMail")]
        public async Task<ResGetMail> GetMail([FromBody] ReqGetMail request)
        {
            var check = await _redisDB.CheckAuthToken(request.Email, request.AuthToken);

            ResGetMail resLogin = new ResGetMail
            {
                Result = check
            };

            if (check == ErrorCode.CheckTokenError)
            {
                _logger.ZLogError($"{request.Email} : 토큰 확인 실패");
                return resLogin;
            }

            var mail = await _gameDB.GetMailbox(request.Email);
            resLogin.Result = mail.Item1;

            if (mail.Item1 == ErrorCode.GetMailError || mail.Item2 == null)
            {
                _logger.ZLogError($"{request.Email} : 메일 가져오기 에러");
                return resLogin;
            }

            resLogin.Mails = mail.Item2;

            return resLogin;
        }
    }
}
