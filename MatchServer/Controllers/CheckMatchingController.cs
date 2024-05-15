using MatchServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MatchServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckMatchingController : ControllerBase
    {
        IMatchWorker _matchWorker;

        public CheckMatchingController(IMatchWorker matchWorker)
        {
            _matchWorker = matchWorker;
        }

        [HttpPost]
        [Route("checkMatching")]
        public ResCheckMatching CheckMatching([FromBody] ReqCheckMatching request)
        {
            var res = new ResCheckMatching();
            var comp =_matchWorker.GetCompleteMatchData(request.Id);

            if (comp == null)
            {
                res.MatchResult = ErrorCode.WaitMatching;
            }
            else
            {
                res.MatchResult = ErrorCode.None;
                res.ServerAddress = comp.ServerAddress;
                res.Port = comp.Port;
                res.RoomNumber = comp.RoomNumber;
                res.OtherUserId = comp.OtherUserId;
            }

            return res;
        }
    }
}
