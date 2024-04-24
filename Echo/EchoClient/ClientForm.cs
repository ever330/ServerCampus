using MemoryPack;
using Microsoft.VisualBasic.Devices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EchoClient
{
    public partial class ClientForm : Form
    {
        private ClientNetwork _network;

        private Queue<PacketData> _recvQueue;
        private Queue<byte[]> _sendQueue;

        private Thread _recvThread;
        private Thread _sendThread;

        private PacketBufferManager _packetBufferManager;

        private bool _networkThreadRunning;

        public ClientForm()
        {
            InitializeComponent();
            _recvQueue = new Queue<PacketData>();
            _sendQueue = new Queue<byte[]>();
            _network = new ClientNetwork();

            _packetBufferManager = new PacketBufferManager(2048, PacketDefine.PACKET_HEADER);

            _recvThread = new Thread(ReceiveProcess);
            _recvThread.Start();
            _sendThread = new Thread(SendProcess);
            _sendThread.Start();

            _networkThreadRunning = true;

            processTimer.Tick += new EventHandler(BackGroundProcess);
            processTimer.Interval = 100;
            processTimer.Start();
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            if (sendTextBox.Text == "")
                return;

            EchoPacket echoPacket = new EchoPacket
            {
                Message = sendTextBox.Text
            };
            var packetData = MemoryPackSerializer.Serialize(echoPacket);

            PostSendPacket(PACKET_ID.PACKET_ID_ECHO, packetData);

            sendTextBox.Text = "";
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            if (_network.IsConnected())
            {
                logRichTextBox.Text += "이미 서버에 연결되어 있습니다.";
                logRichTextBox.Text += "\n";
                return;
            }

            int port = 0;
            if (!int.TryParse(portTextBox.Text, out port))
            {
                logRichTextBox.Text += "포트번호는 숫자만 입력해주세요.";
                logRichTextBox.Text += "\n";
                return;
            }

            _network.Connect(ipTextBox.Text, int.Parse(portTextBox.Text));
        }

        void ShowNetworkLog()
        {
            if (_network.NetworkMessageQ.Count > 0)
            {
                logRichTextBox.Text += _network.NetworkMessageQ.Dequeue();
                logRichTextBox.Text += "\n";
            }
            Thread.Sleep(1);
        }

        void ReceiveProcess()
        {
            while (_networkThreadRunning)
            {
                if (_network == null || !_network.IsConnected())
                {
                    Thread.Sleep(1);
                    continue;
                }

                var recvData = _network.Receive();

                if (recvData != null)
                {
                    _packetBufferManager.WriteData(recvData.Item2, recvData.Item1);

                    var data = _packetBufferManager.ReadData();
                    if (data.Count < 1)
                    {
                        break;
                    }

                    var packet = new PacketData();
                    packet.DataSize = (short)(data.Count - PacketDefine.PACKET_HEADER);
                    packet.PacketID = BitConverter.ToInt16(data.Array, data.Offset + 2);
                    packet.Body = new byte[packet.DataSize];
                    Buffer.BlockCopy(data.Array, data.Offset + 4, packet.Body, 0, packet.DataSize);
                    lock (((System.Collections.ICollection)_recvQueue).SyncRoot)
                    {
                        _recvQueue.Enqueue(packet);
                    }
                }
                else
                {
                    _network.Close();
                    SetDisconnect();
                }
            }
        }

        void SendProcess()
        {
            while (_networkThreadRunning)
            {
                Thread.Sleep(1);

                if (_network.IsConnected() == false)
                {
                    continue;
                }

                lock (((System.Collections.ICollection)_sendQueue).SyncRoot)
                {
                    if (_sendQueue.Count > 0)
                    {
                        var packet = _sendQueue.Dequeue();
                        _network.Send(packet);
                    }
                }
            }
        }

        void BackGroundProcess(object sender, EventArgs e)
        {
            ShowNetworkLog();

            try
            {
                var packet = new PacketData();

                lock (((System.Collections.ICollection)_recvQueue).SyncRoot)
                {
                    if (_recvQueue.Count() > 0)
                    {
                        packet = _recvQueue.Dequeue();
                    }
                }

                if (packet.PacketID != 0)
                {
                    PacketAnalysisProcess(packet);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("BackGroundProcess. error:{0}", ex.Message));
            }
        }

        void PacketAnalysisProcess(PacketData packet)
        {
            switch ((PACKET_ID)packet.PacketID)
            {
                case PACKET_ID.PACKET_ID_ECHO:
                    {
                        var echoPacket = MemoryPackSerializer.Deserialize<EchoPacket>(packet.Body);
                        _network.NetworkMessageQ.Enqueue($"메시지 : {echoPacket.Message}");
                    }
                    break;

                default:
                    break;
            }
        }

        public void PostSendPacket(PACKET_ID pacetId, byte[] body)
        {
            if (_network == null || !_network.IsConnected())
            {
                _network.NetworkMessageQ.Enqueue("서버에 연결이 되어있지 않습니다.");
                return;
            }

            short bodySize = 0;
            if (body != null)
            {
                bodySize = (short)body.Length;
            }

            var packetSize = PacketDefine.PACKET_HEADER + bodySize;

            List<byte> sendData = new List<byte>();
            sendData.AddRange(BitConverter.GetBytes((short)packetSize));
            sendData.AddRange(BitConverter.GetBytes((short)pacetId));

            if (body != null)
            {
                sendData.AddRange(body);
            }

            _sendQueue.Enqueue(sendData.ToArray());
        }

        void SetDisconnect()
        {
            _sendQueue.Clear();
            _network.NetworkMessageQ.Enqueue("서버 접속 종료");
        }

        private void disconnectBtn_Click(object sender, EventArgs e)
        {
            SetDisconnect();
        }
    }
}
