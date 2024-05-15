using APIServer;
using APIServer.Controllers;
using APIServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace MatchServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestMatchingController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        public RequestMatchingController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("matching")]
        public async Task<ResMatching> Matching([FromBody] ReqMatching request)
        {
            var client = new HttpClient();

            var response = await client.PostAsJsonAsync("http://10.192.8.223:5922/api/RequestMatching/matching", request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.ZLogError($"{request.Id} : 매칭서버에 요청 실패");
            }

            ResMatching? res = await response.Content.ReadFromJsonAsync<ResMatching>();

            return res;
        }
    }
}
