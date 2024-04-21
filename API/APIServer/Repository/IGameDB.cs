using APIServer.Models;
using APIServer.Models.DAO;

namespace APIServer.Repository
{
    public interface IGameDB : IDisposable
    {
        public Task<ErrorCode> CreateUserGameData(string email);
        public Task<Tuple<ErrorCode, UserGameData?>> GetUserGameData(string email);
        public Task<Tuple<ErrorCode, int>> DailyAttendance(string email);
        public Task<ErrorCode> CheckAttendanceAlready(string email);
        public Task<ErrorCode> PostToMailbox(string email, string mailName, string mailContent, int reward);
        public Task<Tuple<ErrorCode, List<Mail>?>> GetMailbox(string email);
    }
}
