namespace APIServer.Repository
{
    public interface IRedisDB : IDisposable
    {
        public ErrorCode SetAuthToken(string id, string authToken);
        public Task<ErrorCode> CheckAuthToken(string id, string authToken);
    }
}
