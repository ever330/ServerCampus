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
            roomExitBtn = new Button();
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
            WinLoseTextLabel.Location = new Point(412, 5);
            WinLoseTextLabel.Margin = new Padding(2, 0, 2, 0);
            WinLoseTextLabel.Name = "WinLoseTextLabel";
            WinLoseTextLabel.Size = new Size(42, 15);
            WinLoseTextLabel.TabIndex = 4;
            WinLoseTextLabel.Text = "승률 : ";
            // 
            // attendanceBtn
            // 
            attendanceBtn.Location = new Point(603, 411);
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
            moneyTextLabel.Location = new Point(13, 415);
            moneyTextLabel.Name = "moneyTextLabel";
            moneyTextLabel.Size = new Size(70, 15);
            moneyTextLabel.TabIndex = 7;
            moneyTextLabel.Text = "보유 머니 : ";
            // 
            // moneyLabel
            // 
            moneyLabel.AutoSize = true;
            moneyLabel.Location = new Point(74, 415);
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
            winLoseLabel.Location = new Point(458, 5);
            winLoseLabel.Name = "winLoseLabel";
            winLoseLabel.Size = new Size(24, 15);
            winLoseLabel.TabIndex = 11;
            winLoseLabel.Text = "0%";
            // 
            // mailboxBtn
            // 
            mailboxBtn.Location = new Point(523, 411);
            mailboxBtn.Name = "mailboxBtn";
            mailboxBtn.Size = new Size(75, 23);
            mailboxBtn.TabIndex = 12;
            mailboxBtn.Text = "우편함";
            mailboxBtn.UseVisualStyleBackColor = true;
            mailboxBtn.Click += mailboxBtn_Click;
            // 
            // netMessageRTB
            // 
            netMessageRTB.Location = new Point(12, 69);
            netMessageRTB.Name = "netMessageRTB";
            netMessageRTB.Size = new Size(191, 84);
            netMessageRTB.TabIndex = 13;
            netMessageRTB.Text = "";
            // 
            // enterRoomBtn
            // 
            enterRoomBtn.Location = new Point(12, 40);
            enterRoomBtn.Name = "enterRoomBtn";
            enterRoomBtn.Size = new Size(75, 23);
            enterRoomBtn.TabIndex = 14;
            enterRoomBtn.Text = "방 입장";
            enterRoomBtn.UseVisualStyleBackColor = true;
            enterRoomBtn.Click += enterRoomBtn_Click;
            // 
            // roomNumberLabel
            // 
            roomNumberLabel.AutoSize = true;
            roomNumberLabel.Location = new Point(93, 44);
            roomNumberLabel.Name = "roomNumberLabel";
            roomNumberLabel.Size = new Size(54, 15);
            roomNumberLabel.TabIndex = 15;
            roomNumberLabel.Text = "방번호 : ";
            // 
            // roomNumberText
            // 
            roomNumberText.AutoSize = true;
            roomNumberText.Location = new Point(144, 44);
            roomNumberText.Name = "roomNumberText";
            roomNumberText.Size = new Size(19, 15);
            roomNumberText.TabIndex = 16;
            roomNumberText.Text = "-1";
            // 
            // loginBtn
            // 
            loginBtn.Location = new Point(571, 5);
            loginBtn.Name = "loginBtn";
            loginBtn.Size = new Size(95, 23);
            loginBtn.TabIndex = 17;
            loginBtn.Text = "테스트 로그인";
            loginBtn.UseVisualStyleBackColor = true;
            loginBtn.Click += loginBtn_Click;
            // 
            // chatTextBox
            // 
            chatTextBox.Location = new Point(12, 308);
            chatTextBox.Name = "chatTextBox";
            chatTextBox.Size = new Size(100, 23);
            chatTextBox.TabIndex = 18;
            // 
            // chatSendBtn
            // 
            chatSendBtn.Location = new Point(128, 305);
            chatSendBtn.Name = "chatSendBtn";
            chatSendBtn.Size = new Size(75, 23);
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
            omokPanel.Location = new Point(240, 38);
            omokPanel.Margin = new Padding(2);
            omokPanel.Name = "omokPanel";
            omokPanel.RowCount = 2;
            omokPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            omokPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            omokPanel.Size = new Size(420, 360);
            omokPanel.TabIndex = 20;
            omokPanel.MouseDown += panel1_MouseDown;
            // 
            // readyBtn
            // 
            readyBtn.Location = new Point(128, 338);
            readyBtn.Margin = new Padding(2);
            readyBtn.Name = "readyBtn";
            readyBtn.Size = new Size(75, 25);
            readyBtn.TabIndex = 21;
            readyBtn.Text = "게임준비";
            readyBtn.UseVisualStyleBackColor = true;
            // 
            // chatRTB
            // 
            chatRTB.Location = new Point(12, 156);
            chatRTB.Margin = new Padding(2);
            chatRTB.Name = "chatRTB";
            chatRTB.Size = new Size(191, 141);
            chatRTB.TabIndex = 22;
            chatRTB.Text = "";
            // 
            // stateLabel
            // 
            stateLabel.AutoSize = true;
            stateLabel.Location = new Point(12, 343);
            stateLabel.Margin = new Padding(2, 0, 2, 0);
            stateLabel.Name = "stateLabel";
            stateLabel.Size = new Size(42, 15);
            stateLabel.TabIndex = 23;
            stateLabel.Text = "상태 : ";
            // 
            // stateTextLabel
            // 
            stateTextLabel.AutoSize = true;
            stateTextLabel.Location = new Point(52, 343);
            stateTextLabel.Margin = new Padding(2, 0, 2, 0);
            stateTextLabel.Name = "stateTextLabel";
            stateTextLabel.Size = new Size(43, 15);
            stateTextLabel.TabIndex = 24;
            stateTextLabel.Text = "대기중";
            // 
            // otherUserLabel
            // 
            otherUserLabel.AutoSize = true;
            otherUserLabel.Location = new Point(12, 379);
            otherUserLabel.Margin = new Padding(2, 0, 2, 0);
            otherUserLabel.Name = "otherUserLabel";
            otherUserLabel.Size = new Size(54, 15);
            otherUserLabel.TabIndex = 25;
            otherUserLabel.Text = "상대방 : ";
            // 
            // otherUserTextLabel
            // 
            otherUserTextLabel.AutoSize = true;
            otherUserTextLabel.Location = new Point(69, 379);
            otherUserTextLabel.Margin = new Padding(2, 0, 2, 0);
            otherUserTextLabel.Name = "otherUserTextLabel";
            otherUserTextLabel.Size = new Size(36, 15);
            otherUserTextLabel.TabIndex = 26;
            otherUserTextLabel.Text = "None";
            // 
            // roomExitBtn
            // 
            roomExitBtn.Location = new Point(128, 375);
            roomExitBtn.Name = "roomExitBtn";
            roomExitBtn.Size = new Size(75, 23);
            roomExitBtn.TabIndex = 27;
            roomExitBtn.Text = "퇴장";
            roomExitBtn.UseVisualStyleBackColor = true;
            roomExitBtn.Click += roomExitBtn_Click;
            // 
            // InGameForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(705, 445);
            Controls.Add(roomExitBtn);
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
        private Button roomExitBtn;
    }
}