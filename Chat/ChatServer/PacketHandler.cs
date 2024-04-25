using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class PacketHandler
    {
        protected MainServer _serverNetwork;
        protected UserManager _userManager = null;


        public void Init(MainServer serverNetwork, UserManager userManager)
        {
            _serverNetwork = serverNetwork;
            _userManager = userManager;
        }
    }
}
