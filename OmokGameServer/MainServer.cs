﻿using System;
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
        int _heartBeatIndex = 0;
        const int HeartBeatInterval = 250;

        Timer _checkRoomTimer;
        int _checkRoomIndex = 0;
        const int CheckRoomInterval = 250;

        Timer _checkSessionTimer;
        int _checkSessionIndex = 0;
        const int CheckSessionInterval = 250;

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
            _userManager.Init(_dbManager, _serverOption.MaxConnectionNumber, SendData, DisconnectUser);

            _roomManager = new RoomManager();
            _roomManager.Init(_dbManager, _serverOption.RoomMaxCount, _serverOption.RoomMaxUserCount, SendData, InsertToDB);

            _packetProcessor = new PacketProcessor();
            _packetProcessor.Init(_mainLogger, _userManager, _roomManager, SendData, InsertToDB);
            _packetProcessor.RegistHandlers();

            _dbProcessor = new DBProcessor();
            _dbProcessor.Init(_mainLogger, _userManager, _dbManager, SendData, Distribute);
            _dbProcessor.RegistHandlers();

            _heartBeatTimer = new Timer(SendHeartBeat, null, 0, HeartBeatInterval);
            _checkRoomTimer = new Timer(SendCheckRoom, null, 0, CheckRoomInterval);
            _checkSessionTimer = new Timer(SendCheckSession, null, 0, CheckSessionInterval);

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
            pac.CurrentIndex = _heartBeatIndex;
            var pacData = MemoryPackSerializer.Serialize(pac);
            OmokBinaryRequestInfo req = new OmokBinaryRequestInfo((short)(pacData.Length + OmokBinaryRequestInfo.HEADER_SIZE), (short)PACKET_ID.REQ_SEND_HEART_BEAT, pacData);
            _packetProcessor.InsertPacket(req);
            _heartBeatIndex++;
            if (_heartBeatIndex >= 4)
            {
                _heartBeatIndex = 0;
            }
        }

        void SendCheckRoom(object o)
        {
            if (_userManager.GetUserCount() == 0)
            {
                return;
            }
            var pac = new ReqSendCheckRoomPacket();
            pac.CurrentIndex = _checkRoomIndex;
            var pacData = MemoryPackSerializer.Serialize(pac);
            OmokBinaryRequestInfo req = new OmokBinaryRequestInfo((short)(pacData.Length + OmokBinaryRequestInfo.HEADER_SIZE), (short)PACKET_ID.REQ_SEND_CHECK_ROOM, pacData);
            _packetProcessor.InsertPacket(req);
            _checkRoomIndex++;
            if (_checkRoomIndex >= 4)
            {
                _checkRoomIndex = 0;
            }
        }

        void SendCheckSession(object o)
        {
            if (_userManager.GetUserCount() == 0)
            {
                return;
            }
            var pac = new ReqSendCheckSessionPacket();
            pac.CurrentIndex = _checkSessionIndex;
            var pacData = MemoryPackSerializer.Serialize(pac);
            OmokBinaryRequestInfo req = new OmokBinaryRequestInfo((short)(pacData.Length + OmokBinaryRequestInfo.HEADER_SIZE), (short)PACKET_ID.REQ_SEND_CHECK_SESSION, pacData);
            _packetProcessor.InsertPacket(req);
            _checkSessionIndex++;
            if (_checkSessionIndex >= 4)
            {
                _checkSessionIndex = 0;
            }
        }

        public void InsertToDB(DBRequestInfo dbReq)
        {
            _dbProcessor.InsertPacket(dbReq);
        }

        public void Distribute(OmokBinaryRequestInfo req)
        {
            _packetProcessor.InsertPacket(req);
        }

        public void DisconnectUser(string sessionId)
        {
            var session = GetSessionByID(sessionId);
            session.Close();
        }
    }

    public class ClientSession : AppSession<ClientSession, OmokBinaryRequestInfo>
    {

    }
}
