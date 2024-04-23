using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EchoClient
{
    public class ClientNetwork
    {
        private Socket? _soc = null;

        public Queue<string> NetworkMessageQ = new Queue<string>();

        public bool Connect(string ip, int port)
        {
            try
            {
                _soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                var ipEp = new IPEndPoint(IPAddress.Parse("ip"), port);
                _soc.Connect(ipEp);

                if (_soc == null || !_soc.Connected)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                NetworkMessageQ.Enqueue(ex.Message);
                return false;
            }
        }

        public void Close()
        {
            if (_soc != null && _soc.Connected)
            {
                _soc.Close();
                NetworkMessageQ.Enqueue("연결을 종료하였습니다.");
            }
            NetworkMessageQ.Enqueue("연결이 되어있지 않습니다.");
        }

        public Tuple<int, byte[]> Receive()
        {
            try
            {
                byte[] receiveBuffer = new byte[2048];
                var nRecv = _soc.Receive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None);

                if (nRecv == 0)
                {
                    return null;
                }

                return new Tuple<int, byte[]>(nRecv, receiveBuffer);
            }
            catch (SocketException ex)
            {
                NetworkMessageQ.Enqueue(ex.Message);

                return null;
            }
        }

        public void Send(byte[] data)
        {
            try
            {
                if (_soc != null && _soc.Connected)
                {
                    _soc.Send(data);
                }
                else
                {
                    NetworkMessageQ.Enqueue("서버에 연결이 되어있지 않습니다.");
                }
            }
            catch (SocketException ex)
            {
                NetworkMessageQ.Enqueue(ex.Message);
            }
        }

        public bool IsConnected()
        {
            if (_soc != null && _soc.Connected)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
