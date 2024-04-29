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

        // 게임 서버 접속 관련
        USER_COUNT_MAX = 3001,
        USER_NOT_EXIST = 3002,
        USER_LOGIN_ERROR = 3003,

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
    }
}
