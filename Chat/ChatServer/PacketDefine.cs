using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class PacketDefine
    {
        public const short PACKET_HEADER = 4;
    }

    public enum PACKET_ID : short
    {
        ECHO = 1,

        C2S_BEGIN = 1000,

        REQ_LOGIN = 1001,
        RES_LOGIN = 1002,

        REQ_ENTER_ROOM = 1011,
        RES_ENTER_ROOM = 1012,
        S2C_USER_LIST = 1013,
        S2C_NEW_USER = 1014,

        REQ_LEAVE_ROOM = 1021,
        RES_LEAVE_ROOM = 1022,
        S2C_LEAVE_USER = 1023,

        C2S_ROOM_CHAT = 1031,
        S2C_ROOM_CHAT = 1032,

        C2S_END = 1100,
    }

    public enum ERROR_CODE : short
    {
        NONE = 0,
    }
}
