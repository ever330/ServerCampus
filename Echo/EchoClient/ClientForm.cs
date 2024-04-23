namespace EchoClient
{
    public partial class ClientForm : Form
    {
        private ClientNetwork _network;

        private Queue<EchoPacket> _recvQueue;
        private Queue<byte[]> _sendQueue;

        private Thread _networkLogThread;
        private Thread _recvThread;
        private Thread _sendThread;

        private bool _networkThreadRunning;

        public ClientForm()
        {
            InitializeComponent();
            _recvQueue = new Queue<EchoPacket>();
            _sendQueue = new Queue<byte[]>();
            _network = new ClientNetwork();

            _networkThreadRunning = true;

            _networkLogThread = new Thread(new ThreadStart(ShowNetworkLog));
            _networkLogThread.Start();
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {

        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
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
            while (_networkThreadRunning)
            {
                if (_network.NetworkMessageQ.Count > 0)
                {
                    logRichTextBox.Text += _network.NetworkMessageQ.Dequeue();
                    logRichTextBox.Text += "\n";
                }

                Thread.Sleep(10);
            }
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

            }
        }

        public void PostSendPacket(PACKET_ID pacetId, byte[] body)
        {
            if (_network == null || !_network.IsConnected())
            {
                logRichTextBox.Text += "서버에 연결이 되어있지 않습니다.";
                logRichTextBox.Text += "\n";
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
            sendData.AddRange(new byte[] { (byte)0 });

            if (body != null)
            {
                sendData.AddRange(body);
            }

            _sendQueue.Enqueue(sendData.ToArray());
        }
    }
}
