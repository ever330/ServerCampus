namespace HiveServer.Repository
{
    public interface IAccountDB : IDisposable
    {
        public Task<ErrorCode> CreateAccount(string email, string password);

        public Task<ErrorCode> AccountLogin(string email, string password);
    }
}
