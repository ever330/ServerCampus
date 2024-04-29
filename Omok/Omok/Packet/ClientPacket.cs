using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;

namespace Omok.Packet
{
    public struct ClientPacket
    {
        public short PacketSize {  get; set; }
        public short PacketId {  get; set; }
        public byte[] Body { get; set; }
    }

    [MemoryPackable]
    public partial class ReqLoginPacket
    {
        public string Id { get; set; }
    }

    [MemoryPackable]
    public partial class ResLoginPacket
    {
        public bool Result { get; set; }
    }

    [MemoryPackable]
    public partial class ReqEnterRoomPacket
    {

    }

    [MemoryPackable]
    public partial class ResEnterRoomPacket
    {
        public int RoomNumber { get; set; }
    }

    [MemoryPackable]
    public partial class ReqChatPacket
    {
        public string Chat { get; set; }
    }

    [MemoryPackable]
    public partial class NtfChatPacket
    {
        public string Id { set; get; }
        public string Chat { get; set; }
    }

    [MemoryPackable]
    public partial class ReqReadyPacket
    {

    }

    [MemoryPackable]
    public partial class ResReadyPacket
    {
        public bool Result { get; set; }
    }

    [MemoryPackable]
    public partial class NtfReadyPacket
    {
        public bool Result { get; set; }
    }
}