namespace HiveServer.DB
{
    public interface IRedisDB : IDisposable
    {
        public void SetAuthToken(string email, string authToken);
        public Task<bool> VerifyToken(string email, string authToken);
    }
}
