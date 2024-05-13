using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class PacketDefine
    {
        public const short PacketHeader = 4;
    }

    public enum ErrorCode : short
    {
        None = 0,

        // DB 관련
        SetGameServerTokenError = 1000,
        CheckTokenError = 1001,
        UpdateUserInfoError = 1002,
        UserDataNotExist = 1003,
        GetUserDataError = 1004,

        // 게임 서버 접속 관련
        UserCountMax = 3001,
        UserNotExist = 3002,
        UserAlreadyExist = 3003,

        // 방 관련
        RoomUserMax = 3101,
        RoomLeaveError = 3202,
    }

    public enum PacketId : short
    {
        SessionConnect = 1001,
        SessionDisconnect = 1002,

        ReqLogin = 1101,
        ResLogin = 1102,

        ReqEnterRoom = 1201,
        ResEnterRoom = 1202,
        ResLeaveRoom = 1203,
        ReqLeaveRoom = 1204,
        ReqRoomChat = 1205,
        NtfRoomChat = 1206,
        NtfNewUser = 1207,
        NtfLeaveUser = 1208,
        NtfRoomUsers = 1209,
        ReqReady = 1210,
        ResReady = 1211,
        ReqNotReady = 1212,
        ResNotReady = 1213,
        NtfReadyState = 1214,
        NtfGameStart = 1215,
        ReqPutStone = 1216,
        ResPutStone = 1217,
        NtfPutStone = 1218,
        NtfWinGame = 1219,
        ReqTimeOut = 1220,
        NtfTimeOut = 1221,
        NtfTimeOutWin = 1222,
        NtfDraw = 1223,

        ReqHeartBeat = 1300,
        ResHeartBeat = 1301,
        NtfSessionTimeLimit = 1302,

        // MYSQL
        ReqUpdateResult = 2000,
        ReqUserData = 2001,
        ResUserData = 2002,

        // REDIS
        ReqCheckAuthToken = 2101,
        ResCheckAuthToken = 2102,

        // INNER
        ReqCheckHeartBeat = 3000,
        ReqCheckRoom = 3001,
        ReqCheckSession = 3002,
    }
}
