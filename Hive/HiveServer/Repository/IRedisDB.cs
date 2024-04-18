namespace HiveServer.Repository
{
    public interface IRedisDB : IDisposable
    {
        public ErrorCode SetAuthToken(string email, string authToken);
        public Task<ErrorCode> VerifyToken(string email, string authToken);
    }
}
