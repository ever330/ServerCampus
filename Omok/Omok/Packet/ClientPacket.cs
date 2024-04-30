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
        public string OtherUserId { get; set; }
    }

    [MemoryPackable]
    public partial class ReqLeaveRoomPacket
    {
        public int RoomNumber { get; set; }
    }

    [MemoryPackable]
    public partial class ResLeaveRoomPacket
    {
        public bool Result { get; set; }
    }

    [MemoryPackable]
    public partial class NtfNewUserPacket
    {
        public string Id { get; set; }
    }

    [MemoryPackable]
    public partial class NtfLeaveUserPacket
    {
        public string Id { get; set; }
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
        public int RoomNumber { get; set; }
    }

    [MemoryPackable]
    public partial class ResReadyPacket
    {
        public bool Result { get; set; }
    }

    [MemoryPackable]
    public partial class ReqNotReadyPacket
    {
        public int RoomNumber { get; set; }
    }

    [MemoryPackable]
    public partial class ResNotReadyPacket
    {
        public bool Result { get; set; }
    }

    [MemoryPackable]
    public partial class NtfReadyStatePacket
    {
        public string Id { set; get; }
        public bool Result { get; set; }
    }

    [MemoryPackable]
    public partial class NtfGameStartPacket
    {
        public string StartPlayer { set; get; }
    }

    [MemoryPackable]
    public partial class ReqPutStonePacket
    {
        public int RoomNumber { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
    }

    [MemoryPackable]
    public partial class ResPutStonePacket
    {
        public bool Result { get; set; }
    }

    [MemoryPackable]
    public partial class NtfPutStonePacket
    {
        public int Stone { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
    }

    [MemoryPackable]
    public partial class NtfWinPacket
    {
        public int Stone { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public string Id { get; set; }
    }

    [MemoryPackable]
    public partial class ReqTimeOutPacket
    {
        public int RoomNumber { get; set; }
    }
}