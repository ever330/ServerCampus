using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Omok
{
    public partial class MailBoxForm : Form
    {
        private List<Mail> _mails = new List<Mail>();

        public MailBoxForm()
        {
            InitializeComponent();
        }

        public void MailSetting(List<Mail> mails)
        {
            _mails = mails;
            for (int i = 0; i < mails.Count; i++)
            {
                mailListBox.Items.Add(mails[i]);
            }
        }

        private void mailListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mailListBox.Items.Count == 0)
                return;

            // ListBox의 선택된 인덱스가 변경될 때 호출할 함수
            var tempMail = mailListBox.SelectedItem as Mail;

            mailnameTextBox.Text = tempMail.MailName;
            mailContentTextBox.Text = tempMail.MailContent;
            rewardTextBox.Text = "머니 : " + tempMail.Reward;
        }

        private void getBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
