namespace APIServer
{
    public enum ErrorCode
    {
        None = 0,

        CreateAccountError = 1000,
        LoginAccountError = 1001,
        EmailCheckError = 1002,

        AccountNOTExist = 1100,

        CreateGameDataError = 1200,
    }
}
