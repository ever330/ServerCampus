namespace APIServer.Repository
{
    using APIServer.Models;
    using APIServer.Models.DAO;
    using Dapper;
    using MySqlConnector;
    using SqlKata;
    using SqlKata.Compilers;
    using SqlKata.Execution;
    using System.Reflection;

    public class GameDB : IGameDB
    {
        private IConfiguration _config;
        private MySqlConnection _connection;
        private MySqlCompiler _compiler;
        private QueryFactory _queryFactory;

        public GameDB(IConfiguration config)
        {
            _config = config;

            _connection = new MySqlConnection(_config.GetConnectionString("GameDB"));
            _compiler = new MySqlCompiler();

            _connection.Open();
            _queryFactory = new QueryFactory(_connection, _compiler);
        }

        public void Dispose()
        {
            _connection.Close();
        }

        public async Task<ErrorCode> CreateUserGameData(string id)
        {
            try
            {
                var count = await _queryFactory.Query("userGameData").InsertAsync(new
                {
                    id = id,
                    level = 1,
                    exp = 0,
                    winCount = 0,
                    loseCount = 0
                });

                if (count != 1)
                {
                    return ErrorCode.CreateUserDataError;
                }

                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.CreateUserDataError;
            }
        }

        public async Task<Tuple<ErrorCode, UserGameData?>> GetUserGameData(string id)
        {
            try
            {
                var userData = await _queryFactory.Query("userGameData").Select().Where("id", id).FirstOrDefaultAsync<UserGameData>();

                if (userData == null || userData.Level == 0)
                {
                    return new Tuple<ErrorCode, UserGameData?>(ErrorCode.UserDataNotExist, userData);
                }
                return new Tuple<ErrorCode, UserGameData?>(ErrorCode.None, userData);
            }
            catch
            {
                return new Tuple<ErrorCode, UserGameData?>(ErrorCode.GetUserDataError, null);
            }
        }

        public async Task<Tuple<ErrorCode, int>> DailyAttendance(string id)
        {
            try
            {
                var subQuery = await _queryFactory.Query("userGameData").Select("uid").Where("id", id).FirstOrDefaultAsync();

                var data = await _queryFactory.Query("userDailyAttendance").Select().WhereIn("uid", new List<object> { subQuery.uid }).Where("attendanceDate", DateTime.Now.AddDays(-1)).FirstOrDefaultAsync<UserDailyAttendance>();

                UserDailyAttendance attendance = new UserDailyAttendance();
                attendance.AttendanceDate = DateTime.Today;

                if (data == null)
                {
                    attendance.ConsecutiveAttendance = 1;
                }
                else
                {
                    attendance.ConsecutiveAttendance = data.ConsecutiveAttendance + 1;
                }

                var count = await _queryFactory.Query("userDailyAttendance").InsertAsync(new
                {
                    uid = subQuery.uid,
                    attendance_date = attendance.AttendanceDate,
                    consecutive_attendance = attendance.ConsecutiveAttendance
                });

                if (count != 1)
                {
                    return new Tuple<ErrorCode, int>(ErrorCode.AttendanceError, 0);
                }

                return new Tuple<ErrorCode, int>(ErrorCode.None, attendance.ConsecutiveAttendance);
            }
            catch
            {
                return new Tuple<ErrorCode, int>(ErrorCode.AttendanceError, 0);
            }
        }

        public async Task<ErrorCode> CheckAttendanceAlready(string id)
        {
            try
            {
                var subQuery = await _queryFactory.Query("userGameData").Select("uid").Where("id", id).FirstOrDefaultAsync();

                var todayCheck = await _queryFactory.Query("userDailyAttendance").Select().WhereIn("uid", subQuery.uid).Where("attendanceDate", DateTime.Now).FirstOrDefaultAsync();

                if (todayCheck != 0)
                {
                    return ErrorCode.AttendanceAlready;
                }

                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.AttendanceError;
            }
        }

        public async Task<ErrorCode> PostToMailbox(string id, string mailName, string mailContent, int reward)
        {
            try
            {
                var subQuery = await _queryFactory.Query("userGameData").Select("uid").Where("id", id).FirstOrDefaultAsync();

                if (subQuery == null)
                {
                    return ErrorCode.PostMailError;
                }

                var count = await _queryFactory.Query("userMailbox").InsertAsync(new
                {
                    uid = subQuery.uid,
                    mail_name = mailName,
                    mail_content = mailContent,
                    reward = reward
                });

                if (count != 1)
                {
                    return ErrorCode.PostMailError;
                }

                return ErrorCode.None;
            }
            catch
            {
                return ErrorCode.PostMailError;
            }
        }

        public async Task<Tuple<ErrorCode, List<Mail>?>> GetMailbox(string id)
        {
            try
            {
                var subQuery = await _queryFactory.Query("userGameData").Select("uid").Where("id", id).FirstOrDefaultAsync();

                Query query = new Query("userMailbox").Where("uid", subQuery.uid);

                // 쿼리 실행
                IEnumerable<dynamic> results = await _queryFactory.FromQuery(query).GetAsync();

                // 결과를 Mail 객체로 변환
                List<Mail> mails = results.Select(r => new Mail
                {
                    MailName = r.mail_name,
                    MailContent = r.mail_content,
                    Reward = r.reward
                }).ToList();

                if (mails == null)
                {
                    return new Tuple<ErrorCode, List<Mail>?> (ErrorCode.GetMailError, null);
                }

                return new Tuple<ErrorCode, List<Mail>?> (ErrorCode.None, mails);
            }
            catch
            {
                return new Tuple<ErrorCode, List<Mail>?> (ErrorCode.GetMailError, null);
            }
        }
    }
}
