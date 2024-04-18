using System.Text.Json;
using System.Text;
using System.Web;
using System.Net.Http.Json;
using System.Security.Cryptography;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using Omok.Models;
using Omok.Model;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Omok
{
    public partial class Form1 : Form
    {
        private CreateAccountForm _createAccountForm;
        private InGameForm _inGameForm;

        private string _myEmail;
        private string _myAuthToken;

        public Form1()
        {
            InitializeComponent();
            _createAccountForm = new CreateAccountForm();
            _inGameForm = new InGameForm();

            _myEmail = "";
            _myAuthToken = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void toCreateAccountFormBtn_Click(object sender, EventArgs e)
        {
            _createAccountForm.ShowDialog();
        }

        private async void loginBtn_Click(object sender, EventArgs e)
        {
            string email = emailTextBox.Text;
            string password = pwTextBox.Text;

            bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");

            if (valid)
            {
                await TryLoginToHive(email, password);
            }
            else
            {
                MessageBox.Show("이메일 형식이 잘못되었습니다.");
            }
        }

        private async Task TryLoginToHive(string email, string password)
        {
            var client = new HttpClient();

            string encryptPassword;
            using (SHA256 sha256 = SHA256.Create())
            {
                StringBuilder hash = new StringBuilder();
                byte[] hashArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                foreach (byte b in hashArray)
                {
                    hash.Append(b.ToString("x"));
                }
                encryptPassword = hash.ToString();
            }

            var request = new ReqLoginToHive
            {
                Email = email,
                Password = encryptPassword
            };

            var response = await client.PostAsJsonAsync("http://localhost:5229/api/Login/login", request);

            if (response.IsSuccessStatusCode)
            {
                richTextBox1.Text += "Hive 로그인 요청이 성공적으로 완료되었습니다.\n";
            }
            else
            {
                richTextBox1.Text += "Hive 로그인 오류가 발생하였습니다. 상태 코드: " + response.StatusCode + "\n";
            }

            ResLoginToHive? res = await response.Content.ReadFromJsonAsync<ResLoginToHive>();

            if (res == null)
            {
                richTextBox1.Text += "로그인 실패\n";
                return;
            }

            if (res.Result == ErrorCode.None)
            {
                richTextBox1.Text += "로그인을 성공하였습니다.\n";
                richTextBox1.Text += "토큰 정보 : " + res.AuthToken + "\n";
                _myAuthToken = res.AuthToken;
                _myEmail = email;

                await TryLoginToGameServer();
            }
            else if (res.Result == ErrorCode.LoginError)
            {
                MessageBox.Show("로그인 실패");
            }
            else if (res.Result == ErrorCode.AccountNotExist)
            {
                MessageBox.Show("존재하지 않는 계정입니다.");
            }
        }

        private async Task TryLoginToGameServer()
        {
            var client = new HttpClient();

            var request = new ReqLoginToGame
            {
                Email = _myEmail,
                AuthToken = _myAuthToken
            };

            var response = await client.PostAsJsonAsync("https://localhost:44363/api/Login/login", request);

            if (response.IsSuccessStatusCode)
            {
                richTextBox1.Text += "게임서버 로그인 요청이 성공적으로 완료되었습니다.\n";
            }
            else
            {
                MessageBox.Show("게임서버 로그인 오류가 발생하였습니다. 상태 코드 : " + response.StatusCode);
            }

            ResLoginToGame? res = await response.Content.ReadFromJsonAsync<ResLoginToGame>();

            if (res == null || res.GameData == null)
            {
                MessageBox.Show("게임서버 로그인 실패. 상태 코드 : " + response.StatusCode);
                return;
            }

            if (res.Result == ErrorCode.None)
            {
                richTextBox1.Text += "게임서버 로그인이 완료되었습니다.\n";
                _inGameForm.SetInGameData(_myEmail, res.GameData);
                _inGameForm.ShowDialog();
            }
        }
    }
}
