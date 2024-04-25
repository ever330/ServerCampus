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

        USER_COUNT_MAX = 1001,
        USER_NOT_EXIST = 1002,
    }
}
