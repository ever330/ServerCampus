using ChatServer;
using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OmokGameServer
{
    public class PacketProcessor
    {
        bool _isThreadRunning = false;
        Thread _processThread;
        ILog MainLogger;

        BufferBlock<OmokBinaryRequestInfo> _packetBuffer = new BufferBlock<OmokBinaryRequestInfo>();


        public void Init(ILog mainLogger)
        {
            MainLogger = mainLogger;
            _isThreadRunning = true;
            _processThread= new Thread(Process);
            _processThread.Start();
        }

        public void Process()
        {
            while (_isThreadRunning)
            {
                try
                {
                    var packet = _packetBuffer.Receive();
                }
                catch
                {

                }
            }
        }
    }
}
