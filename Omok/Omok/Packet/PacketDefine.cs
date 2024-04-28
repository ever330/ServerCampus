using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok.Packet
{
    public class PacketDefine
    {
        public const short PACKET_HEADER = 4;

        public const int PACKET_BUFFER_SIZE = 2048;
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
