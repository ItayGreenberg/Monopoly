using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Net;

namespace MonopolyServer
{
    public partial class ServerForm : Form
    {
        public static ServerForm s_instance = null;
        public IPAddress serverIp;
        private readonly object Lock = new object();
        public ServerForm()
        {
            InitializeComponent();
            s_instance = this;
            this.FormClosed += new FormClosedEventHandler(MainForm_FormClosed);
            serverIp = getIp();
            InitServerIpBox();
            Globals.lobbyCtrl.StartServer();
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
        private IPAddress getIp()
        {
            string hostname = Dns.GetHostName();
            IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname);

            IPAddress localIPAddress = null;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    IPInterfaceProperties ipProps = ni.GetIPProperties();

                    foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                            !IPAddress.IsLoopback(ip.Address) && !ip.Address.IsIPv6LinkLocal)
                        {
                            foreach (IPAddress hostAddress in hostAddresses)
                            {
                                if (ip.Address.Equals(hostAddress))
                                {
                                    localIPAddress = ip.Address;
                                    break;
                                }
                            }
                        }

                        if (localIPAddress != null)
                            break;
                    }
                }

                if (localIPAddress != null)
                    break;
            }
            return localIPAddress;
        }
        private void InitServerIpBox()
        {
            
            serverIPBox.Text = $"server ip: {serverIp}";
        }

        private void testCommCtrl_Click(object sender, EventArgs e)
        {
            ServerForm.s_instance.UpdateDebugOutput("testing comm control");
        }
        public void UpdateDebugOutput(string message)
        {
            lock (Lock)
            {

                // InvokeRequired returns true if called from a thread other than the UI thread
                if (debugOutput.InvokeRequired)
                {
                    // Call the same method on the UI thread using BeginInvoke
                    BeginInvoke(new Action<string>(UpdateDebugOutput), message);
                }
                else
                {
                    // Update the TextBox with the message
                    debugOutput.Text += message + Environment.NewLine;
                }
            }
        }
        
    }

}
