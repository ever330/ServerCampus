namespace HiveServer.DB
{
    public interface IAccountDB : IDisposable
    {
        public Task<ErrorCode> CreateAccount(string email, string password);

        public Task<ErrorCode> AccountLogin(string email, string password);

        public Task<Tuple<ErrorCode, bool>> EmailCheck(string email);
    }
}
