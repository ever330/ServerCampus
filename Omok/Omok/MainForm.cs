using System.Text.Json;
using System.Text;
using System.Web;
using System.Net.Http.Json;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using Omok.Model;

namespace Omok
{
    public partial class Form1 : Form
    {
        private CreateAccountForm _createAccountForm;

        public Form1()
        {
            InitializeComponent();
            _createAccountForm = new CreateAccountForm(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            string userId = idTextBox.Text;
            string password = pwTextBox.Text;

        }

        async Task CreateAccount(string userId, string password)
        {
            var client = new HttpClient();

            var model = new ReqCreateAccount
            {
                Email = userId,
                Password = password
            };

            var response = await client.PostAsJsonAsync("https://localhost:7200/api/CreateAccount/create", model);

            if (response.IsSuccessStatusCode)
            {
                richTextBox1.Text += "요청이 성공적으로 완료되었습니다.\n";
            }
            else
            {
                richTextBox1.Text += "오류가 발생하였습니다. 상태 코드: " + response.StatusCode + "\n";
            }
        }

        private void toCreateAccountFormBtn_Click(object sender, EventArgs e)
        {
            _createAccountForm.ShowDialog();
        }
    }
}
