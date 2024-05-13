namespace APIServer.Repository
{
    public interface IRedisDB : IDisposable
    {
        public Task<ERROR_CODE> SetAuthToken(string email, string authToken);
        public Task<ERROR_CODE> CheckAuthToken(string email, string authToken);
    }
}
