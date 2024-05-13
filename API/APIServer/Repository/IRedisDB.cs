namespace APIServer.Repository
{
    public interface IRedisDB : IDisposable
    {
        public Task<ERROR_CODE> SetAuthToken(string id, string authToken);
        public Task<ERROR_CODE> CheckAuthToken(string id, string authToken);
    }
}
