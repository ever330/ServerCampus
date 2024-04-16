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
            emailLabel.Location = new Point(76, 45);
            emailLabel.Margin = new Padding(2, 0, 2, 0);
            emailLabel.Name = "emailLabel";
            emailLabel.Size = new Size(54, 15);
            emailLabel.TabIndex = 0;
            emailLabel.Text = "이메일 : ";
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new Point(64, 75);
            passwordLabel.Margin = new Padding(2, 0, 2, 0);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new Size(66, 15);
            passwordLabel.TabIndex = 1;
            passwordLabel.Text = "비밀번호 : ";
            // 
            // emailTextBox
            // 
            emailTextBox.Location = new Point(138, 43);
            emailTextBox.Margin = new Padding(2);
            emailTextBox.Name = "emailTextBox";
            emailTextBox.Size = new Size(106, 23);
            emailTextBox.TabIndex = 2;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Location = new Point(138, 73);
            passwordTextBox.Margin = new Padding(2);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.Size = new Size(106, 23);
            passwordTextBox.TabIndex = 3;
            // 
            // passwordCheckTextBox
            // 
            passwordCheckTextBox.Location = new Point(138, 103);
            passwordCheckTextBox.Margin = new Padding(2);
            passwordCheckTextBox.Name = "passwordCheckTextBox";
            passwordCheckTextBox.Size = new Size(106, 23);
            passwordCheckTextBox.TabIndex = 5;
            // 
            // passwordCheckLabel
            // 
            passwordCheckLabel.AutoSize = true;
            passwordCheckLabel.Location = new Point(34, 104);
            passwordCheckLabel.Margin = new Padding(2, 0, 2, 0);
            passwordCheckLabel.Name = "passwordCheckLabel";
            passwordCheckLabel.Size = new Size(94, 15);
            passwordCheckLabel.TabIndex = 4;
            passwordCheckLabel.Text = "비밀번호 확인 : ";
            // 
            // emailCheckBtn
            // 
            emailCheckBtn.Location = new Point(261, 42);
            emailCheckBtn.Margin = new Padding(2);
            emailCheckBtn.Name = "emailCheckBtn";
            emailCheckBtn.Size = new Size(90, 24);
            emailCheckBtn.TabIndex = 6;
            emailCheckBtn.Text = "이메일 확인";
            emailCheckBtn.UseVisualStyleBackColor = true;
            emailCheckBtn.Click += emailCheckBtn_Click;
            // 
            // createAccountBtn
            // 
            createAccountBtn.Location = new Point(164, 137);
            createAccountBtn.Margin = new Padding(2);
            createAccountBtn.Name = "createAccountBtn";
            createAccountBtn.Size = new Size(78, 25);
            createAccountBtn.TabIndex = 7;
            createAccountBtn.Text = "계정 생성";
            createAccountBtn.UseVisualStyleBackColor = true;
            createAccountBtn.Click += createAccountBtn_Click;
            // 
            // CreateAccountForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(414, 193);
            Controls.Add(createAccountBtn);
            Controls.Add(emailCheckBtn);
            Controls.Add(passwordCheckTextBox);
            Controls.Add(passwordCheckLabel);
            Controls.Add(passwordTextBox);
            Controls.Add(emailTextBox);
            Controls.Add(passwordLabel);
            Controls.Add(emailLabel);
            Margin = new Padding(2);
            Name = "CreateAccountForm";
            Text = "CreateAccount";
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