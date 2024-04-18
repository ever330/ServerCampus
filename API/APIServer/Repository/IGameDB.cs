using APIServer.Models;

namespace APIServer.Repository
{
    public interface IGameDB : IDisposable
    {
        public Task<ErrorCode> CreateUserGameData(string email);
        public Task<Tuple<ErrorCode, UserGameData?>> GetUserGameData(string email);
    }
}
