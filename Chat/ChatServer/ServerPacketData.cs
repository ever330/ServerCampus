using MemoryPack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class ServerPacketData
    {
        public short PacketSize;
        public string SessionId;
        public short PacketId;
        public byte[] Body;

        public void Assign(string sessionId, short packetId, byte[] packetBody)
        {
            SessionId = sessionId;
            PacketId = packetId;

            if (packetBody.Length > 0)
            {
                Body = packetBody;
            }
        }
    }



    [MemoryPackable]
    public partial class InternalNtfRoomLeavePacket
    {
        public int RoomNumber;

        public string UserId;
    }
}
