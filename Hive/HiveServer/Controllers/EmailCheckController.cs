using HiveServer.DB;
using HiveServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HiveServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailCheckController : ControllerBase
    {
        private readonly ILogger<EmailCheckController> _logger;

        private readonly IAccountDB _accountDB;


        public EmailCheckController(ILogger<EmailCheckController> logger, IAccountDB accountDB)
        {
            this._logger = logger;
            this._accountDB = accountDB;
        }

        [HttpPost]
        [Route("check")]
        public async Task<ResCheckEmail> Check([FromBody] ReqCheckEmail model)
        {
            Tuple<ErrorCode, bool> result = await _accountDB.EmailCheck(model.Email);

            ResCheckEmail res = new ResCheckEmail
            {
                Result = _accountDB.EmailCheck(model.Email).Result.Item2
            };

            return res;
        }
    }
}
