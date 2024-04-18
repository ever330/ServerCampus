using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok
{
    public enum ErrorCode
    {
        None = 0,

        CreateAccountError = 1000,
        LoginError = 1001,
        EmailCheckError = 1002,
        VerifyTokenError = 1003,

        AccountNotExist = 1100,
        AccountAlreadyExist = 1101,
        SetTokenError = 1102,

        UserDataNotExist = 1200,
        CreateUserDataError = 1201,
        GetUserDataError = 1202,
    }
}
