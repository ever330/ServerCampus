using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoClient
{
    public class EchoPacket
    {
        public short PacketSize;
        public short PacketId;
        public byte[] Body;
    }
}
