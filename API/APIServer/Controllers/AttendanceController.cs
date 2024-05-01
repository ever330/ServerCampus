using APIServer.Models;
using APIServer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly ILogger<AttendanceController> _logger;

        private readonly IGameDB _gameDB;
        private readonly IRedisDB _redisDB;

        private const int RewardMoney = 100;

        public AttendanceController(ILogger<AttendanceController> logger, IGameDB accountDB, IRedisDB redisDB)
        {
            this._logger = logger;
            this._gameDB = accountDB;
            this._redisDB = redisDB;
        }

        [HttpPost]
        [Route("attendance")]
        public async Task<ResDailyAttendance> Attendance([FromBody] ReqDailyAttendance request)
        {
            var check = await _redisDB.CheckAuthToken(request.Id, request.AuthToken);
            var checkAttendanceAlready = await _gameDB.CheckAttendanceAlready(request.Id);

            ResDailyAttendance resLogin = new ResDailyAttendance
            {
                Result = check
            };

            if (check == ERROR_CODE.CheckTokenError)
            {
                _logger.ZLogError($"{request.Id} : 토큰 확인 실패");
                return resLogin;
            }

            if (checkAttendanceAlready == ERROR_CODE.AttendanceAlready)
            {
                resLogin.Result = checkAttendanceAlready;
                return resLogin;
            }

            var res = await _gameDB.DailyAttendance(request.Id);

            resLogin.Result = res.Item1;
            if (resLogin.Result == ERROR_CODE.AttendanceError)
            {
                _logger.ZLogError($"{request.Id} : 출석 체크 실패");
                return resLogin;
            }

            _logger.ZLogInformation($"{request.Id} : 출석 체크");
            resLogin.ConsecutiveAttendance = res.Item2;

            await PostAttendanceMail(request.Id, resLogin.ConsecutiveAttendance);

            return resLogin;
        }

        private async Task PostAttendanceMail(string id, int consecutiveAttendance)
        {
            string mailName = "출석 체크 보상";
            string mailContent = "출석 체크 보상입니다.";
            int reward = consecutiveAttendance * RewardMoney;

            var result = await _gameDB.PostToMailbox(id, mailName, mailContent, reward);

            if (result == ERROR_CODE.PostMailError)
            {
                _logger.ZLogError($"{id} : 우편 전송 에러");
            }
            _logger.ZLogError($"{id} : 우편 전송 성공");
        }
    }
}
