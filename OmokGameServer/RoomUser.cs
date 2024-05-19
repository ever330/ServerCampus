using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class RoomUser
    {
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public STONE Stone { get; set; }
        public USER_STATE State { get; set; }
        public int TimeOutCount { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
    }
}
