using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OmokGameServer
{
    public class ClientPacket
    {
        public static byte[] MakeClientPacket(PACKET_ID packetId, byte[] body)
        {
            short packetSize = (short)(PacketDefine.PACKET_HEADER + body.Length);

            var sendData = new byte[packetSize];
            FastBinaryWrite.Int16(sendData, 0, packetSize);
            FastBinaryWrite.Int16(sendData, 2, (short)packetId);
            Array.Copy(body, 0, sendData, 4, body.Length);

            return sendData;
        }
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
    public partial class NtfEnterRoomPacket
    {
        public int RoomNumber { get; set; }
        public string Id { get; set; }
    }

    [MemoryPackable]
    public partial class NtfLeaveRoomPacket
    {
        public int RoomNumber { get; set; }
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
