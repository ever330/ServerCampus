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

    public class ReqSetAuthToken
    {
        public string UserId { get; set; }
        public string AuthToken { get; set; }
    }

    public class ReqCheckAuthToken
    {
        public string UserId { get; set; }
        public string AuthToken { get; set; }
    }

    public class ReqUpdateWinLose
    {
        public string UserId { get; set; }
        public bool Result { get; set; }
    }
}
