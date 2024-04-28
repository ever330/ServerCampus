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
    public partial class NtfUserEnterRoomPacket
    {
        public int RoomNumber { get; set; }
    }

    [MemoryPackable]
    public partial class NtfUserLeaveRoomPacket
    {
        public int RoomNumber { get; set; }
    }
}
