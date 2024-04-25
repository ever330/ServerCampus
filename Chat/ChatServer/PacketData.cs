using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemoryPack;

namespace ChatServer
{
    public class PacketSerializer
    {
        public static byte[] Serialize(PACKET_ID packetId, byte[] body)
        {
            var pktId = (short)packetId;
            short bodySize = 0;
            if (body != null)
            {
                bodySize = (short)body.Length;
            }
            var packetSize = (short)(bodySize + PacketDefine.PACKET_HEADER);

            var dataSource = new byte[packetSize];
            Buffer.BlockCopy(BitConverter.GetBytes(packetSize), 0, dataSource, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(pktId), 0, dataSource, 2, 2);

            if (body != null)
            {
                Buffer.BlockCopy(body, 0, dataSource, 4, bodySize);
            }

            return dataSource;
        }
    }

    [MemoryPackable]
    public partial class ReqLoginPacket
    {
        public string UserId;

        public string AuthToken;
    }

    [MemoryPackable]
    public partial class ResLoginPacket
    {
        public short Result;
    }


    [MemoryPackable]
    public partial class NtfMustClosePacket
    {
        public short Result;
    }



    [MemoryPackable]
    public partial class ReqRoomEnterPacket
    {
        public int RoomNumber;
    }

    [MemoryPackable]
    public partial class ResRoomEnterPacket
    {
        public short Result;
    }

    [MemoryPackable]
    public partial class NtfRoomUserListPacket
    {
        public List<string> UserIdList = new List<string>();
    }

    [MemoryPackable]
    public partial class NtfRoomNewUserPacket
    {
        public string UserId;
    }


    [MemoryPackable]
    public partial class ReqRoomLeavePacket
    {

    }

    [MemoryPackable]
    public partial class ResRoomLeavePacket
    {
        public short Result;
    }

    [MemoryPackable]
    public partial class NtfRoomLeaveUserPacket
    {
        public string UserId;
    }


    [MemoryPackable]
    public partial class ReqRoomChatPacket
    {
        public string ChatMessage;
    }


    [MemoryPackable]
    public partial class NtfRoomChatPacket
    {
        public string UserId;

        public string ChatMessage;
    }
}
