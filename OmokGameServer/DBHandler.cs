using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
    public class DBHandler
    {
        protected DBManager _dbManager; 
        protected UserManager _userManager;
        protected ILog _logger;
        protected Func<string, byte[], bool> _sendFunc;
        protected Action<OmokBinaryRequestInfo> _sendToPP;

        public void Init(UserManager userManager, DBManager dbManager, ILog logger, Func<string, byte[], bool> sendFunc, Action<OmokBinaryRequestInfo> sendToPP)
        {
            _dbManager = dbManager;
            _userManager = userManager;
            _logger = logger;
            _sendFunc = sendFunc;
            _sendToPP = sendToPP;
        }
    }
}
