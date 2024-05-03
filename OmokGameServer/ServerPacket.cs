using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;

namespace OmokGameServer
{
    [MemoryPackable]
    public partial class NtfSessionConnectPacket
    {
    }

    [MemoryPackable]
    public partial class NtfSessionDisConnectPacket
    {
    }

    [MemoryPackable]
    public partial class ReqSendHeartBeatPacket
    {
        public int CurrentIndex { get; set; }
    }

    [MemoryPackable]
    public partial class ReqSendCheckRoomPacket
    {
        public int CurrentIndex { get; set; }
    }
}
