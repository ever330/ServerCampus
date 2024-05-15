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
    public partial class ReqCheckHeartBeatPacket
    {
        public int CurrentIndex { get; set; }
    }

    [MemoryPackable]
    public partial class ReqCheckRoomPacket
    {
        public int CurrentIndex { get; set; }
    }

    [MemoryPackable]
    public partial class ReqCheckSessionPacket
    {
        public int CurrentIndex { get; set; }
    }

    [MemoryPackable]
    public partial class ReqMatchUsersEnterPacket
    {
        public string UserA { get; set; }
        public string UserB { get; set; }
        public int RoomNumber { get; set; }
    }
}
