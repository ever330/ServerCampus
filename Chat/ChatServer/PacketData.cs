using System;
using System.Collections.Generic;
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
    public partial class RoomUsersPacket
    {
        public List<string> UserIdList = new List<string>();
    }

    [MemoryPackable]
    public partial class NewUserPacket
    {
        public string UserId;
    }

    [MemoryPackable]
    public partial class LeaveUserPacket
    {
        public string UserId;
    }
}
