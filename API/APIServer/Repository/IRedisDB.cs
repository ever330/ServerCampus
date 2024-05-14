namespace APIServer.Repository
{
    public interface IRedisDB : IDisposable
    {
        public Task<ErrorCode> SetAuthToken(string email, string authToken);
        public Task<ErrorCode> CheckAuthToken(string email, string authToken);
    }
}
