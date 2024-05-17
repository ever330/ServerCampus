using MatchServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancelMatchingController : ControllerBase
    {
        IMatchWorker _matchWorker;

        public CancelMatchingController(IMatchWorker matchWorker)
        {
            _matchWorker = matchWorker;
        }

        [HttpPost]
        [Route("cancelMatching")]
        public ResCancelMatching CancelMatching([FromBody] ReqCancelMatching request)
        {
            var res = new ResCancelMatching();
            res.Result = _matchWorker.CancelMatching(request.Id);

            return res;
        }
    }
}
