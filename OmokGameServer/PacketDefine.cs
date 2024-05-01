using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class PacketDefine
    {
        public const short PACKET_HEADER = 4;
    }

    public enum ERROR_CODE : short
    {
        NONE = 0,

        // DB 관련
        SET_GAME_SERVER_TOKEN_ERROR = 1000,
        CHECK_TOKEN_ERROR = 1001,
        UPDATE_USER_INFO_ERROR = 1002,
        USER_DATA_NOT_EXIST = 1003,
        GET_USER_DATA_ERROR = 1004,

        // 게임 서버 접속 관련
        USER_COUNT_MAX = 3001,
        USER_NOT_EXIST = 3002,
        USER_ALREADY_EXIST = 3003,

        // 방 관련
        ROOM_USER_MAX = 3101,
        ROOM_LEAVE_ERROR = 3202,
    }

    public enum PACKET_ID : short
    {
        SESSION_CONNECT = 1001,
        SESSION_DISCONNECT = 1002,

        REQ_LOGIN = 1101,
        RES_LOGIN = 1102,

        REQ_ENTER_ROOM = 1201,
        RES_ENTER_ROOM = 1202,
        REQ_LEAVE_ROOM = 1203,
        RES_LEAVE_ROOM = 1204,
        REQ_ROOM_CHAT = 1205,
        NTF_ROOM_CHAT = 1206,
        NTF_NEW_USER = 1207,
        NTF_LEAVE_USER = 1208,
        NTF_ROOM_USERS = 1209,
        REQ_READY = 1210,
        RES_READY = 1211,
        REQ_NOT_READY = 1212,
        RES_NOT_READY = 1213,
        NTF_READY_STATE = 1214,
        NTF_GAME_START = 1215,
        REQ_PUT_STONE = 1216,
        RES_PUT_STONE = 1217,
        NTF_PUT_STONE = 1218,
        NTF_WIN_GAME = 1219,
        REQ_TIME_OUT = 1220,
        NTF_TIME_OUT = 1221,
        NTF_TIME_OUT_WIN = 1222,

        NTF_HEART_BEAT = 1300,
        RES_HEART_BEAT = 1301,
    }
}
