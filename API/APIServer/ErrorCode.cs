namespace APIServer
{
    public enum ErrorCode
    {
        None = 0,

        CreateAccountError = 1000,
        LoginError = 1001,
        EmailCheckError = 1002,
        VerifyTokenError = 1003,

        UserDataNotExist = 1200,
        CreateUserDataError = 1201,
        GetUserDataError = 1202,
    }
}
