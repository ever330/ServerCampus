namespace EchoClient
{
    partial class ClientForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ipTextBox = new TextBox();
            ipLabel = new Label();
            portLabel = new Label();
            portTextBox = new TextBox();
            connectBtn = new Button();
            label1 = new Label();
            sendTextBox = new TextBox();
            sendBtn = new Button();
            logRichTextBox = new RichTextBox();
            disconnnectBtn = new Button();
            processTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // ipTextBox
            // 
            ipTextBox.Location = new Point(38, 12);
            ipTextBox.Name = "ipTextBox";
            ipTextBox.Size = new Size(126, 23);
            ipTextBox.TabIndex = 0;
            // 
            // ipLabel
            // 
            ipLabel.AutoSize = true;
            ipLabel.Location = new Point(4, 15);
            ipLabel.Name = "ipLabel";
            ipLabel.Size = new Size(28, 15);
            ipLabel.TabIndex = 1;
            ipLabel.Text = "IP : ";
            // 
            // portLabel
            // 
            portLabel.AutoSize = true;
            portLabel.Location = new Point(170, 15);
            portLabel.Name = "portLabel";
            portLabel.Size = new Size(40, 15);
            portLabel.TabIndex = 2;
            portLabel.Text = "Port : ";
            // 
            // portTextBox
            // 
            portTextBox.Location = new Point(207, 13);
            portTextBox.Name = "portTextBox";
            portTextBox.Size = new Size(84, 23);
            portTextBox.TabIndex = 3;
            // 
            // connectBtn
            // 
            connectBtn.Location = new Point(297, 12);
            connectBtn.Name = "connectBtn";
            connectBtn.Size = new Size(57, 23);
            connectBtn.TabIndex = 4;
            connectBtn.Text = "접속";
            connectBtn.UseVisualStyleBackColor = true;
            connectBtn.Click += connectBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(4, 52);
            label1.Name = "label1";
            label1.Size = new Size(70, 15);
            label1.TabIndex = 5;
            label1.Text = "전송 내용 : ";
            // 
            // sendTextBox
            // 
            sendTextBox.Location = new Point(80, 49);
            sendTextBox.Name = "sendTextBox";
            sendTextBox.Size = new Size(231, 23);
            sendTextBox.TabIndex = 6;
            // 
            // sendBtn
            // 
            sendBtn.Location = new Point(327, 49);
            sendBtn.Name = "sendBtn";
            sendBtn.Size = new Size(87, 23);
            sendBtn.TabIndex = 7;
            sendBtn.Text = "전송하기";
            sendBtn.UseVisualStyleBackColor = true;
            sendBtn.Click += sendBtn_Click;
            // 
            // logRichTextBox
            // 
            logRichTextBox.Location = new Point(4, 92);
            logRichTextBox.Name = "logRichTextBox";
            logRichTextBox.Size = new Size(410, 184);
            logRichTextBox.TabIndex = 8;
            logRichTextBox.Text = "";
            // 
            // disconnnectBtn
            // 
            disconnnectBtn.Location = new Point(357, 11);
            disconnnectBtn.Name = "disconnnectBtn";
            disconnnectBtn.Size = new Size(57, 23);
            disconnnectBtn.TabIndex = 9;
            disconnnectBtn.Text = "해제";
            disconnnectBtn.UseVisualStyleBackColor = true;
            disconnnectBtn.Click += disconnectBtn_Click;
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(426, 297);
            Controls.Add(disconnnectBtn);
            Controls.Add(logRichTextBox);
            Controls.Add(sendBtn);
            Controls.Add(sendTextBox);
            Controls.Add(label1);
            Controls.Add(connectBtn);
            Controls.Add(portTextBox);
            Controls.Add(portLabel);
            Controls.Add(ipLabel);
            Controls.Add(ipTextBox);
            Name = "ClientForm";
            Text = "EchoClient";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox ipTextBox;
        private Label ipLabel;
        private Label portLabel;
        private TextBox portTextBox;
        private Button connectBtn;
        private Label label1;
        private TextBox sendTextBox;
        private Button sendBtn;
        private RichTextBox logRichTextBox;
        private Button disconnnectBtn;
        private System.Windows.Forms.Timer processTimer;
    }
}
