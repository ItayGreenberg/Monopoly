using Microsoft.VisualBasic;
using System.Collections.Concurrent;
using System.Numerics;
using Font = System.Drawing.Font;
using Image = System.Drawing.Image;
using System.IO;

namespace MonopolyClient
{
    public partial class ClientForm : Form
    {
        public static ClientForm s_instance = null;
        public ChatForm chatForm;
        public TradeForm tradeForm;
        public BlockingCollection<string> privateMessageQueue;
        private TaskCompletionSource<bool> privateOkClicked = new TaskCompletionSource<bool>();

        public Player thisPlayer;

        private string baseDir;
        PictureBox[] piecesOnBoard = new PictureBox[4];
        Button[] playersNameButtons = new Button[4];
        Label[] moneyLabels = new Label[4];
        Button[] readyButtons = new Button[4];
        Label[] amountOfOutOfJailCardsLabels = new Label[4];
        Label[] privateMessageLabeles = new Label[4];
        Button[] privateOkButtons = new Button[4];


        private Label[] squaresOwners = new Label[40]; // squarre Id, ownerName
        public PictureBox[] housesPictures = new PictureBox[40];
        private Label[] housesMultipliers = new Label[40];
        private Button[] squaresButtons = new Button[40];


        private int currentPlayerId;
        private Player currentPlayer;
        public ClientForm()
        {
            InitializeComponent();
            s_instance = this;
            privateMessageQueue = new BlockingCollection<string>();
            this.FormClosed += new FormClosedEventHandler(MainForm_FormClosed);



            Globals.LobbyCtrl.Init();
            baseDir = Application.StartupPath;
            InitPiecesOnBoard();
            InitPlayersNameButtons();
            InitMoneyLables();
            InitReadyButtons();
            InitAmountOfOutOfJailCardsLabels();
            InitPrivateMessageLabeles();
            InitPrivateOkButtons();
                       

            InitSquaresButtons();
            InitOwnerNames();
            InitHousesPictures();
            InitHousesMultiplier();

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
        private void InitPiecesOnBoard()
        {
            int firstRow = 1398;
            int firstCol = 923;
            int secondRow = 1443;
            int secondCol = 953;
            //horizontal gap between first row and second is 45 so the max square is 45 + 2 * xSize of the pieces 
            PictureBox train = new PictureBox();
            train.Image = Image.FromFile(Path.Combine(baseDir, "Pictures", "Pieces", "train.png")); // also possible @"Pictues\Pieces\ship.png"
            InitializePictureBox(train, firstRow, firstCol);
            piecesOnBoard[0] = train;

            PictureBox hat = new PictureBox();
            hat.Image = Image.FromFile(Path.Combine(baseDir, "Pictures", "Pieces", "hat.png"));
            InitializePictureBox(hat, firstRow, secondCol);
            piecesOnBoard[1] = hat;

            PictureBox car = new PictureBox();
            car.Image = Image.FromFile(Path.Combine(baseDir, "Pictures", "Pieces", "car.png"));
            InitializePictureBox(car, secondRow, firstCol);
            piecesOnBoard[2] = car;

            PictureBox ship = new PictureBox();
            ship.Image = Image.FromFile(Path.Combine(baseDir, "Pictures", "Pieces", "ship.png")); 
            InitializePictureBox(ship, secondRow, secondCol);
            piecesOnBoard[3] = ship;

        }
        private void InitializePictureBox(PictureBox pictureBox, int x, int y)
        {
            pictureBox.Location = new Point(x, y);
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox.Size = new Size(24, 22);
            pictureBox.Visible = false;
            pictureBox.BackColor = Color.FromArgb(192, 255, 192);
            Controls.Add(pictureBox);
            pictureBox.BringToFront();


        }
        private void InitPlayersNameButtons()
        {
            playersNameButtons[0] = Player1NameButton;
            playersNameButtons[1] = Player2NameButton;
            playersNameButtons[2] = Player3NameButton;
            playersNameButtons[3] = Player4NameButton;
        }
        private void InitMoneyLables()
        {
            moneyLabels[0] = Player1Money;
            moneyLabels[1] = Player2Money;
            moneyLabels[2] = Player3Money;
            moneyLabels[3] = Player4Money;
        }
        private void InitReadyButtons()
        {
            readyButtons[0] = ReadyButton1;
            readyButtons[1] = ReadyButton2;
            readyButtons[2] = ReadyButton3;
            readyButtons[3] = ReadyButton4;
        }
        private void InitAmountOfOutOfJailCardsLabels()
        {
            amountOfOutOfJailCardsLabels[0] = amountOfOutOfJailCardsLabel1;
            amountOfOutOfJailCardsLabels[1] = amountOfOutOfJailCardsLabel2;
            amountOfOutOfJailCardsLabels[2] = amountOfOutOfJailCardsLabel3;
            amountOfOutOfJailCardsLabels[3] = amountOfOutOfJailCardsLabel4;
        }
        private void InitPrivateMessageLabeles()
        {
            privateMessageLabeles[0] = PrivateMessageLabel1;
            privateMessageLabeles[1] = PrivateMessageLabel2;
            privateMessageLabeles[2] = PrivateMessageLabel3;
            privateMessageLabeles[3] = PrivateMessageLabel4;
        }
        private void InitPrivateOkButtons()
        {
            privateOkButtons[0] = PrivateOkButton1;
            privateOkButtons[1] = PrivateOkButton2;
            privateOkButtons[2] = PrivateOkButton3;
            privateOkButtons[3] = PrivateOkButton4;

        }
        private void InitSquaresButtons()
        {
            squaresButtons[0] = square0;
            squaresButtons[1] = square1;
            squaresButtons[2] = square2;
            squaresButtons[3] = square3;
            squaresButtons[4] = square4;
            squaresButtons[5] = square5;
            squaresButtons[6] = square6;
            squaresButtons[7] = square7;
            squaresButtons[8] = square8;
            squaresButtons[9] = square9;
            squaresButtons[10] = square10;
            squaresButtons[11] = square11;
            squaresButtons[12] = square12;
            squaresButtons[13] = square13;
            squaresButtons[14] = square14;
            squaresButtons[15] = square15;
            squaresButtons[16] = square16;
            squaresButtons[17] = square17;
            squaresButtons[18] = square18;
            squaresButtons[19] = square19;
            squaresButtons[20] = square20;
            squaresButtons[21] = square21;
            squaresButtons[22] = square22;
            squaresButtons[23] = square23;
            squaresButtons[24] = square24;
            squaresButtons[25] = square25;
            squaresButtons[26] = square26;
            squaresButtons[27] = square27;
            squaresButtons[28] = square28;
            squaresButtons[29] = square29;
            squaresButtons[30] = square30;
            squaresButtons[31] = square31;
            squaresButtons[32] = square32;
            squaresButtons[33] = square33;
            squaresButtons[34] = square34;
            squaresButtons[35] = square35;
            squaresButtons[36] = square36;
            squaresButtons[37] = square37;
            squaresButtons[38] = square38;
            squaresButtons[39] = square39;


            for (int i = 0; i < squaresButtons.Length; i++) //size, text
            {
                Square square = Board.squares[i];
                for (int j = 0; j < square.name.Length; j++)
                {
                    if (square.name[j] == ' ')
                    {
                        if (square.name[j - 1] != '.')
                            squaresButtons[i].Text += "\r\n";
                        else
                            squaresButtons[i].Text += " ";

                    }
                    else
                        squaresButtons[i].Text += square.name[j];
                }
                if (square is StreetSquare)
                {
                    squaresButtons[i].Text += $"\r\n Price:{((StreetSquare)square).price}";
                }
                else if (square is RailroadSquare)
                {
                    squaresButtons[i].Text += $"\r\n Price:{((RailroadSquare)square).price}";
                }
                else if (square is UtilitySquare)
                {
                    squaresButtons[i].Text += $"\r\n Price:{((UtilitySquare)square).price}";
                }
                squaresButtons[i].Font = new Font(squaresButtons[i].Font.FontFamily, 10);
                squaresButtons[i].ForeColor = Color.Black;

            }


            foreach (Button button in squaresButtons)// outlines, disable
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = Color.Black;
                button.FlatAppearance.BorderSize = 1;
            }


        }        
        private void InitOwnerNames()
        {
            squaresOwners[1] = OwnerName1;
            squaresOwners[3] = OwnerName2;
            squaresOwners[5] = OwnerName3;
            squaresOwners[6] = OwnerName4;
            squaresOwners[8] = OwnerName5;
            squaresOwners[9] = OwnerName6;
            squaresOwners[11] = OwnerName7;
            squaresOwners[12] = OwnerName8;
            squaresOwners[13] = OwnerName9;
            squaresOwners[14] = OwnerName10;
            squaresOwners[15] = OwnerName11;
            squaresOwners[16] = OwnerName12;
            squaresOwners[18] = OwnerName13;
            squaresOwners[19] = OwnerName14;
            squaresOwners[21] = OwnerName15;
            squaresOwners[23] = OwnerName16;
            squaresOwners[24] = OwnerName17;
            squaresOwners[25] = OwnerName18;
            squaresOwners[26] = OwnerName19;
            squaresOwners[27] = OwnerName20;
            squaresOwners[28] = OwnerName21;
            squaresOwners[29] = OwnerName22;
            squaresOwners[31] = OwnerName23;
            squaresOwners[32] = OwnerName24;
            squaresOwners[34] = OwnerName25;
            squaresOwners[35] = OwnerName26;
            squaresOwners[37] = OwnerName27;
            squaresOwners[39] = OwnerName28;
        }
        private void InitHousesPictures()
        {
            housesPictures[1] = house1;
            housesPictures[3] = house2;
            housesPictures[6] = house3;
            housesPictures[8] = house4;
            housesPictures[9] = house5;
            housesPictures[11] = house6;
            housesPictures[13] = house7;
            housesPictures[14] = house8;
            housesPictures[16] = house9;
            housesPictures[18] = house10;
            housesPictures[19] = house11;
            housesPictures[21] = house12;
            housesPictures[23] = house13;
            housesPictures[24] = house14;
            housesPictures[26] = house15;
            housesPictures[27] = house16;
            housesPictures[29] = house17;
            housesPictures[31] = house18;
            housesPictures[32] = house19;
            housesPictures[34] = house20;
            housesPictures[37] = house21;
            housesPictures[39] = house22;
        }
        private void InitHousesMultiplier()
        {
            housesMultipliers[1] = houseMul1;
            housesMultipliers[3] = houseMul2;
            housesMultipliers[6] = houseMul3;
            housesMultipliers[8] = houseMul4;
            housesMultipliers[9] = houseMul5;
            housesMultipliers[11] = houseMul6;
            housesMultipliers[13] = houseMul7;
            housesMultipliers[14] = houseMul8;
            housesMultipliers[16] = houseMul9;
            housesMultipliers[18] = houseMul10;
            housesMultipliers[19] = houseMul11;
            housesMultipliers[21] = houseMul12;
            housesMultipliers[23] = houseMul13;
            housesMultipliers[24] = houseMul14;
            housesMultipliers[26] = houseMul15;
            housesMultipliers[27] = houseMul16;
            housesMultipliers[29] = houseMul17;
            housesMultipliers[31] = houseMul18;
            housesMultipliers[32] = houseMul19;
            housesMultipliers[34] = houseMul20;
            housesMultipliers[37] = houseMul21;
            housesMultipliers[39] = houseMul22;
        }
        
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            ConnectButton.Enabled = false;
            string serverIp = Interaction.InputBox("enter the server ip:", "server ip input");
            if (!string.IsNullOrEmpty(serverIp))
            {
                Message message = new Message();
                message.messageId = MessageId.Connect;
                message.serverIp = serverIp;
                Globals.LobbyCtrl.messageQueue.Add(message);

            }
            else
            {
                ConnectButton.Enabled = true;
            }

        }
        public void ConnectSuccess()
        {
            // InvokeRequired returns true if called from a thread other than the UI thread
            if (ConnectButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action(ConnectSuccess));
            }
            else
            {
                ConnectButton.Visible = false;
                foreach (Button button in playersNameButtons)
                {
                    button.Visible = true;
                }
            }
        }
        public void ConnectFailed()
        {
            // InvokeRequired returns true if called from a thread other than the UI thread
            if (ConnectButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action(ConnectFailed));
            }
            else
            {
                MessageBox.Show("Failed to connect. Please try again", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ConnectButton.Enabled = true;
            }
        }
        private void Player1NameButton_Click(object sender, EventArgs e)
        {

            Player1NameButton.Enabled = false;
            string name = Interaction.InputBox("Enter your name:", "Name Input");

            if (!string.IsNullOrEmpty(name))
            {
                Message message = new Message();
                message.messageId = MessageId.RegisterPlayer;
                message.name = name;
                message.id = 0;
                Globals.LobbyCtrl.messageQueue.Add(message);
            }
            else
            {
                Player1NameButton.Enabled = true;
            }
        }
        private void Player2NameButton_Click(object sender, EventArgs e)
        {
            Player2NameButton.Enabled = false;
            string name = Interaction.InputBox("Enter your name:", "Name Input");

            if (!string.IsNullOrEmpty(name))
            {
                Message message = new Message();
                message.messageId = MessageId.RegisterPlayer;
                message.name = name;
                message.id = 1;
                Globals.LobbyCtrl.messageQueue.Add(message);
            }
            else
            {
                Player2NameButton.Enabled = true;
            }
        }
        private void Player3NameButton_Click(object sender, EventArgs e)
        {
            Player3NameButton.Enabled = false;
            string name = Interaction.InputBox("Enter your name:", "Name Input");

            if (!string.IsNullOrEmpty(name))
            {
                Message message = new Message();
                message.messageId = MessageId.RegisterPlayer;
                message.name = name;
                message.id = 2;
                Globals.LobbyCtrl.messageQueue.Add(message);
            }
            else
            {
                Player3NameButton.Enabled = true;
            }
        }
        private void Player4NameButton_Click(object sender, EventArgs e)
        {
            Player4NameButton.Enabled = false;
            string name = Interaction.InputBox("Enter your name:", "Name Input");

            if (!string.IsNullOrEmpty(name))
            {
                Message message = new Message();
                message.messageId = MessageId.RegisterPlayer;
                message.name = name;
                message.id = 3;
                Globals.LobbyCtrl.messageQueue.Add(message);
            }
            else
            {
                Player4NameButton.Enabled = true;
            }
        }
        public void RegisterSuccess(string name, int id)
        {
            // InvokeRequired returns true if called from a thread other than the UI thread
            if (Player1NameButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<string, int>(RegisterSuccess), name, id);
            }
            else
            {
                thisPlayer = new Player(name, moneyLabels[id], piecesOnBoard[id], id);
                thisPlayer.moneyLabel.Visible = true;
                thisPlayer.pictureBox.Visible = true;
                playersNameButtons[id].Text = name;
                playersNameButtons[id].Visible = true;
                foreach (Button b in playersNameButtons)
                {
                    b.Enabled = false;
                }
                Globals.players[id] = thisPlayer;
            }
        }
        public void RegisterFailure(int id)
        {
            if (Player1NameButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int>(RegisterFailure), id);
            }
            else
            {
                MessageBox.Show("Failed to register. Please try again", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (!moneyLabels[id].Visible)
                    playersNameButtons[id].Enabled = true;
            }
        }
        public void PlayerJoined(string name, int id)
        {
            // InvokeRequired returns true if called from a thread other than the UI thread
            if (Player2NameButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<string, int>(PlayerJoined), name, id);
            }
            else
            {
                Player player = new Player(name, moneyLabels[id], piecesOnBoard[id], id);
                player.moneyLabel.Visible = true;
                player.pictureBox.Visible = true;
                playersNameButtons[id].Text = name;
                playersNameButtons[id].Visible = true;
                playersNameButtons[id].Enabled = false;
                Globals.players[id] = player;
            }
        }
        public void UpdateDebugOutput(string message)
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
        } // not used now        
        public void ActivateReadyButton(int id)
        {
            if (readyButtons[id].InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int>(ActivateReadyButton), id);
            }
            else
            {
                readyButtons[id].Visible = true;
                readyButtons[id].Enabled = true;
                for (int i = 0; i < 4; i++)
                {
                    if (moneyLabels[i].Visible == true)
                    {
                        readyButtons[i].Visible = true;
                    }
                }
            }

        }
        private void ReadyButton1_Click(object sender, EventArgs e)
        {
            ReadyButton1.Enabled = false;
            Message message = new Message();
            message.messageId = MessageId.ReadyToStartGame;
            Globals.LobbyCtrl.messageQueue.Add(message);
        }
        private void ReadyButton2_Click(object sender, EventArgs e)
        {
            ReadyButton2.Enabled = false;
            Message message = new Message();
            message.messageId = MessageId.ReadyToStartGame;
            Globals.LobbyCtrl.messageQueue.Add(message);
        }
        private void ReadyButton3_Click(object sender, EventArgs e)
        {
            ReadyButton3.Enabled = false;
            Message message = new Message();
            message.messageId = MessageId.ReadyToStartGame;
            Globals.LobbyCtrl.messageQueue.Add(message);
        }
        private void ReadyButton4_Click(object sender, EventArgs e)
        {
            ReadyButton4.Enabled = false;
            Message message = new Message();
            message.messageId = MessageId.ReadyToStartGame;
            Globals.LobbyCtrl.messageQueue.Add(message);
        }
        public void ReadySuccess(int id)
        {
            if (readyButtons[id].InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int>(ReadySuccess), id);
            }
            else
            {
                readyButtons[id].Text = "Ready!";
            }
        }
        public void ReadyFailure(int id)
        {
            if (readyButtons[id].InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int>(ReadyFailure), id);
            }
            else
            {
                MessageBox.Show("Failed to ready. Please try again", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                readyButtons[id].Enabled = true;
            }

        }
        public void PlayerIsReady(int id)
        {
            if (readyButtons[id].InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int>(PlayerIsReady), id);
            }
            else
            {
                readyButtons[id].Text = "Ready!";
            }
        }
        public void EnableRegisterInId(int id)
        {
            if (readyButtons[id].InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int>(EnableRegisterInId), id);
            }
            else
            {
                playersNameButtons[id].Text = "Click to enter name";
                if(thisPlayer == null)
                    playersNameButtons[id].Enabled = true;
                readyButtons[id].Visible = false;
                readyButtons[id].Text = "Click to ready";
                moneyLabels[id].Visible = false;
                Globals.players[id].pictureBox.Visible = false;
                Globals.players[id] = null;
            }
        }
        public void DisableReady()
        {
            if (Player1NameButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action(DisableReady));
            }
            else
            {
                readyButtons[thisPlayer.id].Visible = false;
                readyButtons[thisPlayer.id].Text = "Click to ready";
            }
        }
        public void GameStarted()
        {
            if (LeftDice.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action(GameStarted));
            }
            else
            {
                if (thisPlayer != null)
                {
                    LeftDice.Visible = true;
                    RightDice.Visible = true;

                    RollButton.Visible = true;
                    RollButton.Enabled = false;

                    CurrentPlayerNameLabel.Visible = true;

                    ChatButton.Visible = true;
                    tradeForm = new TradeForm();
                    chatForm = new ChatForm();
                    TradeButton.Visible = true;
                    PictureBox[] screenPices = { player1Piece, player2Piece, player3Piece, player4Piece };
                    for (int i = 0; i < moneyLabels.Length; i++)
                    {
                        readyButtons[i].Visible = false;
                        if (moneyLabels[i].Visible == false)
                        {
                            playersNameButtons[i].Visible = false;
                            screenPices[i].Visible = false;
                        }
                        else
                        {
                            amountOfOutOfJailCardsLabels[i].Text = "out of jail cards: 0";
                            amountOfOutOfJailCardsLabels[i].Visible = true;
                        }
                    }
                    Thread thread = new Thread(PrivateMessageQueue);
                    thread.Start();
                }
                else
                {
                    foreach (Button button in playersNameButtons)
                    {
                        button.Enabled = false;
                    }
                    MessageBox.Show("game had already started. Please close the program", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }

        }
        public async void PrivateMessageQueue()
        {
            foreach (string message in privateMessageQueue.GetConsumingEnumerable())
            {
                if (this.InvokeRequired)
                {
                    // If the method is called from a non-UI thread, marshal the call to the UI thread
                    this.Invoke(new Action(async () =>
                    {
                        privateMessageLabeles[thisPlayer.id].Visible = true;
                        privateOkButtons[thisPlayer.id].Visible = true;
                        privateMessageLabeles[thisPlayer.id].Text = message;

                        await privateOkClicked.Task;
                        privateOkClicked = new TaskCompletionSource<bool>();

                        privateMessageLabeles[thisPlayer.id].Visible = false;
                        privateOkButtons[thisPlayer.id].Visible = false;
                    }));
                }
                else
                {
                    // If already on the UI thread, directly access UI controls
                    privateMessageLabeles[thisPlayer.id].Visible = true;
                    privateOkButtons[thisPlayer.id].Visible = true;
                    privateMessageLabeles[thisPlayer.id].Text = message;

                    await privateOkClicked.Task;
                    privateOkClicked = new TaskCompletionSource<bool>();

                    privateMessageLabeles[thisPlayer.id].Visible = false;
                    privateOkButtons[thisPlayer.id].Visible = false;
                }

                await Task.Delay(1000);
            }
        }
        public void GameHadAlreadyStarted()
        {
            if (LeftDice.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action(GameHadAlreadyStarted));
            }
            else
            {
                foreach (Button button in playersNameButtons)
                {
                    button.Enabled = false;
                }
                MessageBox.Show("game had already started. Please close the program", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public void WhoseTurn(int id)
        {
            if (RollButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int>(WhoseTurn), id);
            }
            else
            {
                currentPlayerId = id;
                currentPlayer = Globals.players[currentPlayerId];
                CurrentPlayerNameLabel.Text = $"current player: {currentPlayer.name}";
            }
        }
        public void RollDiceAvailable()
        {
            if (RollButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action(RollDiceAvailable));
            }
            else
            {
                RollButton.Enabled = true;
            }
        }
        private void RollButton_Click(object sender, EventArgs e)
        {
            RollButton.Enabled = false;
            //UpdateDebugOutput("rollButtonClicked");
            Message message = new Message();
            message.messageId = MessageId.RollDiceClicked;
            Globals.LobbyCtrl.messageQueue.Add(message);
        }
        public void RollDiceFailure()
        {
            if (RollButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action(RollDiceFailure));
            }
            else
            {
                MessageBox.Show("Failed to RollDice. Please try again", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RollButton.Enabled = true;
            }
        }
        public void SendDicesValue(int dice1, int dice2)
        {
            if (RollButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int, int>(SendDicesValue), dice1, dice2);
            }
            else
            {
                RollDiceAsync(dice1, dice2);
            }
        }
        private async void RollDiceAsync(int dice1, int dice2)
        {
            //UpdateDebugOutput($"dices values: {dice1},{dice2}");
            Random rnd = new Random();
            int rolls = 20;
            int prev1 = -1;
            int prev2 = -1;
            int timeToWait = 20;

            for (int i = 0; i < rolls; i++)
            {
                int num1 = rnd.Next(1, 7);
                int num2 = rnd.Next(1, 7);

                if (prev1 == num1)
                    num1 = (num1 + 1) % 6 + 1;
                if (prev2 == num2)
                    num2 = (num2 + 1) % 6 + 1;


                LeftDice.ImageLocation = Path.Combine(baseDir, "Pictures", "Dices", $"dice{num1}.png");
                LeftDice.SizeMode = PictureBoxSizeMode.StretchImage;

                RightDice.ImageLocation = Path.Combine(baseDir, "Pictures", "Dices", $"dice{num2}.png");
                RightDice.SizeMode = PictureBoxSizeMode.StretchImage;

                await Task.Delay(timeToWait);
                timeToWait += 10;
                prev1 = num1;
                prev2 = num2;
            }

            LeftDice.ImageLocation  = Path.Combine(baseDir, "Pictures", "Dices", $"dice{dice1}.png");
            RightDice.ImageLocation = Path.Combine(baseDir, "Pictures", "Dices", $"dice{dice2}.png");

            Message message = new Message();
            message.messageId = MessageId.RollDiceEnded;
            Globals.LobbyCtrl.messageQueue.Add(message);
        }
        public async void MovePlayer(int id, int amount)
        {
            if (RollButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int, int>(MovePlayer), id, amount);
            }
            else
            {
                Move move = new Move(Globals.players[id], amount);
                await move.Execute();

                Message message = new Message();
                message.messageId = MessageId.PlayerMovingEnded;
                Globals.LobbyCtrl.messageQueue.Add(message);
            }

        }       
        public async void PlayerNeedsToGoToSquare(int id, int squareId)
        {
            if (RollButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int, int>(PlayerNeedsToGoToSquare), id, squareId);
            }
            else
            {
                Move move = new Move(Globals.players[id], Board.squares[squareId]);
                await move.Execute();

                Message message = new Message();
                message.messageId = MessageId.PlayerMovingEnded;
                Globals.LobbyCtrl.messageQueue.Add(message);
            }
        }
        public void UpdateMessageLabelForEveryone(string text)
        {
            if (MessageLabel.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<string>(UpdateMessageLabelForEveryone), text);
            }
            else
            {
                MessageLabel.Text = text;
                MessageLabel.Visible = true;
                OkButton.Visible = true;
            }
        }
        private void OkButton_Click(object sender, EventArgs e)
        {
            OkButton.Visible = false;
            MessageLabel.Visible = false;
            TradeMadeLabel.Visible = false;

            Message message = new Message();
            message.messageId = MessageId.OkClicked;
            Globals.LobbyCtrl.messageQueue.Add(message);
        }
        public void BuySquareOffer(int squareId, string text)
        {
            if (MessageLabel.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int, string>(BuySquareOffer), squareId, text);
            }
            else
            {
                MessageLabel.Text = text;
                MessageLabel.Visible = true;

                StreetData.Text = Board.squares[squareId].ToString();
                StreetData.Visible = true;

                BuyButton.Visible = true;
                DontBuyButton.Visible = true;
            }
        }
        public void BuyHouseOffer(int squareId, string text)
        {
            if (MessageLabel.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int, string>(BuyHouseOffer), squareId, text);
            }
            else
            {
                MessageLabel.Text = text;
                MessageLabel.Visible = true;

                StreetData.Text = Board.squares[squareId].ToString();
                StreetData.Visible = true;

                BuyButton.Visible = true;
                DontBuyButton.Visible = true;
            }
        }
        private void BuyButton_Click(object sender, EventArgs e) // buy button
        {
            BuyButton.Visible = false;
            DontBuyButton.Visible = false;
            MessageLabel.Visible = false;
            StreetData.Visible = false;
            Message message = new Message();
            message.messageId = MessageId.Buy;
            Globals.LobbyCtrl.messageQueue.Add(message);


        }
        private void DontBuyButton_Click(object sender, EventArgs e)
        {
            BuyButton.Visible = false;
            DontBuyButton.Visible = false;
            MessageLabel.Visible = false;
            StreetData.Visible = false;
            Message message = new Message();
            message.messageId = MessageId.DontBuy;
            Globals.LobbyCtrl.messageQueue.Add(message);
        }

        public void UpdateSquareWithOwner(int ownerId, int squareId)
        {
            if (RollButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int, int>(UpdateSquareWithOwner), ownerId, squareId);
            }
            else
            {
                squaresOwners[squareId].Text = Globals.players[ownerId].name;
                Globals.players[ownerId].squaresOwned.Add(Board.squares[squareId]);
            }
        }
        public void HouseAdded(int squareId, int amount)
        {
            if (RollButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int, int>(HouseAdded), squareId, amount);
            }
            else
            {
                housesPictures[squareId].Visible = true;
                housesMultipliers[squareId].Text = $"{amount}x";
                housesMultipliers[squareId].Visible = true;

            }
        }
        public void UpdateMoneyLabel(int id, int money)
        {
            if (moneyLabels[id].InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int, int>(UpdateMoneyLabel), id, money);
            }
            else
            {
                moneyLabels[id].Text = money.ToString();
                Globals.players[id].money = money;
                //UpdateDebugOutput(Globals.players[id].name);
            }
        }
        public void AmountOfOutOfJailCards(int id, int amount)
        {
            if (MessageLabel.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int, int>(AmountOfOutOfJailCards), id, amount);
            }
            else
            {
                amountOfOutOfJailCardsLabels[id].Text = $"out of jail cards: {amount}";
            }
        }
        public void UseOutOfJailCardOffer(string text)
        {
            if (MessageLabel.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<string>(UseOutOfJailCardOffer), text);
            }
            else
            {
                MessageLabel.Text = text;
                MessageLabel.Visible = true;

                UseCardButton.Visible = true;
                DontUseCardButton.Visible = true;
            }
        }
        private void UseCardButton_Click(object sender, EventArgs e)
        {
            UseCardButton.Visible = false;
            DontUseCardButton.Visible = false;
            MessageLabel.Visible = false;

            Message message = new Message();
            message.messageId = MessageId.UseCard;
            Globals.LobbyCtrl.messageQueue.Add(message);
        }
        private void DontUseCardButton_Click(object sender, EventArgs e)
        {
            UseCardButton.Visible = false;
            DontUseCardButton.Visible = false;
            MessageLabel.Visible = false;

            Message message = new Message();
            message.messageId = MessageId.DontUseCard;
            Globals.LobbyCtrl.messageQueue.Add(message);
        }
        public void RemoveSquareFromOwner(int squareId, int ownerId)
        {
            if (RollButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<int, int>(RemoveSquareFromOwner), squareId, ownerId);
            }
            else
            {
                Globals.players[ownerId].RemoveSquare(squareId);
            }
        }
        
        
        
        public void RemovePlayerDisconnectedFromClients(int playerId)
        {
            if (RollButton.InvokeRequired)
            {
                BeginInvoke(new Action<int>(RemovePlayerDisconnectedFromClients), playerId);
            }
            else
            {
                List<Square> squaresOwned = Globals.players[playerId].GetSquares();
                foreach (Square square in squaresOwned)
                {
                    int squareId = square.id;
                    squaresOwners[squareId].Text = "";
                    if (Board.squares[squareId] is StreetSquare)
                    {
                        housesPictures[squareId].Visible = false;
                        housesMultipliers[squareId].Visible = false;
                    }
                }
                Globals.players[playerId].pictureBox.Visible = false;
                Globals.players[playerId].moneyLabel.Visible = false;
                amountOfOutOfJailCardsLabels[playerId].Visible = false;
                playersNameButtons[playerId].Visible = false;
                PictureBox[] bigPieces = { player1Piece, player2Piece, player3Piece, player4Piece };
                bigPieces[playerId].Visible = false;
                Globals.players[playerId] = null;
            }
        }
        public void MakePlayerSpectateInClients(int playerId)
        {
            if (RollButton.InvokeRequired)
            {
                BeginInvoke(new Action<int>(MakePlayerSpectateInClients), playerId);
            }
            else
            {
                List<Square> squaresOwned = Globals.players[playerId].GetSquares();
                foreach (Square square in squaresOwned)
                {
                    int squareId = square.id;
                    squaresOwners[squareId].Text = "";
                    if (Board.squares[squareId] is StreetSquare)
                    {
                        housesPictures[squareId].Visible = false;
                        housesMultipliers[squareId].Visible = false;
                    }
                }
                Globals.players[playerId].pictureBox.Visible = false;
                Globals.players[playerId].moneyLabel.Visible = false;
                amountOfOutOfJailCardsLabels[playerId].Visible = false;
                playersNameButtons[playerId].Visible = false;
                PictureBox[] bigPieces = { player1Piece, player2Piece, player3Piece, player4Piece };
                bigPieces[playerId].Visible = false;
                if (playerId == thisPlayer.id)
                {
                    TradeButton.Visible = false;
                    tradeForm.Close();
                }

                Globals.players[playerId] = null;
            }
        }


       
        

        

        private void ChatButton_Click(object sender, EventArgs e)
        {
            chatForm.Show();
        }
        private void TradeButton_Click(object sender, EventArgs e)
        {
            tradeForm.Show();
        }
        private void square1_Click(object sender, EventArgs e)
        {
            SendOffer(1);
        }
        private void square3_Click(object sender, EventArgs e)
        {
            SendOffer(3);
        }

        private void square5_Click(object sender, EventArgs e)
        {
            SendOffer(5);

        }

        private void square6_Click(object sender, EventArgs e)
        {
            SendOffer(6);
        }

        private void square8_Click(object sender, EventArgs e)
        {
            SendOffer(8);
        }

        private void square9_Click(object sender, EventArgs e)
        {
            SendOffer(9);
        }

        private void square11_Click(object sender, EventArgs e)
        {
            SendOffer(11);
        }

        private void square12_Click(object sender, EventArgs e)
        {
            SendOffer(12);
        }

        private void square13_Click(object sender, EventArgs e)
        {
            SendOffer(13);
        }

        private void square14_Click(object sender, EventArgs e)
        {
            SendOffer(14);
        }

        private void square15_Click(object sender, EventArgs e)
        {
            SendOffer(15);
        }

        private void square16_Click(object sender, EventArgs e)
        {
            SendOffer(16);
        }

        private void square18_Click(object sender, EventArgs e)
        {
            SendOffer(18);
        }

        private void square19_Click(object sender, EventArgs e)
        {
            SendOffer(19);
        }

        private void square21_Click(object sender, EventArgs e)
        {
            SendOffer(21);
        }

        private void square23_Click(object sender, EventArgs e)
        {
            SendOffer(23);
        }

        private void square24_Click(object sender, EventArgs e)
        {
            SendOffer(24);
        }

        private void square25_Click(object sender, EventArgs e)
        {
            SendOffer(25);
        }

        private void square26_Click(object sender, EventArgs e)
        {
            SendOffer(26);
        }

        private void square27_Click(object sender, EventArgs e)
        {
            SendOffer(27);
        }

        private void square28_Click(object sender, EventArgs e)
        {
            SendOffer(28);
        }

        private void square29_Click(object sender, EventArgs e)
        {
            SendOffer(29);
        }

        private void square31_Click(object sender, EventArgs e)
        {
            SendOffer(31);
        }

        private void square32_Click(object sender, EventArgs e)
        {
            SendOffer(32);

        }

        private void square34_Click(object sender, EventArgs e)
        {
            SendOffer(34);

        }

        private void square35_Click(object sender, EventArgs e)
        {
            SendOffer(35);
        }

        private void square37_Click(object sender, EventArgs e)
        {
            SendOffer(37);
        }

        private void square39_Click(object sender, EventArgs e)
        {
            SendOffer(39);
        }

        private void SendOffer(int squareId) 
        {
            Player owner = GetOwner(squareId);
            if (owner != null)
            {
                if (!owner.Equals(thisPlayer))
                {
                    if (owner.ableToReceiveOffer)
                    {
                        string money = Interaction.InputBox("Enter the amount that you want to give:", "Trading");
                        if (!string.IsNullOrEmpty(money))
                        {
                            int intMoney;
                            if (int.TryParse(money, out intMoney))
                            {
                                if (intMoney < thisPlayer.money)
                                {
                                    Message message = new Message();
                                    message.messageId = MessageId.AskedSquaresOfferedMoney;
                                    message.id = owner.id;
                                    message.priceOffered = intMoney;
                                    List<Square> s = [Board.squares[squareId]];
                                    message.squaresAsked = s;
                                    Globals.LobbyCtrl.messageQueue.Add(message);
                                }
                                else
                                {
                                    MessageBox.Show("Enter less money than your balance", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Can not convert the input to a number.", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                               
                        }
                        else // not all digits
                        {
                            MessageBox.Show("Enter digits only", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("The owner has a pending offer", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("You are the owner.", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("This square has no owner.", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }

        }

        public void TradingOffer(string text)
        {
            if (RollButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<string>(TradingOffer), text);
            }
            else
            {
                TradeOfferLabel.Text = text;
                TradeOfferLabel.Visible = true;
                SellButton.Visible = true;
                DontSellButton.Visible = true;
            }
        }
        private void SellButton_Click(object sender, EventArgs e)
        {
            SellButton.Visible = false;
            DontSellButton.Visible = false;
            TradeOfferLabel.Visible = false;

            Message message = new Message();
            message.messageId = MessageId.Sell;
            Globals.LobbyCtrl.messageQueue.Add(message);
        }
        private void DontSellButton_Click(object sender, EventArgs e)
        {
            SellButton.Visible = false;
            DontSellButton.Visible = false;
            TradeOfferLabel.Visible = false;

            Message message = new Message();
            message.messageId = MessageId.DontSell;
            Globals.LobbyCtrl.messageQueue.Add(message);
        }
        public void TradeWasMade(string text)
        {
            if (RollButton.InvokeRequired)
            {
                // Call the same method on the UI thread using BeginInvoke
                BeginInvoke(new Action<string>(TradeWasMade), text);
            }
            else
            {
                TradeMadeLabel.Text = text;
                TradeMadeLabel.Visible = true;
                OkButton.Visible = true;
            }
        }
        public Player GetOwner(int squareId)
        {
            foreach (Player p in Globals.players)
            {
                if (p != null)
                {
                    foreach (Square s in p.squaresOwned)
                    {
                        if (s.id == squareId)
                            return p;
                    }
                }
            }
            return null;
        }

        private void PrivateOkButton1_Click(object sender, EventArgs e)
        {
            privateOkClicked.SetResult(true);
        }

        private void PrivateOkButton2_Click(object sender, EventArgs e)
        {
            privateOkClicked.SetResult(true);
        }

        private void PrivateOkButton3_Click(object sender, EventArgs e)
        {
            privateOkClicked.SetResult(true);
        }

        private void PrivateOkButton4_Click(object sender, EventArgs e)
        {
            privateOkClicked.SetResult(true);
        }
    }
}
