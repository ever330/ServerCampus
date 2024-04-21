namespace Omok
{
    partial class MailBoxForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mailListBox = new ListBox();
            mailnameTextBox = new TextBox();
            mailContentTextBox = new RichTextBox();
            rewardTextBox = new TextBox();
            getBtn = new Button();
            SuspendLayout();
            // 
            // mailListBox
            // 
            mailListBox.FormattingEnabled = true;
            mailListBox.ItemHeight = 15;
            mailListBox.Location = new Point(12, 12);
            mailListBox.Name = "mailListBox";
            mailListBox.Size = new Size(108, 229);
            mailListBox.TabIndex = 0;
            mailListBox.SelectedIndexChanged += mailListBox_SelectedIndexChanged;
            // 
            // mailnameTextBox
            // 
            mailnameTextBox.Location = new Point(126, 12);
            mailnameTextBox.Name = "mailnameTextBox";
            mailnameTextBox.ReadOnly = true;
            mailnameTextBox.Size = new Size(180, 23);
            mailnameTextBox.TabIndex = 1;
            // 
            // mailContentTextBox
            // 
            mailContentTextBox.Location = new Point(126, 41);
            mailContentTextBox.Name = "mailContentTextBox";
            mailContentTextBox.ReadOnly = true;
            mailContentTextBox.Size = new Size(180, 166);
            mailContentTextBox.TabIndex = 2;
            mailContentTextBox.Text = "";
            // 
            // rewardTextBox
            // 
            rewardTextBox.Location = new Point(126, 218);
            rewardTextBox.Name = "rewardTextBox";
            rewardTextBox.ReadOnly = true;
            rewardTextBox.Size = new Size(100, 23);
            rewardTextBox.TabIndex = 3;
            // 
            // getBtn
            // 
            getBtn.Location = new Point(231, 218);
            getBtn.Name = "getBtn";
            getBtn.Size = new Size(75, 23);
            getBtn.TabIndex = 4;
            getBtn.Text = "수령하기";
            getBtn.UseVisualStyleBackColor = true;
            getBtn.Click += getBtn_Click;
            // 
            // MailBoxForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(318, 292);
            Controls.Add(getBtn);
            Controls.Add(rewardTextBox);
            Controls.Add(mailContentTextBox);
            Controls.Add(mailnameTextBox);
            Controls.Add(mailListBox);
            Name = "MailBoxForm";
            Text = "MailBoxForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox mailListBox;
        private TextBox mailnameTextBox;
        private RichTextBox mailContentTextBox;
        private TextBox rewardTextBox;
        private Button getBtn;
    }
}