namespace APIServer.DB
{
    public interface IOmokDB : IDisposable
    {
        public Task<ErrorCode> CreateUserGameData(string email);
    }
}
