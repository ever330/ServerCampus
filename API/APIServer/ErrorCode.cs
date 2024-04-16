﻿namespace APIServer
{
    public enum ErrorCode
    {
        None = 0,

        CreateAccountError = 1000,
        LoginError = 1001,
        EmailCheckError = 1002,
        VerifyTokenError = 1003,

        AccountNOTExist = 1100,

        CreateGameDataError = 1200,
    }
}
