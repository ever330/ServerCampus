using CloudStructures;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OmokGameServer
{
    public class RedisDBProcessor
    {
        bool _isThreadRunning = false;
        List<Thread> _threads = new List<Thread>();
        ILog _mainLogger;

        BufferBlock<DBRequestInfo> _packetBuffer = new BufferBlock<DBRequestInfo>();
        Dictionary<short, Action<RedisConnection, DBRequestInfo>> _handlerDict = new Dictionary<short, Action<RedisConnection, DBRequestInfo>>();

        RedisDBHandler _redisDBHandler = new RedisDBHandler();
        string _redisDBConnectionString;

        public void Init(ILog mainLogger, RedisDB redisDB, Func<string, byte[], bool> sendFunc, Action<OmokBinaryRequestInfo> sendToPP, int maxThreadCount, string redisDBConStr)
        {
            _mainLogger = mainLogger;
            _redisDBHandler.Init(redisDB, mainLogger, sendFunc, sendToPP);
            _isThreadRunning = true;
            _redisDBConnectionString = redisDBConStr;


            for (int i = 0; i < maxThreadCount; i++)
            {
                var thread = new Thread(Process);
                thread.Start();
            }
        }

        public void Destroy()
        {
            _isThreadRunning = false;
            _packetBuffer.Complete();
        }

        public void RegistHandlers()
        {
            _redisDBHandler.RegistPacketHandler(_handlerDict);
        }

        public void InsertPacket(DBRequestInfo req)
        {
            _packetBuffer.Post(req);
        }

        void Process()
        {
            var conf = new RedisConfig("HiveUsers", _redisDBConnectionString);
            var connection = new RedisConnection(conf);

            while (_isThreadRunning)
            {
                try
                {
                    var packet = _packetBuffer.Receive();

                    if (_handlerDict.ContainsKey(packet.PacketId))
                    {
                        _handlerDict[packet.PacketId](connection, packet);
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
