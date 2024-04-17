using APIServer.Models;

namespace APIServer.DB
{
    public interface IOmokDB : IDisposable
    {
        public Task<ErrorCode> CreateUserGameData(string email);
        public Task<Tuple<ErrorCode, UserGameData>> GetUserGameData(string email);
    }
}
