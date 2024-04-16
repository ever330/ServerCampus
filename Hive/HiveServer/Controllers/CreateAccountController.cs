using HiveServer.DB;
using HiveServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HiveServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateAccountController : ControllerBase
    {
        private readonly ILogger<CreateAccountController> logger;

        private readonly IAccountDB accountDB;


        public CreateAccountController(ILogger<CreateAccountController> logger, IAccountDB accountDB)
        {
            this.logger = logger;
            this.accountDB = accountDB;
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create([FromBody] ReqCreateAccount model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (accountDB.CreateAccount(model.Email, model.Password).Result == ErrorCode.CreateAccountError)
            {
                return BadRequest();
            }

            ResCreateAccount res = new ResCreateAccount
            {
                Result = true
            };

            return Ok(res);
        }
    }
}
