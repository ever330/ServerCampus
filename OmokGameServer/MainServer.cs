using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using MemoryPack;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using System.Diagnostics.Metrics;

namespace OmokGameServer
{
    public class MainServer : AppServer<ClientSession, OmokBinaryRequestInfo>, IHostedService
    {
        ILog _mainLogger;

        ServerOption _serverOption;
        IServerConfig _networkConfig;
        
        PacketProcessor _packetProcessor;
        DBProcessor _dbProcessor;

        UserManager _userManager;
        RoomManager _roomManager;
        DBManager _dbManager;

        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger<MainServer> _appLogger;

        Timer _heartBeatTimer;

        int _heartBeatCounter = 0;
        int _heartBeatIndex = 0;
        const int _heartBeatInterval = 1000;
        const int _heartBeatLimit = 200;

        public MainServer(IHostApplicationLifetime appLifetime, IOptions<ServerOption> serverConfig, ILogger<MainServer> logger)
            : base(new DefaultReceiveFilterFactory<ReceiveFilter, OmokBinaryRequestInfo>())
        {
            _serverOption = serverConfig.Value;
            _appLogger = logger;
            _appLifetime = appLifetime;

            NewSessionConnected += new SessionHandler<ClientSession>(OnConnected);
            SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnClosed);
            NewRequestReceived += new RequestHandler<ClientSession, OmokBinaryRequestInfo>(OnPacketReceived);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(AppOnStarted);
            _appLifetime.ApplicationStopped.Register(AppOnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void AppOnStarted()
        {
            _appLogger.LogInformation("OnStarted");

            InitConfig(_serverOption);

            CreateServer(_serverOption);

            var IsResult = base.Start();

            if (IsResult)
            {
                _mainLogger.Info("서버 네트워크 시작");
            }
            else
            {
                _appLogger.LogError("서버 네트워크 시작 실패");
                return;
            }
        }

        private void AppOnStopped()
        {
            _appLogger.LogInformation("OnStopped - begin");

            base.Stop();
            StopServer();

            _appLogger.LogInformation("OnStopped - end");
        }

        private void InitConfig(ServerOption serverOption)
        {
            _networkConfig = new ServerConfig
            {
                Name = serverOption.Name,
                Ip = "Any",
                Port = serverOption.Port,
                Mode = SocketMode.Tcp,
                MaxConnectionNumber = serverOption.MaxConnectionNumber,
                MaxRequestLength = serverOption.MaxRequestLength,
                ReceiveBufferSize = serverOption.ReceiveBufferSize,
                SendBufferSize = serverOption.SendBufferSize
            };
        }

        private void CreateServer(ServerOption serverOption)
        {
            try
            {
                bool bResult = Setup(new RootConfig(), _networkConfig, logFactory: new NLogLogFactory());

                if (bResult == false)
                {
                    _appLogger.LogError("서버 네트워크 설정 실패");
                    return;
                }
                else
                {
                    _mainLogger = base.Logger;
                }

                CreateComponent(serverOption);

                _mainLogger.Info("서버 생성 성공");
            }
            catch (Exception ex)
            {
                _appLogger.LogError($"[ERROR] 서버 생성 실패: {ex.ToString()}");
            }
        }

        public ERROR_CODE CreateComponent(ServerOption serverOpt)
        {
            _dbManager = new DBManager(serverOpt);

            _userManager = new UserManager();
            _userManager.Init(_dbManager, _serverOption.MaxConnectionNumber, SendData);

            _roomManager = new RoomManager();
            _roomManager.Init(_dbManager, _serverOption.RoomMaxCount, _serverOption.RoomMaxUserCount, SendData, InsertToDB);

            _packetProcessor = new PacketProcessor();
            _packetProcessor.Init(_mainLogger, _userManager, _roomManager, SendData, InsertToDB);
            _packetProcessor.RegistHandlers();

            _dbProcessor = new DBProcessor();
            _dbProcessor.Init(_mainLogger, _userManager, _dbManager, SendData, Distribute);
            _dbProcessor.RegistHandlers();

            _heartBeatTimer = new Timer(SendHeartBeat, null, 0, 250);

            _mainLogger.Info("CreateComponent - Success");
            return ERROR_CODE.NONE;
        }

        public bool SendData(string sessionId, byte[] data)
        {
            var session = GetSessionByID(sessionId);

            try
            {
                if (session == null)
                {
                    _mainLogger.Error($"Send Data Session is Null");
                    return false;
                }

                session.Send(data);

                return true;
            }
            catch (Exception ex)
            {
                _mainLogger.Error($"Send Data Error : {ex.ToString()}");

                session.SendEndWhenSendingTimeOut();
                session.Close();

                return false;
            }
        }

        public void StopServer()
        {
            _packetProcessor.Destroy();
            Stop();
        }

        void OnConnected(ClientSession session)
        {
            _mainLogger.Info($"[{DateTime.Now}] {session.SessionID} 접속, ThreadId : {Thread.CurrentThread.ManagedThreadId}");

            var req = new OmokBinaryRequestInfo(PacketDefine.PACKET_HEADER, (short)PACKET_ID.SESSION_CONNECT, null);
            req.SessionId = session.SessionID;
            _packetProcessor.InsertPacket(req);
        }

        void OnClosed(ClientSession session, CloseReason reason)
        {
            _mainLogger.Info($"[{DateTime.Now}] {session.SessionID} 접속 해제, {reason.ToString()}");

            var req = new OmokBinaryRequestInfo(PacketDefine.PACKET_HEADER, (short)PACKET_ID.SESSION_DISCONNECT, null);
            req.SessionId = session.SessionID;
            _packetProcessor.InsertPacket(req);
        }

        void OnPacketReceived(ClientSession session, OmokBinaryRequestInfo reqInfo)
        {
            _mainLogger.Info($"[{DateTime.Now}] {session.SessionID} 데이터 수신, 데이터 크기 : {reqInfo.Body.Length}, ThreadId : {Thread.CurrentThread.ManagedThreadId}");
            reqInfo.SessionId = session.SessionID;
            _packetProcessor.InsertPacket(reqInfo);
        }

        void SendHeartBeat(object o)
        {
            if (_userManager.GetUserCount() == 0)
            {
                return;
            }
            var pac = new ReqSendHeartBeatPacket();
            //while (_isServerRunning)
            //{
            //    if (_userManager.GetUserCount() == 0)
            //    {
            //        Thread.Sleep(_heartBeatInterval);
            //        continue;
            //    }
            //    else
            //    {
            //        foreach (var user in _userManager.GetUsers())
            //        {
            //            if (_heartBeatCounter >= _heartBeatIndex * (_serverOption.MaxConnectionNumber / 4) && _heartBeatCounter < (_heartBeatIndex + 1) * (_serverOption.MaxConnectionNumber / 4))
            //            {
            //                var userHeartBeat = user.Value.HeartBeatTime;
            //                var dif = DateTime.Now - userHeartBeat;
            //                if (dif.TotalSeconds >= _heartBeatLimit)
            //                {
            //                    _mainLogger.Info($"{user.Key} : 연결 지연으로 인하여 접속 강제 종료");
            //                    _userManager.RemoveUser(user.Key);
            //                    GetSessionByID(user.Key).Close();
            //                    continue;
            //                }
            //                var ntfPac = new NtfHeartBeatPacket();
            //                var ntf = MemoryPackSerializer.Serialize(ntfPac);
            //                var ntfData = ClientPacket.MakeClientPacket(PACKET_ID.NTF_HEART_BEAT, ntf);
            //                SendData(user.Key, ntfData);
            //            }
            //            _heartBeatCounter++;
            //        }
            //        _heartBeatIndex++;
            //        if (_heartBeatIndex >= 4)
            //        {
            //            _heartBeatIndex = 0;
            //            _heartBeatCounter = 0;
            //        }
            //        //_mainLogger.Info($"하트비트 전송");
            //        Thread.Sleep(_heartBeatInterval / 4);
            //    }
            //}
        }

        public void InsertToDB(DBRequestInfo dbReq)
        {
            _dbProcessor.InsertPacket(dbReq);
        }

        public void Distribute(OmokBinaryRequestInfo req)
        {
            _packetProcessor.InsertPacket(req);
        }
    }

    public class ClientSession : AppSession<ClientSession, OmokBinaryRequestInfo>
    {

    }
}
