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
        SetGameServerTokenError = 1202,
        GetUserDataError = 1203,
        CheckTokenError = 1204,
        AttendanceError = 1205,
        AttendanceAlready = 1206,
        PostMailError = 1207,
        GetMailError = 1208,


        // 게임 서버 접속 관련
        USER_COUNT_MAX = 3001,
        USER_NOT_EXIST = 3002,
        USER_LOGIN_ERROR = 3003,

        // 방 관련
        ROOM_USER_MAX = 3101,
        ROOM_LEAVE_ERROR = 3202,
    }
}
