using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OmokGameServer
{
    public class GameDBProcessor
    {
        bool _isThreadRunning = false;
        List<Thread> _threads = new List<Thread>();
        ILog _mainLogger;

        BufferBlock<DBRequestInfo> _packetBuffer = new BufferBlock<DBRequestInfo>();
        Dictionary<short, Action<QueryFactory, DBRequestInfo>> _handlerDict = new Dictionary<short, Action<QueryFactory, DBRequestInfo>>();

        GameDBHandler _gameDBHandler = new GameDBHandler();
        string _gameDBConnectionString;

        public void Init(ILog mainLogger, GameDB gameDB, Func<string, byte[], bool> sendFunc, Action<OmokBinaryRequestInfo> sendToPP, int maxThreadCount, string gameDBConStr)
        {
            _mainLogger = mainLogger;
            _gameDBHandler.Init(gameDB, mainLogger, sendFunc, sendToPP);
            _isThreadRunning = true;
            _gameDBConnectionString = gameDBConStr;


            for (int i = 0; i < maxThreadCount ; i++)
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
            _gameDBHandler.RegistPacketHandler(_handlerDict);
        }

        public void InsertPacket(DBRequestInfo req)
        {
            _packetBuffer.Post(req);
        }

        void Process()
        {
            var mySqlConnection = new MySqlConnection(_gameDBConnectionString);
            var compiler = new MySqlCompiler();

            mySqlConnection.Open();
            var queryFactory = new QueryFactory(mySqlConnection, compiler);

            while (_isThreadRunning)
            {
                try
                {
                    if (mySqlConnection.State != System.Data.ConnectionState.Open)
                    {
                        mySqlConnection = new MySqlConnection(_gameDBConnectionString);
                        queryFactory = new QueryFactory(mySqlConnection, compiler);
                    }

                    var packet = _packetBuffer.Receive();

                    if (_handlerDict.ContainsKey(packet.PacketId))
                    {
                        _handlerDict[packet.PacketId](queryFactory, packet);
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
            mySqlConnection.Close();
        }
    }
}
