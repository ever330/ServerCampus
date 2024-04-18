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
        public CreateAccountForm()
        {
            InitializeComponent();
        }

        private async void createAccountBtn_Click(object sender, EventArgs e)
        {
            if (passwordTextBox.Text == passwordCheckTextBox.Text)
            {
                await CreateAccount(emailTextBox.Text, passwordTextBox.Text);
            }
            else
            {
                MessageBox.Show("비밀번호를 확인해주세요.");
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

            var request = new ReqCreateAccount
            {
                Email = email,
                Password = encryptPassword
            };

            var response = await client.PostAsJsonAsync("http://localhost:5229/api/CreateAccount/create", request);


            ResCreateAccount? res = await response.Content.ReadFromJsonAsync<ResCreateAccount>();

            if (res == null)
            {
                MessageBox.Show("계정 생성에 실패하였습니다.");
                return;
            }

            if (res.Result == ErrorCode.None)
            {
                MessageBox.Show("계정이 생성되었습니다.");
                this.Close();
            }
            else if (res.Result == ErrorCode.AccountAlreadyExist)
            {
                MessageBox.Show("이미 존재하는 계정입니다.");
            }
            else
            {
                MessageBox.Show("계정 생성에 실패하였습니다.");
            }
        }
    }
}
