using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatServer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;

namespace OmokGameServer
{
    public class MainServer : AppServer<ClientSession, OmokBinaryRequestInfo>, IHostedService
    {
        public static ILog MainLogger;

        ServerOption _serverOption;
        IServerConfig _networkConfig;

        /// <summary>
        /// 패킷 프로세서, 룸 매니저 만들기
        /// </summary>

        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger<MainServer> _appLogger;

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
                _appLogger.LogInformation("서버 네트워크 시작");
            }
            else
            {
                _appLogger.LogError("서버 네트워크 시작 실패");
                return;
            }
        }

        private void AppOnStopped()
        {
            MainLogger.Info("OnStopped - begin");

            base.Stop();

            MainLogger.Info("OnStopped - end");
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
                    MainLogger.Error("서버 네트워크 설정 실패");
                    return;
                }
                else
                {
                    MainLogger = base.Logger;
                }

                CreateComponent(serverOption);

                MainLogger.Info("서버 생성 성공");
            }
            catch (Exception ex)
            {
                MainLogger.Error($"[ERROR] 서버 생성 실패: {ex.ToString()}");
            }
        }

        public ERROR_CODE CreateComponent(ServerOption serverOpt)
        {
            // todo: 패킷 프로세서와 룸 매니저 기본 세팅

            MainLogger.Info("CreateComponent - Success");
            return ERROR_CODE.NONE;
        }

        public void StopServer()
        {
            Stop();
        }

        void OnConnected(ClientSession session)
        {
            MainLogger.Info($"[{DateTime.Now}] {session.SessionID} 접속, ThreadId : {Thread.CurrentThread.ManagedThreadId}");
        }

        void OnClosed(ClientSession session, CloseReason reason)
        {
            MainLogger.Info($"[{DateTime.Now}] {session.SessionID} 접속 해제, {reason.ToString()}");
        }

        void OnPacketReceived(ClientSession session, OmokBinaryRequestInfo reqInfo)
        {
            MainLogger.Info($"[{DateTime.Now}] {session.SessionID} 데이터 수신, 데이터 크기 : {reqInfo.Body.Length}, ThreadId : {Thread.CurrentThread.ManagedThreadId}");

        }
    }

    public class ClientSession : AppSession<ClientSession, OmokBinaryRequestInfo>
    {

    }
}
