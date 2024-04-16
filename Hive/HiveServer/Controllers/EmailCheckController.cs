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
        private readonly ILogger<EmailCheckController> logger;

        private readonly IAccountDB accountDB;


        public EmailCheckController(ILogger<EmailCheckController> logger, IAccountDB accountDB)
        {
            this.logger = logger;
            this.accountDB = accountDB;
        }

        [HttpPost]
        [Route("check")]
        public IActionResult Check([FromBody] ReqCheckEmail model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Tuple<ErrorCode, bool> result = accountDB.EmailCheck(model.Email).Result;

            if (result.Item1 == ErrorCode.EmailCheckError)
                return BadRequest();

            ResCheckEmail res = new ResCheckEmail
            {
                Result = accountDB.EmailCheck(model.Email).Result.Item2
            };


            return Ok(res);
        }
    }
}
