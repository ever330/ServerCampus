using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class RequestMatchData
    {
        public string UserA { get; set; }
        public string UserB { get; set; }
    }

    public class ResponseMatchData
    {
        public string UserA { get; set; }
        public string UserB { get; set; }
        public string ServerAddress { get; set; }
        public int Port { get; set; }
        public int RoomNumber { get; set; }
    }
}
