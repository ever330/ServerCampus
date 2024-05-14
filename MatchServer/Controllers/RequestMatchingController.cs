using MatchServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestMatchingController : ControllerBase
    {
        IMatchWorker _matchWorker;

        public RequestMatchingController(IMatchWorker matchWorker)
        {
            _matchWorker = matchWorker;
        }

        [HttpPost]
        [Route("matching")]
        public ResMatching Matching([FromBody] ReqMatching request)
        {
            var res = new ResMatching();
            _matchWorker.AddUser(request.Id);

            return res;
        }
    }
}
