namespace Omok
{
    partial class Form1
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
            loginBtn = new Button();
            emailTextBox = new TextBox();
            pwTextBox = new TextBox();
            emailLabel = new Label();
            passwordLabel = new Label();
            richTextBox1 = new RichTextBox();
            toCreateAccountFormBtn = new Button();
            testBtn = new Button();
            SuspendLayout();
            // 
            // loginBtn
            // 
            loginBtn.Location = new Point(210, 210);
            loginBtn.Margin = new Padding(4, 5, 4, 5);
            loginBtn.Name = "loginBtn";
            loginBtn.Size = new Size(107, 38);
            loginBtn.TabIndex = 0;
            loginBtn.Text = "로그인";
            loginBtn.UseVisualStyleBackColor = true;
            loginBtn.Click += loginBtn_Click;
            // 
            // emailTextBox
            // 
            emailTextBox.Location = new Point(210, 80);
            emailTextBox.Margin = new Padding(4, 5, 4, 5);
            emailTextBox.Name = "emailTextBox";
            emailTextBox.Size = new Size(243, 31);
            emailTextBox.TabIndex = 1;
            // 
            // pwTextBox
            // 
            pwTextBox.Location = new Point(210, 145);
            pwTextBox.Margin = new Padding(4, 5, 4, 5);
            pwTextBox.Name = "pwTextBox";
            pwTextBox.PasswordChar = '*';
            pwTextBox.Size = new Size(243, 31);
            pwTextBox.TabIndex = 2;
            // 
            // emailLabel
            // 
            emailLabel.AutoSize = true;
            emailLabel.Location = new Point(120, 80);
            emailLabel.Margin = new Padding(4, 0, 4, 0);
            emailLabel.Name = "emailLabel";
            emailLabel.Size = new Size(82, 25);
            emailLabel.TabIndex = 3;
            emailLabel.Text = "이메일 : ";
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new Point(101, 145);
            passwordLabel.Margin = new Padding(4, 0, 4, 0);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new Size(100, 25);
            passwordLabel.TabIndex = 4;
            passwordLabel.Text = "비밀번호 : ";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(101, 258);
            richTextBox1.Margin = new Padding(4, 5, 4, 5);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(368, 196);
            richTextBox1.TabIndex = 6;
            richTextBox1.Text = "";
            // 
            // toCreateAccountFormBtn
            // 
            toCreateAccountFormBtn.Location = new Point(343, 210);
            toCreateAccountFormBtn.Name = "toCreateAccountFormBtn";
            toCreateAccountFormBtn.Size = new Size(111, 38);
            toCreateAccountFormBtn.TabIndex = 7;
            toCreateAccountFormBtn.Text = "회원가입";
            toCreateAccountFormBtn.UseVisualStyleBackColor = true;
            toCreateAccountFormBtn.Click += toCreateAccountFormBtn_Click;
            // 
            // testBtn
            // 
            testBtn.Location = new Point(254, 24);
            testBtn.Name = "testBtn";
            testBtn.Size = new Size(156, 34);
            testBtn.TabIndex = 8;
            testBtn.Text = "테스트 연결";
            testBtn.UseVisualStyleBackColor = true;
            testBtn.Click += testBtn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(606, 505);
            Controls.Add(testBtn);
            Controls.Add(toCreateAccountFormBtn);
            Controls.Add(richTextBox1);
            Controls.Add(passwordLabel);
            Controls.Add(emailLabel);
            Controls.Add(pwTextBox);
            Controls.Add(emailTextBox);
            Controls.Add(loginBtn);
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button loginBtn;
        private TextBox emailTextBox;
        private TextBox pwTextBox;
        private Label emailLabel;
        private Label passwordLabel;
        private RichTextBox richTextBox1;
        private Button toCreateAccountFormBtn;
        private Button testBtn;
    }
}
