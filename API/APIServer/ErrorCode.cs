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
        SetGameServerTokenError =  1202,
        GetUserDataError = 1203,
        CheckTokenError = 1204,
        AttendanceError = 1205,
        AttendanceAlready = 1206,
        PostMailError = 1207,
        GetMailError = 1208,
    }
}
