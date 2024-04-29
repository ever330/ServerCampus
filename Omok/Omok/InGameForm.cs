using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EchoClient;
using MemoryPack;
using Microsoft.VisualBasic.Devices;
using Omok.Models;
using Omok.Packet;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Omok
{
    public partial class InGameForm : Form
    {
        string _email = null!;
        string _authToken = null!;

        int _level;
        int _exp;
        int _winCount;
        int _loseCount;
        int _money;

        MailBoxForm _mailBoxForm;
        ClientNetwork _clientNetwork;
        PacketBufferManager _packetBufferManager;

        bool _isNetworkRunning = false;
        Thread _recvThread;
        Thread _sendThread;

        Queue<ClientPacket> _recvQueue = new Queue<ClientPacket>();
        Queue<byte[]> _sendQueue = new Queue<byte[]>();

        int _margin = 40;
        int _gridSize = 30; // gridSize
        int _stoneSize = 28; // stoneSize
        int _flowerDotSize = 10; // flowerSize

        Graphics _graphics;
        Pen _pen;
        Brush _wBrush, _bBrush;

        enum STONE { none, black, white };
        STONE[,] _board = new STONE[19, 19];
        bool flag = false;  // false = 검은 돌, true = 흰돌

        public InGameForm()
        {
            InitializeComponent();

            _mailBoxForm = new MailBoxForm();
        }

        public void Init()
        {
            _clientNetwork = new ClientNetwork();
            _clientNetwork.Connect("127.0.0.1", 3030);

            _packetBufferManager = new PacketBufferManager(PacketDefine.PACKET_BUFFER_SIZE, PacketDefine.PACKET_HEADER);

            _isNetworkRunning = true;
            TryLoginToOmokServer();
            SetInGameData("Guest", "test", 1, 0, 0, 0, 0);

            _recvThread = new Thread(ReceiveProcess);
            _recvThread.Start();
            _sendThread = new Thread(SendProcess);
            _sendThread.Start();

            backGroundTimer.Tick += new EventHandler(BackGroundProcess);
            backGroundTimer.Interval = 100;
            backGroundTimer.Start();
            ShowDialog();
        }

        public void SetInGameData(string email, string authToken, int level, int exp, int winCount, int loseCount, int money)
        {
            _email = email;
            _authToken = authToken;
            _level = level;
            _exp = exp;
            _winCount = winCount;
            _loseCount = loseCount;
            _money = money;

            RefreshUserData();
        }

        public void RefreshUserData()
        {
            emailLabel.Text = _email;
            levelLabel.Text = _level + $"({_exp})";
            float winningRate = 0;
            if (_winCount != 0)
            {
                winningRate = _winCount / (_winCount + _loseCount) * 100;
            }
            winLoseLabel.Text = winningRate + $"%(W:{_winCount}/L:{_loseCount})";
            moneyLabel.Text = _money.ToString();
        }

        private async void attendanceBtn_Click(object sender, EventArgs e)
        {
            await TryDailyAttendance();
        }

        private async Task TryDailyAttendance()
        {
            var client = new HttpClient();

            var request = new ReqDailyAttendance
            {
                Email = _email,
                AuthToken = _authToken
            };

            var response = await client.PostAsJsonAsync("http://localhost:5292/api/Attendance/attendance", request);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("출석체크 오류가 발생하였습니다. 상태 코드 : " + response.StatusCode);
            }

            ResDailyAttendance? res = await response.Content.ReadFromJsonAsync<ResDailyAttendance>();

            if (res == null || res.Result == ErrorCode.AttendanceError)
            {
                MessageBox.Show("출석체크 오류가 발생하였습니다. 상태 코드 : " + response.StatusCode);
                return;
            }

            if (res.Result == ErrorCode.AttendanceAlready)
            {
                MessageBox.Show("이미 출석체크를 완료하였습니다.");
                return;
            }

            if (res.Result == ErrorCode.None)
            {
                MessageBox.Show($"출석체크 완료! 연속 출석 일수 : {res.ConsecutiveAttendance}\n" + $"보상이 우편함으로 전송되었습니다.");
            }
        }

        private async void mailboxBtn_Click(object sender, EventArgs e)
        {
            await TryGetMail();
            _mailBoxForm.ShowDialog();
        }

        private async Task TryGetMail()
        {
            var client = new HttpClient();

            var request = new ReqGetMail
            {
                Email = _email,
                AuthToken = _authToken
            };

            var response = await client.PostAsJsonAsync("http://localhost:5292/api/GetMail/getMail", request);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("우편함 가져오기 에러. 상태 코드 : " + response.StatusCode);
                return;
            }

            ResGetMail? res = await response.Content.ReadFromJsonAsync<ResGetMail>();

            if (res == null || res.Result != ErrorCode.None)
            {
                MessageBox.Show("우편함 가져오기 에러. 상태 코드 : " + response.StatusCode);
                return;
            }

            if (res.Mails == null || res.Mails.Count == 0)
            {
                MessageBox.Show("우편함이 비어있습니다.");
                return;
            }

            _mailBoxForm.MailSetting(res.Mails);
        }

        private void TryLoginToOmokServer()
        {
            ReqLoginPacket req = new ReqLoginPacket();
            req.Id = "게스트";

            byte[] reqData = MemoryPackSerializer.Serialize(req);

            _sendQueue.Enqueue(MakeSendData(PACKET_ID.REQ_LOGIN, reqData));
        }

        void SendProcess()
        {
            while (_isNetworkRunning)
            {
                if (_clientNetwork == null || !_clientNetwork.IsConnected())
                {
                    Thread.Sleep(1);
                    continue;
                }

                lock (((System.Collections.ICollection)_sendQueue).SyncRoot)
                {
                    if (_sendQueue.Count > 0)
                    {
                        var packet = _sendQueue.Dequeue();
                        _clientNetwork.Send(packet);
                    }
                }
            }
        }

        void ReceiveProcess()
        {
            while (_isNetworkRunning)
            {
                if (_clientNetwork == null || !_clientNetwork.IsConnected())
                {
                    Thread.Sleep(1);
                    continue;
                }

                var recvData = _clientNetwork.Receive();

                if (recvData != null)
                {
                    _packetBufferManager.WriteData(recvData.Item2, recvData.Item1);

                    var data = _packetBufferManager.ReadData();
                    if (data.Count < 1)
                    {
                        break;
                    }

                    byte[] pac = data.ToArray();
                    short packetSize = BitConverter.ToInt16(pac, 0);
                    short packetId = BitConverter.ToInt16(pac, 2);
                    byte[] packetdata = new byte[packetSize - PacketDefine.PACKET_HEADER];
                    Array.Copy(pac, PacketDefine.PACKET_HEADER, packetdata, 0, packetSize - PacketDefine.PACKET_HEADER);

                    ClientPacket packet = new ClientPacket();
                    packet.PacketSize = packetSize;
                    packet.PacketId = packetId;
                    packet.Body = packetdata;

                    lock (((System.Collections.ICollection)_recvQueue).SyncRoot)
                    {
                        _recvQueue.Enqueue(packet);
                    }
                }
                else
                {
                    _clientNetwork.Close();
                }
            }
        }

        void BackGroundProcess(object sender, EventArgs e)
        {
            if (_clientNetwork.NetworkMessageQ.Count != 0)
            {
                string text = "";
                _clientNetwork.NetworkMessageQ.TryDequeue(out text);
                netMessageRTB.AppendText(text + "\n");
                netMessageRTB.SelectionStart = netMessageRTB.Text.Length;
                netMessageRTB.ScrollToCaret();
            }

            try
            {
                var packet = new ClientPacket();
                lock (((System.Collections.ICollection)_recvQueue).SyncRoot)
                {
                    if (_recvQueue.Count() > 0)
                    {
                        packet = _recvQueue.Dequeue();
                    }
                }
                PacketAnalysis(packet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("BackGroundProcess. error:{0}", ex.Message));
            }
        }

        void PacketAnalysis(ClientPacket packet)
        {
            switch ((PACKET_ID)packet.PacketId)
            {
                case PACKET_ID.RES_LOGIN:
                    {
                        var loginPacket = MemoryPackSerializer.Deserialize<ResLoginPacket>(packet.Body);
                        _clientNetwork.NetworkMessageQ.Enqueue($"로그인 결과 : {loginPacket.Result}");
                    }
                    break;

                case PACKET_ID.RES_ENTER_ROOM:
                    {
                        var enterRoomPacket = MemoryPackSerializer.Deserialize<ResEnterRoomPacket>(packet.Body);
                        _clientNetwork.NetworkMessageQ.Enqueue($"방입장 결과 : {enterRoomPacket.RoomNumber}");
                        roomNumberText.Text = enterRoomPacket.RoomNumber.ToString();
                        InitializeBoard();
                    }
                    break;

                case PACKET_ID.NTF_ROOM_CHAT:
                    {
                        var chatPacket = MemoryPackSerializer.Deserialize<NtfChatPacket>(packet.Body);
                        chatRTB.AppendText($"{chatPacket.Id} : {chatPacket.Chat}\n");
                        chatRTB.SelectionStart = chatRTB.Text.Length;
                        chatRTB.ScrollToCaret();
                    }
                    break;

                default:
                    break;
            }
        }

        private void enterRoomBtn_Click(object sender, EventArgs e)
        {
            ReqEnterRoomPacket req = new ReqEnterRoomPacket();
            byte[] body = MemoryPackSerializer.Serialize(req);

            _sendQueue.Enqueue(MakeSendData(PACKET_ID.REQ_ENTER_ROOM, body));
        }

        byte[] MakeSendData(PACKET_ID packetId, byte[] body)
        {
            short packetSize = (short)(PacketDefine.PACKET_HEADER + body.Length);

            byte[] sendData = new byte[packetSize];
            Array.Copy(BitConverter.GetBytes(packetSize), 0, sendData, 0, 2);
            Array.Copy(BitConverter.GetBytes((short)packetId), 0, sendData, 2, 2);
            Array.Copy(body, 0, sendData, 4, body.Length);

            return sendData;
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            TryLoginToOmokServer();
        }

        private void chatSendBtn_Click(object sender, EventArgs e)
        {
            if (chatTextBox.Text == "")
            {
                return;
            }

            ReqChatPacket req = new ReqChatPacket();
            req.Chat = chatTextBox.Text;
            chatTextBox.Clear();

            _sendQueue.Enqueue(MakeSendData(PACKET_ID.REQ_ROOM_CHAT, MemoryPackSerializer.Serialize(req)));
        }

        private void InitializeBoard()
        {
            _pen = new Pen(Color.Black);
            _bBrush = new SolidBrush(Color.Black);
            _wBrush = new SolidBrush(Color.White);

            _graphics = omokPanel.CreateGraphics();

            // 세로선 19개
            for (int i = 0; i < 19; i++)
            {
                _graphics.DrawLine(_pen, new Point(_margin + i * _gridSize, _margin),
                  new Point(_margin + i * _gridSize, _margin + 18 * _gridSize));
            }

            // 가로선 19개
            for (int i = 0; i < 19; i++)
            {
                _graphics.DrawLine(_pen, new Point(_margin, _margin + i * _gridSize),
                  new Point(_margin + 18 * _gridSize, _margin + i * _gridSize));
            }

            // 화점그리기
            for (int x = 3; x <= 15; x += 6)
                for (int y = 3; y <= 15; y += 6)
                {
                    _graphics.FillEllipse(_bBrush,
                    _margin + _gridSize * x - _flowerDotSize / 2,
                      _margin + _gridSize * y - _flowerDotSize / 2,
                      _flowerDotSize, _flowerDotSize);
                }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            // e.X는 픽셀단위, x는 바둑판 좌표
            int x = (e.X - _margin + _gridSize / 2) / _gridSize;
            int y = (e.Y - _margin + _gridSize / 2) / _gridSize;

            if (_board[x, y] != STONE.none)
                return;

            // 바둑판[x,y] 에 돌을 그린다
            Rectangle r = new Rectangle(
              _margin + _gridSize * x - _stoneSize / 2,
              _margin + _gridSize * y - _stoneSize / 2,
              _stoneSize, _stoneSize);

            // 검은돌 차례
            if (flag == false)
            {
                _graphics.FillEllipse(_bBrush, r);
                flag = true;
                _board[x, y] = STONE.black;
            }
            else
            {
                _graphics.FillEllipse(_wBrush, r);
                flag = false;
                _board[x, y] = STONE.white;
            }
        }
    }
}
