using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Security.Cryptography;
using Omok.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Omok
{
    public partial class CreateAccountForm : Form
    {
        private Form1 _mainForm;

        public CreateAccountForm(Form1 form)
        {
            InitializeComponent();
            _mainForm = form;
        }

        private async void emailCheckBtn_Click(object sender, EventArgs e)
        {
            bool valid = Regex.IsMatch(emailTextBox.Text, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");

            if (valid)
            {
                await CheckEmail(emailTextBox.Text);
            }
            else
            {
                MessageBox.Show("이메일 형식이 잘못되었습니다.");
            }
        }

        private async Task CheckEmail(string email)
        {
            var client = new HttpClient();

            var model = new ReqCheckEmail
            {
                Email = email
            };

            var response = await client.PostAsJsonAsync("https://localhost:44349/api/EmailCheck/check", model);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("오류가 발생하였습니다. 상태 코드: " + response.StatusCode);
            }

            ResCheckEmail res = await response.Content.ReadFromJsonAsync<ResCheckEmail>();

            if (res.Result == ErrorCode.None)
            {
                MessageBox.Show("사용 가능한 이메일입니다.");
            }
            else
            {
                MessageBox.Show("사용 불가능한 이메일입니다.");
            }
        }

        private async void createAccountBtn_Click(object sender, EventArgs e)
        {
            if (passwordTextBox.Text == passwordCheckTextBox.Text)
            {
                await CreateAccount(emailTextBox.Text, passwordTextBox.Text);
            }
            else
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다.");
            }
        }

        private async Task CreateAccount(string email, string password)
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

            var model = new ReqCreateAccount
            {
                Email = email,
                Password = encryptPassword
            };

            var response = await client.PostAsJsonAsync("https://localhost:44349/api/CreateAccount/create", model);


            ResCreateAccount res = await response.Content.ReadFromJsonAsync<ResCreateAccount>();

            if (res.Result == ErrorCode.None)
            {
                MessageBox.Show("계정이 생성되었습니다.");
                this.Close();
            }
            else
            {
                MessageBox.Show("계정 생성에 실패하였습니다.");
            }
        }
    }
}
