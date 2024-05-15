﻿using APIServer.Controllers;
using APIServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace MatchServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckMatchingController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        public CheckMatchingController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("checkMatching")]
        public async Task<ResCheckMatching> CheckMatching([FromBody] ReqCheckMatching request)
        {
            var client = new HttpClient();

            var response = await client.PostAsJsonAsync("http://10.192.8.223:5922/api/CheckMatching/checkMatching", request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.ZLogError($"{request.Id} : 매칭서버에 요청 실패");
            }

            ResCheckMatching? res = await response.Content.ReadFromJsonAsync<ResCheckMatching>();

            return res;
        }
    }
}
