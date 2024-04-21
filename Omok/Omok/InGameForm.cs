using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Omok.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Omok
{
    public partial class InGameForm : Form
    {
        private string _email = null!;
        private string _authToken = null!;
        private int _level;
        private int _exp;
        private int _winCount;
        private int _loseCount;
        private int _money;

        private MailBoxForm _mailBoxForm;

        public InGameForm()
        {
            InitializeComponent();

            _mailBoxForm = new MailBoxForm();
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
            moneyLabel.Text = "d";
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
    }
}
