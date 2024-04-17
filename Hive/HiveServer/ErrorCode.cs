namespace HiveServer
{
    public enum ErrorCode
    {
        None = 0,

        CreateAccountError = 1000,
        LoginError = 1001,
        EmailCheckError = 1002,
        VerifyTokenError = 1003,

        AccountNotExist = 1100,
        UserDataNotExist = 1101,

        CreateGameDataError = 1200,
    }
}
