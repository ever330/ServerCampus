using HiveServer.DB;
using HiveServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace HiveServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateAccountController : ControllerBase
    {
        private readonly ILogger<CreateAccountController> _logger;

        private readonly IAccountDB _accountDB;


        public CreateAccountController(ILogger<CreateAccountController> logger, IAccountDB accountDB)
        {
            this._logger = logger;
            this._accountDB = accountDB;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ResCreateAccount> Create([FromBody] ReqCreateAccount model)
        {
            var result = await _accountDB.CreateAccount(model.Email, model.Password);

            ResCreateAccount res = new ResCreateAccount
            {
                Result = result
            };

            _logger.ZLogDebug($"계정 생성 : {model.Email}");

            return res;
        }
    }
}
