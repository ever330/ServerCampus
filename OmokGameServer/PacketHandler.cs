using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Logging;
using Microsoft.Extensions.Logging;

namespace OmokGameServer
{
    public class PacketHandler
    {
        protected UserManager _userManager;
        protected RoomManager _roomManager;
        protected ILog _logger;
        protected Func<string, byte[], bool> _sendFunc;

        public void Init(UserManager userManager, RoomManager roomManager, ILog logger, Func<string, byte[], bool> sendFunc)
        {
            _userManager = userManager;
            _roomManager = roomManager;
            _logger = logger;
            _sendFunc = sendFunc;
        }
    }
}
