using APIServer.Models;
using APIServer.Models.DAO;

namespace APIServer.Repository
{
    public interface IGameDB : IDisposable
    {
        public Task<ERROR_CODE> CreateUserGameData(string id);
        public Task<Tuple<ERROR_CODE, UserGameData?>> GetUserGameData(string id);
        public Task<Tuple<ERROR_CODE, int>> DailyAttendance(string id);
        public Task<ERROR_CODE> CheckAttendanceAlready(string id);
        public Task<ERROR_CODE> PostToMailbox(string id, string mailName, string mailContent, int reward);
        public Task<Tuple<ERROR_CODE, List<Mail>?>> GetMailbox(string id);
    }
}
