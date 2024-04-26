using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class PacketHandler
    {
        UserManager _userManager;

        public void Init(UserManager userManager)
        {
            _userManager = userManager;
        }
    }
}
