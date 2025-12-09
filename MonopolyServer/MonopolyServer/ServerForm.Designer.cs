namespace MonopolyServer
{
    partial class ServerForm
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
            debugOutput = new TextBox();
            testCommCtrl = new Button();
            serverIPBox = new TextBox();
            SuspendLayout();
            // 
            // debugOutput
            // 
            debugOutput.BackColor = SystemColors.ActiveCaptionText;
            debugOutput.ForeColor = Color.White;
            debugOutput.Location = new Point(31, 42);
            debugOutput.Multiline = true;
            debugOutput.Name = "debugOutput";
            debugOutput.ReadOnly = true;
            debugOutput.ScrollBars = ScrollBars.Both;
            debugOutput.Size = new Size(348, 249);
            debugOutput.TabIndex = 0;
            // 
            // testCommCtrl
            // 
            testCommCtrl.BackColor = SystemColors.Highlight;
            testCommCtrl.Location = new Point(476, 117);
            testCommCtrl.Name = "testCommCtrl";
            testCommCtrl.Size = new Size(98, 44);
            testCommCtrl.TabIndex = 1;
            testCommCtrl.Text = "Test Comm Control";
            testCommCtrl.UseVisualStyleBackColor = false;
            testCommCtrl.Click += testCommCtrl_Click;
            // 
            // serverIPBox
            // 
            serverIPBox.BackColor = SystemColors.GradientActiveCaption;
            serverIPBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            serverIPBox.Location = new Point(445, 42);
            serverIPBox.Multiline = true;
            serverIPBox.Name = "serverIPBox";
            serverIPBox.ReadOnly = true;
            serverIPBox.Size = new Size(180, 46);
            serverIPBox.TabIndex = 2;
            serverIPBox.TextAlign = HorizontalAlignment.Center;
            // 
            // ServerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(serverIPBox);
            Controls.Add(testCommCtrl);
            Controls.Add(debugOutput);
            Name = "ServerForm";
            Text = "ServerForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox debugOutput;
        private Button testCommCtrl;
        private TextBox serverIPBox;
    }
}
