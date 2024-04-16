using Omok.Model;
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

namespace Omok
{
    public partial class CreateAccountForm : Form
    {
        private Form1 mainForm;

        public CreateAccountForm(Form1 form)
        {
            InitializeComponent();
            mainForm = form;
        }

        private void CreateFormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.Enabled = true;
        }

        private void emailCheckBtn_Click(object sender, EventArgs e)
        {
            CheckEmail(emailTextBox.Text);
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

            if (res.Result)
            {
                MessageBox.Show("사용 가능한 이메일입니다.");
            }
            else
            {
                MessageBox.Show("사용 불가능한 이메일입니다.");
            }
        }

        private async Task CreateAccount(string userId, string password)
        {
            var client = new HttpClient();

            var model = new ReqCreateAccount
            {
                Email = userId,
                Password = password
            };

            var response = await client.PostAsJsonAsync("https://localhost:7200/api/CreateAccount/create", model);

            //if (response.IsSuccessStatusCode)
            //{
            //    richTextBox1.Text += "요청이 성공적으로 완료되었습니다.\n";
            //}
            //else
            //{
            //    richTextBox1.Text += "오류가 발생하였습니다. 상태 코드: " + response.StatusCode + "\n";
            //}
        }
    }
}
