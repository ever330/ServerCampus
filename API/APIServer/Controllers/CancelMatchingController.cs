using APIServer.Controllers;
using APIServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace MatchServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancelMatchingController : ControllerBase
    {
        private readonly ILogger<CancelMatchingController> _logger;

        public CancelMatchingController(ILogger<CancelMatchingController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("cancelMatching")]
        public async Task<ResCancelMatching> CancelMatching([FromBody] ReqCancelMatching request)
        {
            var client = new HttpClient();

            var response = await client.PostAsJsonAsync("http://10.192.8.223:5922/api/CheckMatching/checkMatching", request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.ZLogError($"{request.Id} : 매칭 취소 요청 실패");
            }

            ResCancelMatching? res = await response.Content.ReadFromJsonAsync<ResCancelMatching>();

            return res;
        }
    }
}
