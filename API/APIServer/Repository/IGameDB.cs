using APIServer.Models;
using APIServer.Models.DAO;

namespace APIServer.Repository
{
    public interface IGameDB : IDisposable
    {
        public Task<ErrorCode> CreateUserGameData(string id);
        public Task<Tuple<ErrorCode, UserGameData?>> GetUserGameData(string id);
        public Task<Tuple<ErrorCode, int>> DailyAttendance(string id);
        public Task<ErrorCode> CheckAttendanceAlready(string id);
        public Task<ErrorCode> PostToMailbox(string id, string mailName, string mailContent, int reward);
        public Task<Tuple<ErrorCode, List<Mail>?>> GetMailbox(string id);
    }
}
