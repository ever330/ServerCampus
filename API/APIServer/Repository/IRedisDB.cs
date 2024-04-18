namespace APIServer.Repository
{
    public interface IRedisDB : IDisposable
    {
        public void SetAuthToken(string email, string authToken);
    }
}
