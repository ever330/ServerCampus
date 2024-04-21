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
            var check = await _redisDB.CheckAuthToken(request.Email, request.AuthToken);
            var checkAttendanceAlready = await _gameDB.CheckAttendanceAlready(request.Email);

            ResDailyAttendance resLogin = new ResDailyAttendance
            {
                Result = check
            };

            if (check == ErrorCode.CheckTokenError)
            {
                _logger.ZLogError($"{request.Email} : 토큰 확인 실패");
                return resLogin;
            }

            if (checkAttendanceAlready == ErrorCode.AttendanceAlready)
            {
                resLogin.Result = checkAttendanceAlready;
                return resLogin;
            }

            var res = await _gameDB.DailyAttendance(request.Email);

            resLogin.Result = res.Item1;
            if (resLogin.Result == ErrorCode.AttendanceError)
            {
                _logger.ZLogError($"{request.Email} : 출석 체크 실패");
                return resLogin;
            }

            _logger.ZLogInformation($"{request.Email} : 출석 체크");
            resLogin.ConsecutiveAttendance = res.Item2;

            await PostAttendanceMail(request.Email, resLogin.ConsecutiveAttendance);

            return resLogin;
        }

        private async Task PostAttendanceMail(string email, int consecutiveAttendance)
        {
            string mailName = "출석 체크 보상";
            string mailContent = "출석 체크 보상입니다.";
            int reward = consecutiveAttendance * RewardMoney;

            var result = await _gameDB.PostToMailbox(email, mailName, mailContent, reward);

            if (result == ErrorCode.PostMailError)
            {
                _logger.ZLogError($"{email} : 우편 전송 에러");
            }
            _logger.ZLogError($"{email} : 우편 전송 성공");
        }
    }
}
