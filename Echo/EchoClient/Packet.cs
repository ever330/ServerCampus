using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoClient
{
    struct PacketData
    {
        public Int16 DataSize;
        public Int16 PacketID;
        public byte[] Body;
    }

    [MemoryPackable]
    public partial class EchoPacket
    {
        public string Message { get; set; }
    }
}
