using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using SuperSocket.SocketBase.Logging;

namespace OmokGameServer
{
    public class DBProcessor
    {
        bool _isThreadRunning = false;
        Thread _processThread;
        ILog _mainLogger;

        BufferBlock<DBRequestInfo> _packetBuffer = new BufferBlock<DBRequestInfo>();
        Dictionary<short, Action<DBRequestInfo>> _handlerDict = new Dictionary<short, Action<DBRequestInfo>>();

        RedisHandler _redisHandler = new RedisHandler();
        MySqlHandler _mySqlHandler = new MySqlHandler();

        public void Init(ILog mainLogger, UserManager userManager, DBManager dbManager, Func<string, byte[], bool> sendFunc, Action<OmokBinaryRequestInfo> sendToPP)
        {
            _mainLogger = mainLogger;
            _redisHandler.Init(userManager, dbManager, mainLogger, sendFunc, sendToPP);
            _mySqlHandler.Init(userManager, dbManager, mainLogger, sendFunc, sendToPP);
            _isThreadRunning = true;
            _processThread = new Thread(Process);
            _processThread.Start();
        }

        public void Destroy()
        {
            _isThreadRunning = false;
            _packetBuffer.Complete();
        }

        public void RegistHandlers()
        {
            _redisHandler.RegistPacketHandler(_handlerDict);
            _mySqlHandler.RegistPacketHandler(_handlerDict);
        }

        public void InsertPacket(DBRequestInfo req)
        {
            _packetBuffer.Post(req);
        }

        void Process()
        {
            while (_isThreadRunning)
            {
                try
                {
                    var packet = _packetBuffer.Receive();

                    if (_handlerDict.ContainsKey(packet.PacketId))
                    {
                        _handlerDict[packet.PacketId](packet);
                    }
                    else
                    {
                        _mainLogger.Info($"DBProcessor Error : 없는 패킷 ID {packet.PacketId}");
                    }
                }
                catch (Exception ex)
                {
                    _mainLogger.Error($"DBProcessor Error : {ex.ToString()}");
                }
            }
        }
    }
}
