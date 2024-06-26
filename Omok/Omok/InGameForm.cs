﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
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
using OmokGameServer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Omok
{
    public partial class InGameForm : Form
    {
        UserInfo _userInfo;

        MailBoxForm _mailBoxForm;
        ClientNetwork _clientNetwork;
        PacketBufferManager _packetBufferManager;

        bool _isNetworkRunning = false;
        Thread _recvThread;
        Thread _sendThread;

        Queue<ClientPacket> _recvQueue = new Queue<ClientPacket>();
        Queue<byte[]> _sendQueue = new Queue<byte[]>();

        const int MARGIN = 10;
        const int GRID_SIZE = 20;
        const int STONE_SIZE = 17;
        const int FLOWER_DOT_SIZE = 7;

        Graphics _graphics;
        Pen _pen;
        Brush _wBrush, _bBrush;

        enum STONE { NONE, BLACK, WHITE };
        enum USER_STATE { NONE, READY, PLAYING }

        const int BOARD_SIZE = 19;
        STONE[,] _board = new STONE[BOARD_SIZE, BOARD_SIZE];

        STONE _myStone = STONE.NONE;
        USER_STATE _state = USER_STATE.NONE;

        int _selectedX, _selectedY = -1;
        int _roomNumber = -1;

        string _otherUserId;
        bool _isGamePlaying = false;
        int _limitTime;

        const int StartTimeLimit = 30;

        DateTime _serverHeartBeatTime;
        const int ServerHeartBeatTimeLimit = 5;

        bool _isMatching = false;

        public InGameForm()
        {
            InitializeComponent();

            _mailBoxForm = new MailBoxForm();
        }

        public void Init(string id, string authToken)
        {
            _userInfo = new UserInfo
            {
                Email = id,
                AuthToken = authToken,
                Id = id,
                Level = 1,
                Exp = 0,
                WinCount = 0,
                LoseCount = 0,
                Money = 0
            };

            SetUserInfo(_userInfo);

            checkMatchingTimer.Tick += new EventHandler(CheckMatching);
            checkMatchingTimer.Interval = 500;
        }

        void Connect(string ip, int port)
        {
            _clientNetwork = new ClientNetwork();
            _clientNetwork.Connect(ip, port);

            _packetBufferManager = new PacketBufferManager(PacketDefine.PacketBufferSize, PacketDefine.PacketHeader);

            _isNetworkRunning = true;

            TryLoginToOmokServer();

            _recvThread = new Thread(ReceiveProcess);
            _recvThread.Start();
            _sendThread = new Thread(SendProcess);
            _sendThread.Start();

            limitTimer.Tick += new EventHandler(PutStoneTimer);
            limitTimer.Interval = 1000;

            backGroundTimer.Tick += new EventHandler(BackGroundProcess);
            backGroundTimer.Interval = 100;
            backGroundTimer.Start();

            heartBeatTimer.Tick += new EventHandler(SendHeartBeat);
            heartBeatTimer.Interval = 1000;
            heartBeatTimer.Start();
        }

        private void InGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isNetworkRunning)
            {
                _isNetworkRunning = false;
                _clientNetwork.Close();
                backGroundTimer.Stop();
            }
        }

        public void SetUserInfo(UserInfo userInfo)
        {
            _userInfo = userInfo;

            RefreshUserData();
        }

        public void RefreshUserData()
        {
            idLabel.Text = _userInfo.Email;
            levelLabel.Text = _userInfo.Level + $"({_userInfo.Exp})";
            float winningRate = 0;
            if (_userInfo.WinCount != 0)
            {
                winningRate = _userInfo.WinCount / (_userInfo.WinCount + _userInfo.LoseCount) * 100;
            }
            winLoseLabel.Text = winningRate + $"%(W:{_userInfo.WinCount}/L:{_userInfo.LoseCount})";
            moneyLabel.Text = _userInfo.Money.ToString();
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
                Email = _userInfo.Email,
                AuthToken = _userInfo.AuthToken
            };

            var response = await client.PostAsJsonAsync("http://localhost:5292/api/Attendance/attendance", request);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("출석체크 오류가 발생하였습니다. 상태 코드 : " + response.StatusCode);
                return;
            }

            var res = await response.Content.ReadFromJsonAsync<ResDailyAttendance>();

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
                Email = _userInfo.Email,
                AuthToken = _userInfo.AuthToken
            };

            var response = await client.PostAsJsonAsync("http://localhost:5292/api/GetMail/getMail", request);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("우편함 가져오기 에러. 상태 코드 : " + response.StatusCode);
                return;
            }

            var res = await response.Content.ReadFromJsonAsync<ResGetMail>();

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
            var req = new ReqLoginPacket();
            req.Id = _userInfo.Id;
            req.AuthToken = _userInfo.AuthToken;
            _serverHeartBeatTime = DateTime.Now;

            var reqData = MemoryPackSerializer.Serialize(req);

            _sendQueue.Enqueue(MakeSendData(PacketId.ReqLogin, reqData));
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

                    var pac = data.ToArray();

                    var packet = new ClientPacket();
                    packet.PacketSize = FastBinaryRead.Int16(pac, 0);
                    packet.PacketId = FastBinaryRead.Int16(pac, 2);
                    packet.Body = FastBinaryRead.Bytes(pac, PacketDefine.PacketHeader, packet.PacketSize - PacketDefine.PacketHeader);

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
            switch ((PacketId)packet.PacketId)
            {
                case PacketId.ResLogin:
                    {
                        var loginPacket = MemoryPackSerializer.Deserialize<ResLoginPacket>(packet.Body);
                        _clientNetwork.NetworkMessageQ.Enqueue($"로그인 결과 : {loginPacket.Result}");

                        var req = new ReqEnterRoomPacket();
                        req.RoomNumber = _roomNumber;
                        var body = MemoryPackSerializer.Serialize(req);

                        _sendQueue.Enqueue(MakeSendData(PacketId.ReqEnterRoom, body));
                    }
                    break;

                case PacketId.ResEnterRoom:
                    {
                        var enterRoomPacket = MemoryPackSerializer.Deserialize<ResEnterRoomPacket>(packet.Body);
                        otherUserTextLabel.Text = enterRoomPacket.OtherUserId;
                        _otherUserId = enterRoomPacket.OtherUserId;
                        InitializeBoard();
                    }
                    break;

                case PacketId.ResLeaveRoom:
                    {
                        var leaveRoomPacket = MemoryPackSerializer.Deserialize<ResLeaveRoomPacket>(packet.Body);
                        _clientNetwork.NetworkMessageQ.Enqueue("방 퇴장");
                        roomNumberText.Text = "-1";
                        _roomNumber = -1;
                        _otherUserId = "";
                        otherUserTextLabel.Text = "";
                        omokPanel.Refresh();
                        reqMatchingBtn.Text = "매칭 요청";
                    }
                    break;

                case PacketId.NtfNewUser:
                    {
                        var leaveRoomPacket = MemoryPackSerializer.Deserialize<NtfNewUserPacket>(packet.Body);
                        _clientNetwork.NetworkMessageQ.Enqueue("새 유저 입장");
                        otherUserTextLabel.Text = leaveRoomPacket.Id;
                        _otherUserId = leaveRoomPacket.Id;
                    }
                    break;

                case PacketId.ResReady:
                    {
                        var resReadyPac = MemoryPackSerializer.Deserialize<ResReadyPacket>(packet.Body);
                        if (resReadyPac.Result)
                        {
                            _state = USER_STATE.READY;
                            stateTextLabel.Text = "준비완료";
                            readyBtn.Text = "준비해제";
                        }
                    }
                    break;

                case PacketId.ResNotReady:
                    {
                        var resReadyPac = MemoryPackSerializer.Deserialize<ResNotReadyPacket>(packet.Body);
                        if (resReadyPac.Result)
                        {
                            _state = USER_STATE.NONE;
                            stateTextLabel.Text = "대기중";
                            readyBtn.Text = "게임준비";
                        }
                    }
                    break;

                case PacketId.NtfReadyState:
                    {
                        var readyStatePacket = MemoryPackSerializer.Deserialize<NtfReadyStatePacket>(packet.Body);
                        _clientNetwork.NetworkMessageQ.Enqueue("상대방 상태 변경");
                        if (readyStatePacket.Result)
                        {
                            otherUserStateLabel.Text = "준비완료";
                        }
                        else
                        {
                            otherUserStateLabel.Text = "대기중";
                        }
                    }
                    break;

                case PacketId.NtfRoomChat:
                    {
                        var chatPacket = MemoryPackSerializer.Deserialize<NtfChatPacket>(packet.Body);
                        chatRTB.AppendText($"{chatPacket.Id} : {chatPacket.Chat}\n");
                        chatRTB.SelectionStart = chatRTB.Text.Length;
                        chatRTB.ScrollToCaret();
                    }
                    break;

                case PacketId.NtfGameStart:
                    {
                        var startPacket = MemoryPackSerializer.Deserialize<NtfGameStartPacket>(packet.Body);
                        _clientNetwork.NetworkMessageQ.Enqueue("게임 시작");
                        otherUserStateLabel.Text = "게임중";
                        stateTextLabel.Text = "게임중";
                        readyBtn.Text = "게임중";
                        _state = USER_STATE.PLAYING;
                        readyBtn.Enabled = false;
                        _isGamePlaying = true;
                        _limitTime = StartTimeLimit;
                        limitTimer.Start();

                        if (startPacket.StartPlayer == _userInfo.Id)
                        {
                            _myStone = STONE.BLACK;
                            putBtn.Enabled = true;
                            turnLabel.Text = "내차례";
                        }
                        else
                        {
                            _myStone = STONE.WHITE;
                            turnLabel.Text = "상대차례";
                        }
                    }
                    break;

                case PacketId.ResPutStone:
                    {
                        var putPacket = MemoryPackSerializer.Deserialize<ResPutStonePacket>(packet.Body);
                        if (putPacket.Result)
                        {
                            _limitTime = StartTimeLimit;
                            turnLabel.Text = "상대차례";
                            SelectClear(_selectedX, _selectedY);
                            DrawStone(_myStone, _selectedX, _selectedY);
                            _selectedX = -1;
                            _selectedY = -1;
                        }
                        else
                        {
                            putBtn.Enabled = true;
                            MessageBox.Show("놓을 수 없는 위치입니다.");
                        }
                    }
                    break;

                case PacketId.NtfPutStone:
                    {
                        _limitTime = StartTimeLimit;
                        turnLabel.Text = "내차례";
                        putBtn.Enabled = true;
                        var putPacket = MemoryPackSerializer.Deserialize<NtfPutStonePacket>(packet.Body);
                        DrawStone((STONE)putPacket.Stone, putPacket.PosX, putPacket.PosY);
                    }
                    break;

                case PacketId.NtfWinGame:
                    {
                        var winPacket = MemoryPackSerializer.Deserialize<NtfWinPacket>(packet.Body);
                        SelectClear(_selectedX, _selectedY);
                        DrawStone((STONE)winPacket.Stone, winPacket.PosX, winPacket.PosY);
                        MessageBox.Show($"게임 종료!\n승자 : {winPacket.Id}");
                        otherUserStateLabel.Text = "대기중";
                        stateTextLabel.Text = "대기중";
                        readyBtn.Text = "게임준비";
                        readyBtn.Enabled = true;
                        _clientNetwork.NetworkMessageQ.Enqueue("게임 종료");
                        limitTimer.Stop();
                        _state = USER_STATE.NONE;
                        BoardClear();
                        omokPanel.Refresh();
                        InitializeBoard();
                    }
                    break;

                case PacketId.ResHeartBeat:
                    {
                        var reqHeartBeat = MemoryPackSerializer.Deserialize<ResHeartBeatPacket>(packet.Body);
                        _serverHeartBeatTime = DateTime.Now;
                    }
                    break;

                case PacketId.NtfTimeOut:
                    {
                        var res = MemoryPackSerializer.Deserialize<NtfTimeOutPacket>(packet.Body);
                        if ((STONE)res.Stone != _myStone)
                        {
                            _clientNetwork.NetworkMessageQ.Enqueue("상대방 시간 초과");
                            _limitTime = StartTimeLimit;
                            turnLabel.Text = "내차례";
                            putBtn.Enabled = true;
                        }
                        else
                        {
                            _clientNetwork.NetworkMessageQ.Enqueue("내 시간 초과");
                            _limitTime = StartTimeLimit;
                            turnLabel.Text = "상대차례";
                            putBtn.Enabled = false;
                            SelectClear(_selectedX, _selectedY);
                            _selectedX = -1;
                            _selectedY = -1;
                        }
                    }
                    break;

                case PacketId.NtfTimeOutWin:
                    {
                        var res = MemoryPackSerializer.Deserialize<NtfTimeOutWinPacket>(packet.Body);
                        MessageBox.Show($"시간 초과 누적으로 게임 종료!\n승자 : {res.Id}");
                        otherUserStateLabel.Text = "대기중";
                        stateTextLabel.Text = "대기중";
                        readyBtn.Text = "게임준비";
                        readyBtn.Enabled = true;
                        _clientNetwork.NetworkMessageQ.Enqueue("게임 종료");
                        limitTimer.Stop();
                        _state = USER_STATE.NONE;
                        BoardClear();
                        omokPanel.Refresh();
                        InitializeBoard();
                    }
                    break;

                default:
                    break;
            }
        }

        void PutStoneTimer(object sender, EventArgs e)
        {
            _limitTime--;

            if (_limitTime <= 0)
            {
                _limitTime = 0;
            }
            limitTimeLabel.Text = _limitTime.ToString();
        }

        private async void reqMatchingBtn_Click(object sender, EventArgs e)
        {
            if (_roomNumber != -1)
            {
                _clientNetwork.NetworkMessageQ.Enqueue("이미 방에 들어가있습니다.");
                return;
            }

            var client = new HttpClient();
            if (_isMatching)
            {
                if (MessageBox.Show("매칭을 취소하시겠습니까?", "매칭 취소", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var requestCancel = new ReqCancelMatching();
                    requestCancel.Id = _userInfo.Id;
                    var responseCancel = await client.PostAsJsonAsync("http://localhost:5292/api/CancelMatching/cancelMatching", requestCancel);

                    if (!responseCancel.IsSuccessStatusCode)
                    {
                        MessageBox.Show("매칭 취소 요청 실패.");
                    }

                    ResCancelMatching? resCancel = await responseCancel.Content.ReadFromJsonAsync<ResCancelMatching>();

                    if (resCancel.Result == ErrorCode.None)
                    {
                        reqMatchingBtn.Text = "매칭 요청";
                        _isMatching = false;
                        checkMatchingTimer.Stop();
                    }
                    MessageBox.Show("매칭을 취소했습니다.");
                    return;
                }
                else
                {
                    return;
                }
            }

            reqMatchingBtn.Text = "매칭중";

            var request = new ReqMatching();
            request.Id = _userInfo.Id;

            var response = await client.PostAsJsonAsync("http://10.192.8.223:5292/api/RequestMatching/matching", request);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("매칭 요청 실패. 상태 코드 : " + response.StatusCode);
                reqMatchingBtn.Text = "매칭 요청";
                return;
            }

            var res = await response.Content.ReadFromJsonAsync<ResMatching>();

            if (res.Result != ErrorCode.None)
            {
                MessageBox.Show("매칭 요청 실패.");
                reqMatchingBtn.Text = "매칭 요청";
                return;
            }

            _isMatching = true;
            checkMatchingTimer.Start();
        }

        byte[] MakeSendData(PacketId packetId, byte[] body)
        {
            var packetSize = (short)(PacketDefine.PacketHeader + body.Length);

            var sendData = new byte[packetSize];
            FastBinaryWrite.Int16(sendData, 0, packetSize);
            FastBinaryWrite.Int16(sendData, 2, (short)packetId);
            Array.Copy(body, 0, sendData, PacketDefine.PacketHeader, body.Length);

            return sendData;
        }

        private void chatSendBtn_Click(object sender, EventArgs e)
        {
            if (chatTextBox.Text == "")
            {
                return;
            }

            if (_roomNumber == -1)
            {
                _clientNetwork.NetworkMessageQ.Enqueue("방에 들어가있지 않습니다.");
                return;
            }

            chatRTB.AppendText($"{_userInfo.Id} : {chatTextBox.Text}\n");
            chatRTB.SelectionStart = chatRTB.Text.Length;
            chatRTB.ScrollToCaret();

            var req = new ReqChatPacket();
            req.Chat = chatTextBox.Text;
            chatTextBox.Clear();

            _sendQueue.Enqueue(MakeSendData(PacketId.ReqRoomChat, MemoryPackSerializer.Serialize(req)));
        }

        private void InitializeBoard()
        {
            _board = new STONE[19, 19];
            _pen = new Pen(Color.Black);
            _bBrush = new SolidBrush(Color.Black);
            _wBrush = new SolidBrush(Color.White);

            _graphics = omokPanel.CreateGraphics();

            // 세로선 19개
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                _graphics.DrawLine(_pen, new Point(MARGIN + i * GRID_SIZE, MARGIN),
                  new Point(MARGIN + i * GRID_SIZE, MARGIN + 18 * GRID_SIZE));
            }

            // 가로선 19개
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                _graphics.DrawLine(_pen, new Point(MARGIN, MARGIN + i * GRID_SIZE),
                  new Point(MARGIN + 18 * GRID_SIZE, MARGIN + i * GRID_SIZE));
            }

            // 화점그리기
            for (int x = 3; x <= 15; x += 6)
            {
                for (int y = 3; y <= 15; y += 6)
                {
                    _graphics.FillEllipse(_bBrush,
                    MARGIN + GRID_SIZE * x - FLOWER_DOT_SIZE / 2,
                      MARGIN + GRID_SIZE * y - FLOWER_DOT_SIZE / 2,
                      FLOWER_DOT_SIZE, FLOWER_DOT_SIZE);
                }
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (_state != USER_STATE.PLAYING)
                return;

            SelectClear(_selectedX, _selectedY);

            // e.X는 픽셀단위, x는 바둑판 좌표
            int x = (e.X - MARGIN + GRID_SIZE / 2) / GRID_SIZE;
            int y = (e.Y - MARGIN + GRID_SIZE / 2) / GRID_SIZE;

            if (_board[x, y] != STONE.NONE)
            {
                _selectedX = -1;
                _selectedY = -1;
                return;
            }

            Rectangle r = new Rectangle(
              MARGIN + GRID_SIZE * x - STONE_SIZE / 2,
              MARGIN + GRID_SIZE * y - STONE_SIZE / 2,
              STONE_SIZE, STONE_SIZE);

            _graphics.DrawRectangle(_pen, r);

            _selectedX = x;
            _selectedY = y;
        }

        void SelectClear(int posX, int posY)
        {
            if (posX != -1 && posY != -1)
            {
                Rectangle eraseR = new Rectangle(
                    MARGIN + GRID_SIZE * posX - STONE_SIZE / 2,
                    MARGIN + GRID_SIZE * posY - STONE_SIZE / 2,
                    STONE_SIZE, STONE_SIZE);

                _pen.Color = Color.SandyBrown;
                _graphics.DrawRectangle(_pen, eraseR);
            }
            _pen.Color = Color.Black;
            if (posX == 0 && posY == 0)
            {
                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE),
                  new Point(MARGIN + _selectedX * GRID_SIZE + 11, MARGIN + posY * GRID_SIZE));

                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE),
                  new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE + 11));
            }
            else if (posX == 0 && posY == 18)
            {
                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE),
                  new Point(MARGIN + _selectedX * GRID_SIZE + 11, MARGIN + posY * GRID_SIZE));

                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE - 11),
                  new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE));
            }
            else if (posX == 18 && posY == 0)
            {
                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE - 11, MARGIN + posY * GRID_SIZE),
                  new Point(MARGIN + _selectedX * GRID_SIZE, MARGIN + posY * GRID_SIZE));

                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE),
                  new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE + 11));
            }
            else if (posX == 18 && posY == 18)
            {
                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE - 11, MARGIN + posY * GRID_SIZE),
                  new Point(MARGIN + _selectedX * GRID_SIZE, MARGIN + posY * GRID_SIZE));

                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE - 11),
                  new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE));
            }
            else if (posX == 0)
            {
                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE),
                  new Point(MARGIN + _selectedX * GRID_SIZE + 11, MARGIN + posY * GRID_SIZE));

                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE - 11),
                  new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE + 11));
            }
            else if (posY == 0)
            {
                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE - 11, MARGIN + posY * GRID_SIZE),
                  new Point(MARGIN + _selectedX * GRID_SIZE + 11, MARGIN + posY * GRID_SIZE));

                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE),
                  new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE + 11));
            }
            else if (posX == 18)
            {
                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE - 11, MARGIN + posY * GRID_SIZE),
                  new Point(MARGIN + _selectedX * GRID_SIZE, MARGIN + posY * GRID_SIZE));

                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE - 11),
                  new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE + 11));
            }
            else if (posY == 18)
            {
                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE - 11, MARGIN + posY * GRID_SIZE),
                  new Point(MARGIN + _selectedX * GRID_SIZE + 11, MARGIN + posY * GRID_SIZE));

                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE - 11),
                  new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE));
            }
            else
            {
                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE - 11, MARGIN + posY * GRID_SIZE),
                  new Point(MARGIN + _selectedX * GRID_SIZE + 11, MARGIN + posY * GRID_SIZE));

                _graphics.DrawLine(_pen, new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE - 11),
                  new Point(MARGIN + posX * GRID_SIZE, MARGIN + posY * GRID_SIZE + 11));
            }
        }

        void DrawStone(STONE stone, int x, int y)
        {
            Rectangle r = new Rectangle(
              MARGIN + GRID_SIZE * x - STONE_SIZE / 2,
              MARGIN + GRID_SIZE * y - STONE_SIZE / 2,
              STONE_SIZE, STONE_SIZE);

            if (stone == STONE.BLACK)
            {
                _graphics.FillEllipse(_bBrush, r);
                _board[x, y] = STONE.BLACK;
            }
            else
            {
                _graphics.FillEllipse(_wBrush, r);
                _board[x, y] = STONE.WHITE;
            }
        }

        void BoardClear()
        {
            for (int x = 0; x < 19; x++)
            {
                for (int y = 0; y < 19; y++)
                {
                    _board[x, y] = STONE.NONE;
                }
            }
        }

        private void roomExitBtn_Click(object sender, EventArgs e)
        {
            if (_roomNumber == -1)
            {
                _clientNetwork.NetworkMessageQ.Enqueue("방에 들어가있지 않습니다.");
                return;
            }

            var req = new ReqLeaveRoomPacket();
            req.RoomNumber = _roomNumber;

            var body = MemoryPackSerializer.Serialize(req);

            _sendQueue.Enqueue(MakeSendData(PacketId.ReqLeaveRoom, body));
        }

        private void putBtn_Click(object sender, EventArgs e)
        {
            if (_selectedX == -1 & _selectedY == -1)
            {
                MessageBox.Show("놓을곳을 선택해주세요.");
                return;
            }

            var req = new ReqPutStonePacket();
            req.RoomNumber = _roomNumber;
            req.PosX = _selectedX;
            req.PosY = _selectedY;

            var body = MemoryPackSerializer.Serialize(req);

            _sendQueue.Enqueue(MakeSendData(PacketId.ReqPutStone, body));
            putBtn.Enabled = false;
        }

        private void readyBtn_Click(object sender, EventArgs e)
        {
            if (!_isNetworkRunning)
            {
                _clientNetwork.NetworkMessageQ.Enqueue("서버에 접속되어있지 않습니다.");
                return;
            }

            if (_state == USER_STATE.NONE)
            {
                var req = new ReqReadyPacket();
                req.RoomNumber = _roomNumber;

                var body = MemoryPackSerializer.Serialize(req);

                _sendQueue.Enqueue(MakeSendData(PacketId.ReqReady, body));
            }
            else if (_state == USER_STATE.READY)
            {
                var req = new ReqNotReadyPacket();
                req.RoomNumber = _roomNumber;

                var body = MemoryPackSerializer.Serialize(req);

                _sendQueue.Enqueue(MakeSendData(PacketId.ReqNotReady, body));
            }
        }

        void SendHeartBeat(object sender, EventArgs e)
        {
            if (!_isNetworkRunning)
            {
                return;
            }

            var dif = DateTime.Now - _serverHeartBeatTime;
            if (dif.TotalSeconds > ServerHeartBeatTimeLimit)
            {
                _clientNetwork.NetworkMessageQ.Enqueue("서버와의 접속이 끊어졌습니다.");
                _clientNetwork.Close();
            }

            var req = new ReqHeartBeatPacket();
            var body = MemoryPackSerializer.Serialize(req);

            _sendQueue.Enqueue(MakeSendData(PacketId.ReqHeartBeat, body));
        }

        void CheckMatching(object sender, EventArgs e)
        {
            if (!_isMatching)
            {
                return;
            }

            RequestCheckMatching();
        }

        async Task RequestCheckMatching()
        {
            var request = new ReqCheckMatching();
            request.Id = _userInfo.Id;

            var client = new HttpClient();
            var response = await client.PostAsJsonAsync("http://localhost:5292/api/CheckMatching/checkMatching", request);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("매칭 검사 실패. 상태 코드 : " + response.StatusCode);
                _isMatching = false;
                checkMatchingTimer.Stop();
                return;
            }

            var res = await response.Content.ReadFromJsonAsync<ResCheckMatching>();

            if (res.MatchResult != ErrorCode.None)
            {
                return;
            }

            InitializeBoard();
            Connect(res.ServerAddress, res.Port);
            roomNumberText.Text = res.RoomNumber.ToString();
            _roomNumber = res.RoomNumber;

            _clientNetwork.NetworkMessageQ.Enqueue($"매칭 결과 방번호 : {res.RoomNumber}");

            _isMatching = false;
            checkMatchingTimer.Stop();
            reqMatchingBtn.Text = "매칭 완료";
        }
    }
}
