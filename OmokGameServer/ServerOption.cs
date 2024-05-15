using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class ServerOption
    {
        public int ServerUniqueID { get; set; }

        // SuperSocket 용도
        public string Name { get; set; }

        public int MaxConnectionNumber { get; set; }

        public int Port { get; set; }

        public int MaxRequestLength { get; set; }

        public int ReceiveBufferSize { get; set; }

        public int SendBufferSize { get; set; }

        // Application 용도
        public int RoomMaxCount { get; set; } = 0;

        public int RoomMaxUserCount { get; set; } = 0;

        public int RoomStartNumber { get; set; } = 0;
        public string GameDB { get; set; }
        public string RedisDB { get; set; }
        public int GameDBMaxThreadCount { get; set; } = 0;
        public int RedisDBMaxThreadCount { get; set; } = 0;
        public string ReqListKey { get; set; }
        public string ResListKey { get; set; }
    }
}
