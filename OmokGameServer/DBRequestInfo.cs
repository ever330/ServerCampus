using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class DBRequest
    {
        public static DBRequestInfo MakeRequest(short packetId, string sessionId, byte[] body)
        {
            DBRequestInfo dbReq = new DBRequestInfo
            {
                PacketId = packetId,
                SessionId = sessionId,
                Body = body
            };

            return dbReq;
        }
    }

    public class DBRequestInfo
    {
        public short PacketId { get; set; }
        public string SessionId { get; set; }
        public byte[] Body { get; set; }
    }

    [MemoryPackable]
    public partial class ReqCheckAuthToken
    {
        public string UserId { get; set; }
        public string AuthToken { get; set; }
    }

    [MemoryPackable]
    public partial class ResCheckAuthToken
    {
        public string UserId { get; set; }
        public string AuthToken { get; set; }
        public bool Result { get; set; }
    }

    [MemoryPackable]
    public partial class ReqUserData
    {
        public string UserId { get; set; }
    }

    [MemoryPackable]
    public partial class ResUserData
    {
        public bool Result { get; set; }
        public string UserId { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
    }

    [MemoryPackable]
    public partial class ReqUpdateWinLose
    {
        public string UserId { get; set; }
        public bool Result { get; set; }
    }
}
