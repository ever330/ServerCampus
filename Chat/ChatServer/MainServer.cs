using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;

namespace ChatServer
{
    public class MainServer : AppServer<ClientSession, EBinaryRequestInfo>
    {
        public static ChatServerOption ServerOption;
        public static ILog MainLogger;

        IServerConfig _serverConfig;

        PacketProcessor _packetProcessor;
        RoomManager _roomManager;

        public MainServer()
            : base(new DefaultReceiveFilterFactory<ReceiveFilter, EBinaryRequestInfo>())
        {
            NewSessionConnected += new SessionHandler<ClientSession>(OnConnected);
            SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnClosed);
            NewRequestReceived += new RequestHandler<ClientSession, EBinaryRequestInfo>(OnPacketReceived);
        }

        public void InitConfig(ChatServerOption option)
        {
            ServerOption = option;

            _serverConfig = new ServerConfig
            {
                Name = option.Name,
                Ip = "Any",
                Port = option.Port,
                Mode = SocketMode.Tcp,
                MaxConnectionNumber = option.MaxConnectionNumber,
                MaxRequestLength = option.MaxRequestLength,
                ReceiveBufferSize = option.ReceiveBufferSize,
                SendBufferSize = option.SendBufferSize
            };
        }

        public void CreateStartServer()
        {
            try
            {
                bool result = Setup(new RootConfig(), _serverConfig, logFactory: new ConsoleLogFactory());

                if (!result)
                {
                    Console.WriteLine("서버 네트워크 설정 실패!");
                    return;
                }

                MainLogger = base.Logger;
                MainLogger.Info("서버 초기화 성공");
            }
            catch (Exception ex)
            {
                MainLogger.Error($"서버 생성 실패. 에러 : {ex.ToString()}");
            }
        }

        public void CloseServer()
        {
            base.Stop();
            _packetProcessor.Destroy();
        }

        void OnConnected(ClientSession session)
        {
            MainLogger.Info($"[{DateTime.Now}] {session.SessionID} 접속, ThreadId : {Thread.CurrentThread.ManagedThreadId}");
        }

        void OnClosed(ClientSession session, CloseReason reason)
        {
            MainLogger.Info($"[{DateTime.Now}] {session.SessionID} 접속 해제, {reason.ToString()}");
        }

        void OnPacketReceived(ClientSession session, EBinaryRequestInfo reqInfo)
        {
            MainLogger.Info($"[{DateTime.Now}] {session.SessionID} 데이터 수신, 데이터 크기 : {reqInfo.Body.Length}, ThreadId : {Thread.CurrentThread.ManagedThreadId}");

            var totalData = (short)(reqInfo.Body.Length + EBinaryRequestInfo.HEADER_SIZE);

            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(totalData));
            data.AddRange(BitConverter.GetBytes(reqInfo.PacketId));
            data.AddRange(reqInfo.Body);

            session.Send(data.ToArray(), 0, data.Count);
        }
    }

    public class ClientSession : AppSession<ClientSession, EBinaryRequestInfo>
    {

    }
}
