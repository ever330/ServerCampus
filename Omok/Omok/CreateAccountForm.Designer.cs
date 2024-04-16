namespace Omok
{
    partial class CreateAccountForm
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
            emailLabel = new Label();
            passwordLabel = new Label();
            emailTextBox = new TextBox();
            passwordTextBox = new TextBox();
            passwordCheckTextBox = new TextBox();
            passwordCheckLabel = new Label();
            emailCheckBtn = new Button();
            createAccountBtn = new Button();
            SuspendLayout();
            // 
            // emailLabel
            // 
            emailLabel.AutoSize = true;
            emailLabel.Location = new Point(109, 75);
            emailLabel.Name = "emailLabel";
            emailLabel.Size = new Size(82, 25);
            emailLabel.TabIndex = 0;
            emailLabel.Text = "이메일 : ";
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new Point(91, 125);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new Size(100, 25);
            passwordLabel.TabIndex = 1;
            passwordLabel.Text = "비밀번호 : ";
            // 
            // emailTextBox
            // 
            emailTextBox.Location = new Point(197, 72);
            emailTextBox.Name = "emailTextBox";
            emailTextBox.Size = new Size(150, 31);
            emailTextBox.TabIndex = 2;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Location = new Point(197, 122);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.Size = new Size(150, 31);
            passwordTextBox.TabIndex = 3;
            // 
            // passwordCheckTextBox
            // 
            passwordCheckTextBox.Location = new Point(197, 171);
            passwordCheckTextBox.Name = "passwordCheckTextBox";
            passwordCheckTextBox.Size = new Size(150, 31);
            passwordCheckTextBox.TabIndex = 5;
            // 
            // passwordCheckLabel
            // 
            passwordCheckLabel.AutoSize = true;
            passwordCheckLabel.Location = new Point(49, 174);
            passwordCheckLabel.Name = "passwordCheckLabel";
            passwordCheckLabel.Size = new Size(142, 25);
            passwordCheckLabel.TabIndex = 4;
            passwordCheckLabel.Text = "비밀번호 확인 : ";
            // 
            // emailCheckBtn
            // 
            emailCheckBtn.Location = new Point(373, 70);
            emailCheckBtn.Name = "emailCheckBtn";
            emailCheckBtn.Size = new Size(128, 34);
            emailCheckBtn.TabIndex = 6;
            emailCheckBtn.Text = "이메일 확인";
            emailCheckBtn.UseVisualStyleBackColor = true;
            emailCheckBtn.Click += emailCheckBtn_Click;
            // 
            // createAccountBtn
            // 
            createAccountBtn.Location = new Point(235, 229);
            createAccountBtn.Name = "createAccountBtn";
            createAccountBtn.Size = new Size(112, 34);
            createAccountBtn.TabIndex = 7;
            createAccountBtn.Text = "계정 생성";
            createAccountBtn.UseVisualStyleBackColor = true;
            // 
            // CreateAccountForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(592, 322);
            Controls.Add(createAccountBtn);
            Controls.Add(emailCheckBtn);
            Controls.Add(passwordCheckTextBox);
            Controls.Add(passwordCheckLabel);
            Controls.Add(passwordTextBox);
            Controls.Add(emailTextBox);
            Controls.Add(passwordLabel);
            Controls.Add(emailLabel);
            Name = "CreateAccountForm";
            Text = "CreateAccount";
            FormClosing += CreateFormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label emailLabel;
        private Label passwordLabel;
        private TextBox emailTextBox;
        private TextBox passwordTextBox;
        private TextBox passwordCheckTextBox;
        private Label passwordCheckLabel;
        private Button emailCheckBtn;
        private Button createAccountBtn;
    }
}