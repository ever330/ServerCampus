using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    [MemoryPackable]
    public partial class ClientPacket
    {
        public short PacketSize { get; set; }
        public short PacketId { get; set; }
    }
}
