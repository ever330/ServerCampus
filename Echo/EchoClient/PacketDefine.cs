using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoClient
{
    public class PacketDefine
    {
        public const short PACKET_HEADER = 4;
    }

    public enum PACKET_ID : short
    {
        PACKET_ID_ECHO = 1,
    }

    public enum ERROR_CODE : short
    {
        ERROR_CODE_NONE = 0,
    }
}
