using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;

namespace EchoServer
{
    public class MainServer : AppServer<EchoSession, EBinaryRequestInfo>
    {
        public static ILog MainLogger;

        IServerConfig _config;
        bool _isRun = false;

        public MainServer()
            : base(new DefaultReceiveFilterFactory<ReceiveFilter, EBinaryRequestInfo>())
        {
            NewSessionConnected += new SessionHandler<EchoSession>(OnConnected);
            SessionClosed += new SessionHandler<EchoSession, CloseReason>(OnClosed);
            NewRequestReceived += new RequestHandler<EchoSession, EBinaryRequestInfo>(RequestReceived);
        }

        public void InitConfig(int port, string ip, int maxConnection, string name)
        {
            _config = new ServerConfig 
            { 
                Port = 3000,
                Ip = "127.0.0.1",
                MaxConnectionNumber = maxConnection,
                Mode = SocketMode.Tcp,
                Name = name
            };
        }

        public void CreateServer()
        {
            try
            {
                bool result = Setup(new RootConfig(), _config, logFactory: new ConsoleLogFactory());

                if (!result)
                {
                    Console.WriteLine("서버 네트워크 설정 실패!");
                    return;
                }

                MainLogger = base.Logger;

                _isRun = true;
                MainLogger.Info($"[{DateTime.Now}] 서버 생성 성공");
            }
            catch (Exception ex)
            {
                MainLogger.Error($"서버 생성 실패. 에러 : {ex.ToString()}");
            }
        }

        public void CloseServer()
        {
            base.Stop();

            _isRun = false;
        }

        void OnConnected(EchoSession session)
        {
            MainLogger.Info($"[{DateTime.Now}] {session.SessionID} 접속, ThreadId : {Thread.CurrentThread.ManagedThreadId}");
        }

        void OnClosed(EchoSession session, CloseReason reason)
        {
            MainLogger.Info($"[{DateTime.Now}] {session.SessionID} 접속 해제, {reason.ToString()}");
        }

        void RequestReceived(EchoSession session, EBinaryRequestInfo reqInfo)
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
}
