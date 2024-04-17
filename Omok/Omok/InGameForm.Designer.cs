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
            emailLabel = new Label();
            emailTextBox = new TextBox();
            levelLabel = new Label();
            levelTextBox = new TextBox();
            WinLoseLabel = new Label();
            winningRateTextBox = new TextBox();
            SuspendLayout();
            // 
            // emailLabel
            // 
            emailLabel.AutoSize = true;
            emailLabel.Location = new Point(28, 9);
            emailLabel.Name = "emailLabel";
            emailLabel.Size = new Size(82, 25);
            emailLabel.TabIndex = 0;
            emailLabel.Text = "이메일 : ";
            // 
            // emailTextBox
            // 
            emailTextBox.Location = new Point(116, 6);
            emailTextBox.Name = "emailTextBox";
            emailTextBox.ReadOnly = true;
            emailTextBox.Size = new Size(209, 31);
            emailTextBox.TabIndex = 1;
            // 
            // levelLabel
            // 
            levelLabel.AutoSize = true;
            levelLabel.Location = new Point(343, 9);
            levelLabel.Name = "levelLabel";
            levelLabel.Size = new Size(128, 25);
            levelLabel.TabIndex = 2;
            levelLabel.Text = "레벨(경험치) : ";
            // 
            // levelTextBox
            // 
            levelTextBox.Location = new Point(477, 6);
            levelTextBox.Name = "levelTextBox";
            levelTextBox.ReadOnly = true;
            levelTextBox.Size = new Size(77, 31);
            levelTextBox.TabIndex = 3;
            // 
            // WinLoseLabel
            // 
            WinLoseLabel.AutoSize = true;
            WinLoseLabel.Location = new Point(565, 9);
            WinLoseLabel.Name = "WinLoseLabel";
            WinLoseLabel.Size = new Size(64, 25);
            WinLoseLabel.TabIndex = 4;
            WinLoseLabel.Text = "승률 : ";
            // 
            // winningRateTextBox
            // 
            winningRateTextBox.Location = new Point(635, 6);
            winningRateTextBox.Name = "winningRateTextBox";
            winningRateTextBox.ReadOnly = true;
            winningRateTextBox.Size = new Size(150, 31);
            winningRateTextBox.TabIndex = 5;
            // 
            // InGameForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(winningRateTextBox);
            Controls.Add(WinLoseLabel);
            Controls.Add(levelTextBox);
            Controls.Add(levelLabel);
            Controls.Add(emailTextBox);
            Controls.Add(emailLabel);
            Name = "InGameForm";
            Text = "InGameForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label emailLabel;
        private TextBox emailTextBox;
        private Label levelLabel;
        private TextBox levelTextBox;
        private Label WinLoseLabel;
        private TextBox winningRateTextBox;
    }
}