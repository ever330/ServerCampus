using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmokGameServer
{
#if (__NOT_USE_NLOG__ != true)
    public class NLogLogFactory : SuperSocket.SocketBase.Logging.LogFactoryBase
    {
        public NLogLogFactory()
            : this("NLog.config")
        {
        }

        public NLogLogFactory(string nlogConfig)
            : base(nlogConfig)
        {
            if (!IsSharedConfig)
            {
                LogManager.Setup().LoadConfigurationFromFile(new[] { ConfigFile });
            }
            else
            {
            }
        }

        public override SuperSocket.SocketBase.Logging.ILog GetLog(string name)
        {
            return new NLogLog(NLog.LogManager.GetLogger(name));
        }
    }
#endif
}
