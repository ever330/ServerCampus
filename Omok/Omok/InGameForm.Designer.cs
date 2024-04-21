namespace Omok
{
    partial class InGameForm
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
            emailTextLabel = new Label();
            levelTextLabel = new Label();
            WinLoseTextLabel = new Label();
            attendanceBtn = new Button();
            moneyTextLabel = new Label();
            moneyLabel = new Label();
            emailLabel = new Label();
            levelLabel = new Label();
            winLoseLabel = new Label();
            mailboxBtn = new Button();
            SuspendLayout();
            // 
            // emailTextLabel
            // 
            emailTextLabel.AutoSize = true;
            emailTextLabel.Location = new Point(20, 5);
            emailTextLabel.Margin = new Padding(2, 0, 2, 0);
            emailTextLabel.Name = "emailTextLabel";
            emailTextLabel.Size = new Size(54, 15);
            emailTextLabel.TabIndex = 0;
            emailTextLabel.Text = "이메일 : ";
            // 
            // levelTextLabel
            // 
            levelTextLabel.AutoSize = true;
            levelTextLabel.Location = new Point(240, 5);
            levelTextLabel.Margin = new Padding(2, 0, 2, 0);
            levelTextLabel.Name = "levelTextLabel";
            levelTextLabel.Size = new Size(86, 15);
            levelTextLabel.TabIndex = 2;
            levelTextLabel.Text = "레벨(경험치) : ";
            // 
            // WinLoseTextLabel
            // 
            WinLoseTextLabel.AutoSize = true;
            WinLoseTextLabel.Location = new Point(396, 5);
            WinLoseTextLabel.Margin = new Padding(2, 0, 2, 0);
            WinLoseTextLabel.Name = "WinLoseTextLabel";
            WinLoseTextLabel.Size = new Size(42, 15);
            WinLoseTextLabel.TabIndex = 4;
            WinLoseTextLabel.Text = "승률 : ";
            // 
            // attendanceBtn
            // 
            attendanceBtn.Location = new Point(473, 235);
            attendanceBtn.Name = "attendanceBtn";
            attendanceBtn.Size = new Size(75, 23);
            attendanceBtn.TabIndex = 6;
            attendanceBtn.Text = "출석체크";
            attendanceBtn.UseVisualStyleBackColor = true;
            attendanceBtn.Click += attendanceBtn_Click;
            // 
            // moneyTextLabel
            // 
            moneyTextLabel.AutoSize = true;
            moneyTextLabel.Location = new Point(20, 239);
            moneyTextLabel.Name = "moneyTextLabel";
            moneyTextLabel.Size = new Size(70, 15);
            moneyTextLabel.TabIndex = 7;
            moneyTextLabel.Text = "보유 머니 : ";
            // 
            // moneyLabel
            // 
            moneyLabel.AutoSize = true;
            moneyLabel.Location = new Point(81, 239);
            moneyLabel.Name = "moneyLabel";
            moneyLabel.Size = new Size(14, 15);
            moneyLabel.TabIndex = 8;
            moneyLabel.Text = "0";
            // 
            // emailLabel
            // 
            emailLabel.AutoSize = true;
            emailLabel.Location = new Point(79, 5);
            emailLabel.Name = "emailLabel";
            emailLabel.Size = new Size(36, 15);
            emailLabel.TabIndex = 9;
            emailLabel.Text = "email";
            // 
            // levelLabel
            // 
            levelLabel.AutoSize = true;
            levelLabel.Location = new Point(331, 5);
            levelLabel.Name = "levelLabel";
            levelLabel.Size = new Size(29, 15);
            levelLabel.TabIndex = 10;
            levelLabel.Text = "0(0)";
            // 
            // winLoseLabel
            // 
            winLoseLabel.AutoSize = true;
            winLoseLabel.Location = new Point(443, 5);
            winLoseLabel.Name = "winLoseLabel";
            winLoseLabel.Size = new Size(24, 15);
            winLoseLabel.TabIndex = 11;
            winLoseLabel.Text = "0%";
            // 
            // mailboxBtn
            // 
            mailboxBtn.Location = new Point(392, 235);
            mailboxBtn.Name = "mailboxBtn";
            mailboxBtn.Size = new Size(75, 23);
            mailboxBtn.TabIndex = 12;
            mailboxBtn.Text = "우편함";
            mailboxBtn.UseVisualStyleBackColor = true;
            mailboxBtn.Click += mailboxBtn_Click;
            // 
            // InGameForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(560, 270);
            Controls.Add(mailboxBtn);
            Controls.Add(winLoseLabel);
            Controls.Add(levelLabel);
            Controls.Add(emailLabel);
            Controls.Add(moneyLabel);
            Controls.Add(moneyTextLabel);
            Controls.Add(attendanceBtn);
            Controls.Add(WinLoseTextLabel);
            Controls.Add(levelTextLabel);
            Controls.Add(emailTextLabel);
            Margin = new Padding(2);
            Name = "InGameForm";
            Text = "InGameForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label emailTextLabel;
        private Label levelTextLabel;
        private Label WinLoseTextLabel;
        private Button attendanceBtn;
        private Label moneyTextLabel;
        private Label moneyLabel;
        private Label emailLabel;
        private Label levelLabel;
        private Label winLoseLabel;
        private Button mailboxBtn;
    }
}