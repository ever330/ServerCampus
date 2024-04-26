using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;

namespace OmokGameServer
{
    [MemoryPackable]
    public partial class ServerPacket
    {
        public short PacketSize { get; set; }
        public short PacketId { get; set; }
    }

    [MemoryPackable]
    public partial class NtfSessionConnectPacket : ServerPacket
    {
        public string SessionId { get; set; }
    }

    [MemoryPackable]
    public partial class NtfSessionDisConnectPacket : ServerPacket
    {
        public string SessionId { get; set; }
    }

    [MemoryPackable]
    public partial class NtfUserEnterRoomPacket : ServerPacket
    {
        public string SessionId { get; set; }
        public int RoomNumber { get; set; }
    }

    [MemoryPackable]
    public partial class NtfUserLeaveRoomPacket : ServerPacket
    {
        public string SessionId { get; set; }
        public int RoomNumber { get; set; }
    }
}
