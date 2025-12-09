using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MonopolyClient
{
    public partial class ChatForm : Form
    {
        public static ChatForm s_instance = null;
        private object addTextLock = new object();
        public ChatForm()
        {
            InitializeComponent();
            s_instance = this;
            this.FormClosing += ChatForm_FormClosing; // Subscribe to FormClosing event

        }

        private void sendMessageButton_Click(object sender, EventArgs e)
        {
            string stringMessage = Interaction.InputBox("Enter your message:", "Send Message", "");
            if (!string.IsNullOrEmpty(stringMessage))
            {
                if (stringMessage.Length <= 100)
                {
                    Message message = new Message();
                    message.messageId = MessageId.SendMessage;
                    message.message = stringMessage;
                    Globals.LobbyCtrl.messageQueue.Add(message);
                }
                else
                {
                    MessageBox.Show("maximum length is 100", "Chat", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {
                MessageBox.Show("can't send an empty message", "Chat", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        public void AddMessage(int id, string message)
        {
            lock (addTextLock)
            {
                if (ChatBox.InvokeRequired)
                {
                    // Call the same method on the UI thread using BeginInvoke
                    BeginInvoke(new Action<int, string>(AddMessage), id, message);
                }
                else
                {
                    string newMessage = Globals.players[id].name;
                    if (id == ClientForm.s_instance.thisPlayer.id)
                    {
                        newMessage += " (you): " + message;
                    }
                    else
                    {
                        newMessage += ": " + message;
                    }
                    ChatBox.Text += newMessage + Environment.NewLine;

                }
            }
        }

        private void ChatForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the close reason is UserClosing (when user clicks the close button)
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Cancel the closing event
                e.Cancel = true;
                // Hide the form instead of closing it
                this.Hide();
            }
        }


    }
}
