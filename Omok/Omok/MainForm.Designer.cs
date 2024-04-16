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
            SuspendLayout();
            // 
            // loginBtn
            // 
            loginBtn.Location = new Point(147, 126);
            loginBtn.Name = "loginBtn";
            loginBtn.Size = new Size(75, 23);
            loginBtn.TabIndex = 0;
            loginBtn.Text = "로그인";
            loginBtn.UseVisualStyleBackColor = true;
            loginBtn.Click += loginBtn_Click;
            // 
            // emailTextBox
            // 
            emailTextBox.Location = new Point(147, 48);
            emailTextBox.Name = "emailTextBox";
            emailTextBox.Size = new Size(171, 23);
            emailTextBox.TabIndex = 1;
            // 
            // pwTextBox
            // 
            pwTextBox.Location = new Point(147, 87);
            pwTextBox.Name = "pwTextBox";
            pwTextBox.PasswordChar = '*';
            pwTextBox.Size = new Size(171, 23);
            pwTextBox.TabIndex = 2;
            // 
            // emailLabel
            // 
            emailLabel.AutoSize = true;
            emailLabel.Location = new Point(84, 48);
            emailLabel.Name = "emailLabel";
            emailLabel.Size = new Size(54, 15);
            emailLabel.TabIndex = 3;
            emailLabel.Text = "이메일 : ";
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new Point(71, 87);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new Size(66, 15);
            passwordLabel.TabIndex = 4;
            passwordLabel.Text = "비밀번호 : ";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(71, 155);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(259, 119);
            richTextBox1.TabIndex = 6;
            richTextBox1.Text = "";
            // 
            // toCreateAccountFormBtn
            // 
            toCreateAccountFormBtn.Location = new Point(240, 126);
            toCreateAccountFormBtn.Margin = new Padding(2);
            toCreateAccountFormBtn.Name = "toCreateAccountFormBtn";
            toCreateAccountFormBtn.Size = new Size(78, 23);
            toCreateAccountFormBtn.TabIndex = 7;
            toCreateAccountFormBtn.Text = "회원가입";
            toCreateAccountFormBtn.UseVisualStyleBackColor = true;
            toCreateAccountFormBtn.Click += toCreateAccountFormBtn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(424, 303);
            Controls.Add(toCreateAccountFormBtn);
            Controls.Add(richTextBox1);
            Controls.Add(passwordLabel);
            Controls.Add(emailLabel);
            Controls.Add(pwTextBox);
            Controls.Add(emailTextBox);
            Controls.Add(loginBtn);
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
    }
}
