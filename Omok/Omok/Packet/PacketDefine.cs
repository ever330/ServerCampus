using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omok.Packet
{
    public class PacketDefine
    {
        public const short PacketHeader = 4;

        public const int PacketBufferSize = 2048;
    }

    public enum PacketId : short
    {
        SessionConnect = 1001,
        SessionDisconnect = 1002,

        ReqLogin = 1101,
        ResLogin = 1102,

        ReqEnterRoom = 1201,
        ResEnterRoom = 1202,
        ReqLeaveRoom = 1203,
        ResLeaveRoom = 1204,
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
    }
}
