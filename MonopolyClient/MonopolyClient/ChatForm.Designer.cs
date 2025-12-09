
namespace MonopolyClient
{
    partial class ChatForm
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
            sendMessageButton = new Button();
            ChatBox = new TextBox();
            SuspendLayout();
            // 
            // sendMessageButton
            // 
            sendMessageButton.BackColor = Color.LightGreen;
            sendMessageButton.FlatStyle = FlatStyle.Flat;
            sendMessageButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            sendMessageButton.Location = new Point(568, 21);
            sendMessageButton.Name = "sendMessageButton";
            sendMessageButton.Size = new Size(136, 35);
            sendMessageButton.TabIndex = 0;
            sendMessageButton.Text = "Send Message";
            sendMessageButton.UseVisualStyleBackColor = false;
            sendMessageButton.Click += sendMessageButton_Click;
            // 
            // ChatBox
            // 
            ChatBox.BackColor = Color.Silver;
            ChatBox.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ChatBox.Location = new Point(151, 80);
            ChatBox.Multiline = true;
            ChatBox.Name = "ChatBox";
            ChatBox.ReadOnly = true;
            ChatBox.ScrollBars = ScrollBars.Both;
            ChatBox.Size = new Size(959, 431);
            ChatBox.TabIndex = 1;
            // 
            // ChatForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(1255, 577);
            Controls.Add(ChatBox);
            Controls.Add(sendMessageButton);
            Name = "ChatForm";
            Text = "Chat";
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private Button sendMessageButton;
        private TextBox ChatBox;
    }
}