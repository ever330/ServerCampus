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
            components = new System.ComponentModel.Container();
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
            netMessageRTB = new RichTextBox();
            enterRoomBtn = new Button();
            roomNumberLabel = new Label();
            roomNumberText = new Label();
            backGroundTimer = new System.Windows.Forms.Timer(components);
            loginBtn = new Button();
            chatTextBox = new TextBox();
            chatSendBtn = new Button();
            omokPanel = new TableLayoutPanel();
            readyBtn = new Button();
            chatRTB = new RichTextBox();
            stateLabel = new Label();
            stateTextLabel = new Label();
            otherUserLabel = new Label();
            otherUserTextLabel = new Label();
            SuspendLayout();
            // 
            // emailTextLabel
            // 
            emailTextLabel.AutoSize = true;
            emailTextLabel.Location = new Point(29, 8);
            emailTextLabel.Name = "emailTextLabel";
            emailTextLabel.Size = new Size(82, 25);
            emailTextLabel.TabIndex = 0;
            emailTextLabel.Text = "이메일 : ";
            // 
            // levelTextLabel
            // 
            levelTextLabel.AutoSize = true;
            levelTextLabel.Location = new Point(343, 8);
            levelTextLabel.Name = "levelTextLabel";
            levelTextLabel.Size = new Size(128, 25);
            levelTextLabel.TabIndex = 2;
            levelTextLabel.Text = "레벨(경험치) : ";
            // 
            // WinLoseTextLabel
            // 
            WinLoseTextLabel.AutoSize = true;
            WinLoseTextLabel.Location = new Point(588, 8);
            WinLoseTextLabel.Name = "WinLoseTextLabel";
            WinLoseTextLabel.Size = new Size(64, 25);
            WinLoseTextLabel.TabIndex = 4;
            WinLoseTextLabel.Text = "승률 : ";
            // 
            // attendanceBtn
            // 
            attendanceBtn.Location = new Point(862, 685);
            attendanceBtn.Margin = new Padding(4, 5, 4, 5);
            attendanceBtn.Name = "attendanceBtn";
            attendanceBtn.Size = new Size(107, 38);
            attendanceBtn.TabIndex = 6;
            attendanceBtn.Text = "출석체크";
            attendanceBtn.UseVisualStyleBackColor = true;
            attendanceBtn.Click += attendanceBtn_Click;
            // 
            // moneyTextLabel
            // 
            moneyTextLabel.AutoSize = true;
            moneyTextLabel.Location = new Point(18, 692);
            moneyTextLabel.Margin = new Padding(4, 0, 4, 0);
            moneyTextLabel.Name = "moneyTextLabel";
            moneyTextLabel.Size = new Size(106, 25);
            moneyTextLabel.TabIndex = 7;
            moneyTextLabel.Text = "보유 머니 : ";
            // 
            // moneyLabel
            // 
            moneyLabel.AutoSize = true;
            moneyLabel.Location = new Point(105, 692);
            moneyLabel.Margin = new Padding(4, 0, 4, 0);
            moneyLabel.Name = "moneyLabel";
            moneyLabel.Size = new Size(22, 25);
            moneyLabel.TabIndex = 8;
            moneyLabel.Text = "0";
            // 
            // emailLabel
            // 
            emailLabel.AutoSize = true;
            emailLabel.Location = new Point(113, 8);
            emailLabel.Margin = new Padding(4, 0, 4, 0);
            emailLabel.Name = "emailLabel";
            emailLabel.Size = new Size(55, 25);
            emailLabel.TabIndex = 9;
            emailLabel.Text = "email";
            // 
            // levelLabel
            // 
            levelLabel.AutoSize = true;
            levelLabel.Location = new Point(473, 8);
            levelLabel.Margin = new Padding(4, 0, 4, 0);
            levelLabel.Name = "levelLabel";
            levelLabel.Size = new Size(42, 25);
            levelLabel.TabIndex = 10;
            levelLabel.Text = "0(0)";
            // 
            // winLoseLabel
            // 
            winLoseLabel.AutoSize = true;
            winLoseLabel.Location = new Point(655, 8);
            winLoseLabel.Margin = new Padding(4, 0, 4, 0);
            winLoseLabel.Name = "winLoseLabel";
            winLoseLabel.Size = new Size(37, 25);
            winLoseLabel.TabIndex = 11;
            winLoseLabel.Text = "0%";
            // 
            // mailboxBtn
            // 
            mailboxBtn.Location = new Point(747, 685);
            mailboxBtn.Margin = new Padding(4, 5, 4, 5);
            mailboxBtn.Name = "mailboxBtn";
            mailboxBtn.Size = new Size(107, 38);
            mailboxBtn.TabIndex = 12;
            mailboxBtn.Text = "우편함";
            mailboxBtn.UseVisualStyleBackColor = true;
            mailboxBtn.Click += mailboxBtn_Click;
            // 
            // netMessageRTB
            // 
            netMessageRTB.Location = new Point(17, 115);
            netMessageRTB.Margin = new Padding(4, 5, 4, 5);
            netMessageRTB.Name = "netMessageRTB";
            netMessageRTB.Size = new Size(271, 137);
            netMessageRTB.TabIndex = 13;
            netMessageRTB.Text = "";
            // 
            // enterRoomBtn
            // 
            enterRoomBtn.Location = new Point(17, 67);
            enterRoomBtn.Margin = new Padding(4, 5, 4, 5);
            enterRoomBtn.Name = "enterRoomBtn";
            enterRoomBtn.Size = new Size(107, 38);
            enterRoomBtn.TabIndex = 14;
            enterRoomBtn.Text = "방 입장";
            enterRoomBtn.UseVisualStyleBackColor = true;
            enterRoomBtn.Click += enterRoomBtn_Click;
            // 
            // roomNumberLabel
            // 
            roomNumberLabel.AutoSize = true;
            roomNumberLabel.Location = new Point(133, 73);
            roomNumberLabel.Margin = new Padding(4, 0, 4, 0);
            roomNumberLabel.Name = "roomNumberLabel";
            roomNumberLabel.Size = new Size(82, 25);
            roomNumberLabel.TabIndex = 15;
            roomNumberLabel.Text = "방번호 : ";
            // 
            // roomNumberText
            // 
            roomNumberText.AutoSize = true;
            roomNumberText.Location = new Point(206, 73);
            roomNumberText.Margin = new Padding(4, 0, 4, 0);
            roomNumberText.Name = "roomNumberText";
            roomNumberText.Size = new Size(29, 25);
            roomNumberText.TabIndex = 16;
            roomNumberText.Text = "-1";
            // 
            // loginBtn
            // 
            loginBtn.Location = new Point(816, 8);
            loginBtn.Margin = new Padding(4, 5, 4, 5);
            loginBtn.Name = "loginBtn";
            loginBtn.Size = new Size(136, 38);
            loginBtn.TabIndex = 17;
            loginBtn.Text = "테스트 로그인";
            loginBtn.UseVisualStyleBackColor = true;
            loginBtn.Click += loginBtn_Click;
            // 
            // chatTextBox
            // 
            chatTextBox.Location = new Point(17, 513);
            chatTextBox.Margin = new Padding(4, 5, 4, 5);
            chatTextBox.Name = "chatTextBox";
            chatTextBox.Size = new Size(141, 31);
            chatTextBox.TabIndex = 18;
            // 
            // chatSendBtn
            // 
            chatSendBtn.Location = new Point(183, 509);
            chatSendBtn.Margin = new Padding(4, 5, 4, 5);
            chatSendBtn.Name = "chatSendBtn";
            chatSendBtn.Size = new Size(107, 38);
            chatSendBtn.TabIndex = 19;
            chatSendBtn.Text = "전송";
            chatSendBtn.UseVisualStyleBackColor = true;
            chatSendBtn.Click += chatSendBtn_Click;
            // 
            // omokPanel
            // 
            omokPanel.ColumnCount = 2;
            omokPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            omokPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            omokPanel.Location = new Point(343, 67);
            omokPanel.Name = "omokPanel";
            omokPanel.RowCount = 2;
            omokPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            omokPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            omokPanel.Size = new Size(600, 600);
            omokPanel.TabIndex = 20;
            omokPanel.MouseDown += panel1_MouseDown;
            // 
            // readyBtn
            // 
            readyBtn.Location = new Point(183, 567);
            readyBtn.Name = "readyBtn";
            readyBtn.Size = new Size(107, 34);
            readyBtn.TabIndex = 21;
            readyBtn.Text = "게임준비";
            readyBtn.UseVisualStyleBackColor = true;
            // 
            // chatRTB
            // 
            chatRTB.Location = new Point(17, 260);
            chatRTB.Name = "chatRTB";
            chatRTB.Size = new Size(271, 232);
            chatRTB.TabIndex = 22;
            chatRTB.Text = "";
            // 
            // stateLabel
            // 
            stateLabel.AutoSize = true;
            stateLabel.Location = new Point(17, 572);
            stateLabel.Name = "stateLabel";
            stateLabel.Size = new Size(64, 25);
            stateLabel.TabIndex = 23;
            stateLabel.Text = "상태 : ";
            // 
            // stateTextLabel
            // 
            stateTextLabel.AutoSize = true;
            stateTextLabel.Location = new Point(74, 572);
            stateTextLabel.Name = "stateTextLabel";
            stateTextLabel.Size = new Size(66, 25);
            stateTextLabel.TabIndex = 24;
            stateTextLabel.Text = "대기중";
            // 
            // otherUserLabel
            // 
            otherUserLabel.AutoSize = true;
            otherUserLabel.Location = new Point(17, 632);
            otherUserLabel.Name = "otherUserLabel";
            otherUserLabel.Size = new Size(82, 25);
            otherUserLabel.TabIndex = 25;
            otherUserLabel.Text = "상대방 : ";
            // 
            // otherUserTextLabel
            // 
            otherUserTextLabel.AutoSize = true;
            otherUserTextLabel.Location = new Point(98, 632);
            otherUserTextLabel.Name = "otherUserTextLabel";
            otherUserTextLabel.Size = new Size(57, 25);
            otherUserTextLabel.TabIndex = 26;
            otherUserTextLabel.Text = "None";
            // 
            // InGameForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1007, 742);
            Controls.Add(otherUserTextLabel);
            Controls.Add(otherUserLabel);
            Controls.Add(stateTextLabel);
            Controls.Add(stateLabel);
            Controls.Add(chatRTB);
            Controls.Add(readyBtn);
            Controls.Add(omokPanel);
            Controls.Add(chatSendBtn);
            Controls.Add(chatTextBox);
            Controls.Add(loginBtn);
            Controls.Add(roomNumberText);
            Controls.Add(roomNumberLabel);
            Controls.Add(enterRoomBtn);
            Controls.Add(netMessageRTB);
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
        private RichTextBox netMessageRTB;
        private Button enterRoomBtn;
        private Label roomNumberLabel;
        private Label roomNumberText;
        private System.Windows.Forms.Timer backGroundTimer;
        private Button loginBtn;
        private TextBox chatTextBox;
        private Button chatSendBtn;
        private TableLayoutPanel omokPanel;
        private Button readyBtn;
        private RichTextBox chatRTB;
        private Label stateLabel;
        private Label stateTextLabel;
        private Label otherUserLabel;
        private Label otherUserTextLabel;
    }
}