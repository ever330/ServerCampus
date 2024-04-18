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
                MessageBox.Show("�̸��� ������ �߸��Ǿ����ϴ�.");
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
                richTextBox1.Text += "Hive �α��� ��û�� ���������� �Ϸ�Ǿ����ϴ�.\n";
            }
            else
            {
                richTextBox1.Text += "Hive �α��� ������ �߻��Ͽ����ϴ�. ���� �ڵ�: " + response.StatusCode + "\n";
            }

            ResLoginToHive? res = await response.Content.ReadFromJsonAsync<ResLoginToHive>();

            if (res == null)
            {
                richTextBox1.Text += "�α��� ����\n";
                return;
            }

            if (res.Result == ErrorCode.None)
            {
                richTextBox1.Text += "�α����� �����Ͽ����ϴ�.\n";
                richTextBox1.Text += "��ū ���� : " + res.AuthToken + "\n";
                _myAuthToken = res.AuthToken;
                _myEmail = email;

                await TryLoginToGameServer();
            }
            else if (res.Result == ErrorCode.LoginError)
            {
                MessageBox.Show("�α��� ����");
            }
            else if (res.Result == ErrorCode.AccountNotExist)
            {
                MessageBox.Show("�������� �ʴ� �����Դϴ�.");
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
                richTextBox1.Text += "���Ӽ��� �α��� ��û�� ���������� �Ϸ�Ǿ����ϴ�.\n";
            }
            else
            {
                MessageBox.Show("���Ӽ��� �α��� ������ �߻��Ͽ����ϴ�. ���� �ڵ� : " + response.StatusCode);
            }

            ResLoginToGame? res = await response.Content.ReadFromJsonAsync<ResLoginToGame>();

            if (res == null || res.GameData == null)
            {
                MessageBox.Show("���Ӽ��� �α��� ����. ���� �ڵ� : " + response.StatusCode);
                return;
            }

            if (res.Result == ErrorCode.None)
            {
                richTextBox1.Text += "���Ӽ��� �α����� �Ϸ�Ǿ����ϴ�.\n";
                _inGameForm.SetInGameData(_myEmail, res.GameData);
                _inGameForm.ShowDialog();
            }
        }
    }
}
