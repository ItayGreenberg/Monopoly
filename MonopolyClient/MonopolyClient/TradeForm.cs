using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonopolyClient
{
    public partial class TradeForm : Form
    {
        public static TradeForm s_instance = null;
        private readonly object Lock = new object();
        private Button[] squaresButtons = new Button[40];
        private Label[] squaresOwners = new Label[40]; // squarre Id, ownerName
        private Label[] offerOrRequestText = new Label[40];
        private Button[] nameButtons;
        private int myId;
        private int playerChosenId = -1;
        private bool[] squareClicked = new bool[40];


        private List<Square> squaresOffered = new List<Square>();
        private List<Square> squaresAsked = new List<Square>();
        private int priceOffered;
        private int priceAsked;
        public TradeForm()
        {
            InitializeComponent();
            this.FormClosing += TradeForm_FormClosing;
            s_instance = this;
            myId = ClientForm.s_instance.thisPlayer.id;
            InitSquaresButtons();
            InitOwnerNames();
            InitOfferOrRequestText();
            InitNameButtons();

            
        }

        private void InitNameButtons()
        {
            nameButtons = new Button[4];
            nameButtons[0] = Player1NameButton;
            nameButtons[1] = Player2NameButton;
            nameButtons[2] = Player3NameButton;
            nameButtons[3] = Player4NameButton;
            for (int i = 0; i < 4; i++)
            {
                if (Globals.players[i] != null && Globals.players[i].id != myId)
                {
                    nameButtons[i].Text = Globals.players[i].name;
                    nameButtons[i].Visible = true;
                }
            }
        }
        private void InitSquaresButtons()
        {
            squaresButtons[1] = square1;
            squaresButtons[3] = square3;
            squaresButtons[5] = square5;
            squaresButtons[6] = square6;
            squaresButtons[8] = square8;
            squaresButtons[9] = square9;
            squaresButtons[11] = square11;
            squaresButtons[12] = square12;
            squaresButtons[13] = square13;
            squaresButtons[14] = square14;
            squaresButtons[15] = square15;
            squaresButtons[16] = square16;
            squaresButtons[18] = square18;
            squaresButtons[19] = square19;
            squaresButtons[21] = square21;
            squaresButtons[23] = square23;
            squaresButtons[24] = square24;
            squaresButtons[25] = square25;
            squaresButtons[26] = square26;
            squaresButtons[27] = square27;
            squaresButtons[28] = square28;
            squaresButtons[29] = square29;
            squaresButtons[31] = square31;
            squaresButtons[32] = square32;
            squaresButtons[34] = square34;
            squaresButtons[35] = square35;
            squaresButtons[37] = square37;
            squaresButtons[39] = square39;


            for (int i = 0; i < squaresButtons.Length; i++) //size, text
            {
                if (squaresButtons[i] != null)
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
                    squaresButtons[i].FlatStyle = FlatStyle.Flat;
                    squaresButtons[i].FlatAppearance.BorderColor = Color.Black;
                    squaresButtons[i].FlatAppearance.BorderSize = 1;
                }

            }

        }
        private void InitOwnerNames()
        {
            squaresOwners[1] = OwnerName1;
            squaresOwners[3] = OwnerName3;
            squaresOwners[5] = OwnerName5;
            squaresOwners[6] = OwnerName6;
            squaresOwners[8] = OwnerName8;
            squaresOwners[9] = OwnerName9;
            squaresOwners[11] = OwnerName11;
            squaresOwners[12] = OwnerName12;
            squaresOwners[13] = OwnerName13;
            squaresOwners[14] = OwnerName14;
            squaresOwners[15] = OwnerName15;
            squaresOwners[16] = OwnerName16;
            squaresOwners[18] = OwnerName18;
            squaresOwners[19] = OwnerName19;
            squaresOwners[21] = OwnerName21;
            squaresOwners[23] = OwnerName23;
            squaresOwners[24] = OwnerName24;
            squaresOwners[25] = OwnerName25;
            squaresOwners[26] = OwnerName26;
            squaresOwners[27] = OwnerName27;
            squaresOwners[28] = OwnerName28;
            squaresOwners[29] = OwnerName29;
            squaresOwners[31] = OwnerName31;
            squaresOwners[32] = OwnerName32;
            squaresOwners[34] = OwnerName34;
            squaresOwners[35] = OwnerName35;
            squaresOwners[37] = OwnerName37;
            squaresOwners[39] = OwnerName39;
        }
        private void InitOfferOrRequestText()
        {
            offerOrRequestText[1] = OfferOrRequestText1;
            offerOrRequestText[3] = OfferOrRequestText3;
            offerOrRequestText[5] = OfferOrRequestText5;
            offerOrRequestText[6] = OfferOrRequestText6;
            offerOrRequestText[8] = OfferOrRequestText8;
            offerOrRequestText[9] = OfferOrRequestText9;
            offerOrRequestText[11] = OfferOrRequestText11;
            offerOrRequestText[12] = OfferOrRequestText12;
            offerOrRequestText[13] = OfferOrRequestText13;
            offerOrRequestText[14] = OfferOrRequestText14;
            offerOrRequestText[15] = OfferOrRequestText15;
            offerOrRequestText[16] = OfferOrRequestText16;
            offerOrRequestText[18] = OfferOrRequestText18;
            offerOrRequestText[19] = OfferOrRequestText19;
            offerOrRequestText[21] = OfferOrRequestText21;
            offerOrRequestText[23] = OfferOrRequestText23;
            offerOrRequestText[24] = OfferOrRequestText24;
            offerOrRequestText[25] = OfferOrRequestText25;
            offerOrRequestText[26] = OfferOrRequestText26;
            offerOrRequestText[27] = OfferOrRequestText27;
            offerOrRequestText[28] = OfferOrRequestText28;
            offerOrRequestText[29] = OfferOrRequestText29;
            offerOrRequestText[31] = OfferOrRequestText31;
            offerOrRequestText[32] = OfferOrRequestText32;
            offerOrRequestText[34] = OfferOrRequestText34;
            offerOrRequestText[35] = OfferOrRequestText35;
            offerOrRequestText[37] = OfferOrRequestText37;
            offerOrRequestText[39] = OfferOrRequestText39;
        }
        public void UpdateSquareWithOwner(int ownerId, int squareId)
        {
            lock (Lock)
            {
                if (PriceOfferedLabel.InvokeRequired)
                {
                    // Call the same method on the UI thread using BeginInvoke
                    BeginInvoke(new Action<int, int>(UpdateSquareWithOwner), ownerId, squareId);
                }
                else
                {
                    squaresOwners[squareId].Text = Globals.players[ownerId].name;

                    if (ownerId == playerChosenId)
                    {
                        offerOrRequestText[squareId].Text = "Request";
                        offerOrRequestText[squareId].Visible = true;
                    }
                    if (ownerId == myId && playerChosenId != -1)
                    {
                        offerOrRequestText[squareId].Text = "Offer";
                        offerOrRequestText[squareId].Visible = true;
                    }
                }
            }
        }
        public void RemoveSquareFromOwner(int squareId)
        {
            lock (Lock)
            {
                if (PriceOfferedLabel.InvokeRequired)
                {
                    // Call the same method on the UI thread using BeginInvoke
                    BeginInvoke(new Action<int>(RemoveSquareFromOwner), squareId);
                }
                else
                {
                    squaresOwners[squareId].Text = "";
                    if (playerChosenId != -1)
                    {
                        offerOrRequestText[squareId].Visible = false;
                    }
                    bool foundError = false;
                    foreach (Square s in squaresOffered)
                    {
                        if (s.id == squareId)
                        {
                            MessageBox.Show("A square that you offered has been removed from the owner. The trade is canceled", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            foundError = true;
                            break;
                        }
                    }
                    if (!foundError)
                    {
                        foreach (Square s in squaresAsked)
                        {
                            if (s.id == squareId)
                            {
                                MessageBox.Show("A square that you requested has been removed from you. The trade is canceled.", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        }
                    }
                }
            }
        }
        public void HouseAdded(int squareId)
        {
            lock (Lock)
            {
                if (PriceOfferedLabel.InvokeRequired)
                {
                    // Call the same method on the UI thread using BeginInvoke
                    BeginInvoke(new Action<int>(HouseAdded), squareId);
                }
                else
                {
                    if (playerChosenId != -1)
                    {
                        offerOrRequestText[squareId].Visible = false;
                    }
                    bool foundError = false;
                    foreach (Square s in squaresOffered)
                    {
                        if (s is StreetSquare)
                        {
                            if (!NoHousesInStreet(s))
                            {
                                MessageBox.Show("A stret of a square that you offered has a house. The trade is canceled", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                foundError = true;
                                break;
                            }
                        }
                    }
                    if (!foundError)
                    {
                        foreach (Square s in squaresAsked)
                        {
                            if (s is StreetSquare)
                            {
                                if (!NoHousesInStreet(s))
                                {
                                    MessageBox.Show("A street of a square that you requested has a house. The trade is canceled.", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void PriceOfferedButton_Click(object sender, EventArgs e)
            {
                if (PriceAskedLabel.Text != "")
                {
                    MessageBox.Show("You have already asked money. You can't ask and give money.", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string money = Interaction.InputBox("Enter the amount that you want to give:", "Trading");
                    if (!string.IsNullOrEmpty(money))
                    {
                        int intMoney;
                        if (int.TryParse(money, out intMoney))
                        {
                            if (intMoney < Globals.players[myId].money)
                            {
                                PriceOfferedLabel.Text = intMoney.ToString();
                                priceOffered = intMoney;
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
                }
            }
        private void PriceAskedButton_Click(object sender, EventArgs e)
        {
            if (PriceOfferedLabel.Text != "")
            {
                MessageBox.Show("You have already offered money. You can't ask and give money.", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string money = Interaction.InputBox("Enter the amount that you want to give:", "Trading");
                if (!string.IsNullOrEmpty(money))
                {
                    int intMoney;
                    if (int.TryParse(money, out intMoney))
                    {
                        if (intMoney < Globals.players[myId].money)
                        {
                            PriceAskedLabel.Text = intMoney.ToString();
                            priceAsked = intMoney;
                        }
                        else
                        {
                            MessageBox.Show("Enter less money his balance", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Can not convert the input to a number.", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        private void Player1NameButton_Click(object sender, EventArgs e)
        {
            lock (Lock)
            {
                if (Globals.players[0].ableToReceiveOffer)
                {
                    PlayerToTradeWithTitle.Visible = false;
                    playerChosenId = 0;
                    for (int i = 0; i < nameButtons.Length; i++)
                    {
                        nameButtons[i].Visible = false;
                    }
                    EnableTrade();
                }
                else
                {
                    MessageBox.Show("the player alread has a pending offer", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void Player2NameButton_Click(object sender, EventArgs e)
        {
            lock (Lock)
            {
                if (Globals.players[1].ableToReceiveOffer)
                {
                    PlayerToTradeWithTitle.Visible = false;
                    playerChosenId = 1;
                    for (int i = 0; i < nameButtons.Length; i++)
                    {
                        nameButtons[i].Visible = false;
                    }
                    EnableTrade();
                }
                else
                {
                    MessageBox.Show("the player alread has a pending offer", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void Player3NameButton_Click(object sender, EventArgs e)
        {
            lock (Lock)
            {
                if (Globals.players[2].ableToReceiveOffer)
                {
                    PlayerToTradeWithTitle.Visible = false;
                    playerChosenId = 2;
                    for (int i = 0; i < nameButtons.Length; i++)
                    {
                        nameButtons[i].Visible = false;
                    }
                    EnableTrade();
                }
                else
                {
                    MessageBox.Show("the player alread has a pending offer", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void Player4NameButton_Click(object sender, EventArgs e)
        {
            lock (Lock)
            {
                if (Globals.players[3].ableToReceiveOffer)
                {
                    PlayerToTradeWithTitle.Visible = false;
                    playerChosenId = 3;
                    for (int i = 0; i < nameButtons.Length; i++)
                    {
                        nameButtons[i].Visible = false;
                    }
                    EnableTrade();
                }
                else
                {
                    MessageBox.Show("the player alread has a pending offer", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void EnableTrade()
        {
            PriceOfferedButton.Visible = true;
            PriceOfferedLabel.Visible = true;

            PriceAskedButton.Visible = true;
            PriceAskedLabel.Visible = true;



            SquaresOfferedTitle.Visible = true;
            SquaresOffered.Visible = true;

            SquaresAskedTitle.Visible = true;
            SquaresAsked.Visible = true;

            SendTradeButton.Visible = true;
            RestartTrade.Visible = true;
            List<Square> mySquares = Globals.players[myId].GetSquares();
            List<Square> traderSquares = Globals.players[playerChosenId].GetSquares();

            foreach (Square s in mySquares)
            {
                int squareId = s.id;
                if (s is StreetSquare)
                {
                    if (NoHousesInStreet(s))
                    {
                        offerOrRequestText[squareId].Text = "Offer";
                        offerOrRequestText[squareId].Visible = true;
                    }
                }
                else
                {
                    offerOrRequestText[squareId].Text = "Offer";
                    offerOrRequestText[squareId].Visible = true;
                }
            }
            foreach (Square s in traderSquares)
            {
                int squareId = s.id;
                if (s is StreetSquare)
                {
                    if (NoHousesInStreet(s))
                    {
                        offerOrRequestText[squareId].Text = "Request";
                        offerOrRequestText[squareId].Visible = true;
                    }
                }
                else
                {
                    offerOrRequestText[squareId].Text = "Request";
                    offerOrRequestText[squareId].Visible = true;
                }
            }
        }
        private bool NoHousesInStreet(Square square)
        {
            StreetSquare s = (StreetSquare)square;
            for(int i = 0; i < 40; i++)
            {
                PictureBox pictureBox = ClientForm.s_instance.housesPictures[i];
                if (pictureBox != null)
                {
                    if (Board.squares[i] is StreetSquare)
                    {
                        if (((StreetSquare)Board.squares[i]).streetId == s.streetId)
                        {
                            if (pictureBox.Visible)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        private void SquareClicked(int squareId)
        {
            lock (Lock)
            {
                if (offerOrRequestText[squareId].Visible)
                {
                    if (!squareClicked[squareId])
                    {
                        if (offerOrRequestText[squareId].Text == "Offer")
                        {
                            if (squaresOffered.Count <= 2)
                            {
                                squaresOffered.Add(Board.squares[squareId]);
                                SquaresOffered.Text += Board.squares[squareId].name + Environment.NewLine;
                            }
                            else
                            {
                                MessageBox.Show("The most that you can offer is 3 squares", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            if (squaresAsked.Count <= 2)
                            {
                                squaresAsked.Add(Board.squares[squareId]);
                                SquaresAsked.Text += Board.squares[squareId].name + Environment.NewLine;
                            }
                            else
                            {
                                MessageBox.Show("The most that you can ask for is 3 squares", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        squareClicked[squareId] = true;
                    }
                    else
                    {
                        MessageBox.Show("You have alrady clicked this square", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Not available for traid", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void square1_Click(object sender, EventArgs e)
        {
            SquareClicked(1);
        }   
        private void square3_Click(object sender, EventArgs e)
        {
            SquareClicked(3);
        }
        private void square5_Click(object sender, EventArgs e)
        {
            SquareClicked(5);
        }
        private void square6_Click(object sender, EventArgs e)
        {
            SquareClicked(6);
        }
        private void square8_Click(object sender, EventArgs e)
        {
            SquareClicked(8);
        }
        private void square9_Click(object sender, EventArgs e)
        {
            SquareClicked(9);
        }
        private void square11_Click(object sender, EventArgs e)
        {
            SquareClicked(11);
        }
        private void square12_Click(object sender, EventArgs e)
        {
            SquareClicked(12);
        }
        private void square13_Click(object sender, EventArgs e)
        {
            SquareClicked(13);
        }
        private void square14_Click(object sender, EventArgs e)
        {
            SquareClicked(14);
        }
        private void square15_Click(object sender, EventArgs e)
        {
            SquareClicked(15);
        }
        private void square16_Click(object sender, EventArgs e)
        {
            SquareClicked(16);
        }
        private void square18_Click(object sender, EventArgs e)
        {
            SquareClicked(18);
        }
        private void square19_Click(object sender, EventArgs e)
        {
            SquareClicked(19);
        }
        private void square21_Click(object sender, EventArgs e)
        {
            SquareClicked(21);
        }
        private void square23_Click(object sender, EventArgs e)
        {
            SquareClicked(23);
        }
        private void square24_Click(object sender, EventArgs e)
        {
            SquareClicked(24);
        }
        private void square25_Click(object sender, EventArgs e)
        {
            SquareClicked(25);
        }
        private void square26_Click(object sender, EventArgs e)
        {
            SquareClicked(26);
        }
        private void square27_Click(object sender, EventArgs e)
        {
            SquareClicked(27);
        }
        private void square28_Click(object sender, EventArgs e)
        {
            SquareClicked(28);
        }
        private void square29_Click(object sender, EventArgs e)
        {
            SquareClicked(29);
        }
        private void square31_Click(object sender, EventArgs e)
        {
            SquareClicked(31);
        }
        private void square32_Click(object sender, EventArgs e)
        {
            SquareClicked(32);
        }
        private void square34_Click(object sender, EventArgs e)
        {
            SquareClicked(34);
        }
        private void square35_Click(object sender, EventArgs e)
        {
            SquareClicked(35);
        }
        private void square37_Click(object sender, EventArgs e)
        {
            SquareClicked(37);
        }
        private void square39_Click(object sender, EventArgs e)
        {
            SquareClicked(39);
        }
        private void SendTradeButton_Click(object sender, EventArgs e)
        {
            lock (Lock)
            {
                bool tradeIsPossible = Globals.players[playerChosenId].ableToReceiveOffer;
                if (Globals.players[myId].pictureBox.Visible && Globals.players[playerChosenId].pictureBox.Visible)
                {
                    if (Globals.players[myId].money <= priceOffered || Globals.players[playerChosenId].money <= priceAsked)
                    {
                        tradeIsPossible = false;
                    }

                }
                else
                    tradeIsPossible = false;


                if (!tradeIsPossible)
                {
                    MessageBox.Show("Error in sending the offer. Please try again", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Message message = new Message();
                    message.id = playerChosenId;
                    if (PriceOfferedLabel.Text == "" && PriceAskedLabel.Text != "") // asked for money
                    {
                        if (squaresOffered.Count == 0 && squaresAsked.Count != 0) // asked for squares and money and didnt offere anything - error
                        {
                            MessageBox.Show("Error in sending the offer. Please try again", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (squaresOffered.Count != 0 && squaresAsked.Count == 0) // asked for money and sent squares - ok
                        {
                            message.messageId = MessageId.AskedMoneyOfferedSquares;
                            message.squaresOffered = squaresOffered;
                            message.priceAsked = int.Parse(PriceAskedLabel.Text);
                            Globals.LobbyCtrl.messageQueue.Add(message);
                        }
                        else if (squaresOffered.Count != 0 && squaresAsked.Count != 0) // asked for money and squares and gave squares - ok
                        {
                            message.messageId = MessageId.AskedMoneySquaresOfferedSquares;
                            message.squaresOffered = squaresOffered;
                            message.squaresAsked = squaresAsked;
                            message.priceAsked = int.Parse(PriceAskedLabel.Text);
                            Globals.LobbyCtrl.messageQueue.Add(message);
                        }
                        else // no squares offered and asked - error
                        {
                            MessageBox.Show("Error in sending the offer. Please try again", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else if (PriceOfferedLabel.Text != "" && PriceAskedLabel.Text == "")
                    {
                        if (squaresOffered.Count == 0 && squaresAsked.Count != 0) // asked for squares and gave money - ok
                        {
                            message.messageId = MessageId.AskedSquaresOfferedMoney;
                            message.squaresAsked = squaresAsked;
                            message.priceOffered = int.Parse(PriceOfferedLabel.Text);
                            Globals.LobbyCtrl.messageQueue.Add(message);
                        }
                        else if (squaresOffered.Count != 0 && squaresAsked.Count == 0) // only offered - error
                        {
                            MessageBox.Show("Error in sending the offer. Please try again", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (squaresOffered.Count != 0 && squaresAsked.Count != 0) // asked for squares and gave money and squares - ok 
                        {
                            message.messageId = MessageId.AskedSquaresOfferedSquaresMoney;
                            message.squaresAsked = squaresAsked;
                            message.priceOffered = int.Parse(PriceOfferedLabel.Text);
                            message.squaresOffered = squaresOffered;
                            Globals.LobbyCtrl.messageQueue.Add(message);
                        }
                        else // no squares offered and asked - error
                        {
                            MessageBox.Show("Error in sending the offer. Please try again", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else // no money offered and asked
                    {
                        if (squaresOffered.Count != 0 && squaresAsked.Count != 0) // only squares traded - ok
                        {
                            message.messageId = MessageId.AskedSquaresOfferedSquares;
                            message.squaresAsked = squaresAsked;
                            message.squaresOffered = squaresOffered;
                            Globals.LobbyCtrl.messageQueue.Add(message);
                        }
                        else // nothing offered - error
                        {
                            MessageBox.Show("Error in sending the offer. Please try again", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                RestartTradeStatsAndAnimation();
            }
        }
        private void RestartTrade_Click(object sender, EventArgs e)
        {
            RestartTradeStatsAndAnimation();
        }
        private void RestartTradeStatsAndAnimation()
        {
            lock (Lock)
            {
                playerChosenId = -1;
                squareClicked = new bool[40];
                squaresOffered = new List<Square>();
                squaresAsked = new List<Square>();
                priceOffered = 0;
                priceAsked = 0;

                foreach (Label l in offerOrRequestText)
                {
                    if (l != null)
                    {
                        l.Visible = false;
                        l.Text = "";
                    }
                }

                PlayerToTradeWithTitle.Visible = true;



                PriceOfferedButton.Visible = false;
                PriceOfferedLabel.Visible = false;
                SquaresOfferedTitle.Visible = false;
                SquaresOffered.Visible = false;


                PriceAskedButton.Visible = false;
                PriceAskedLabel.Visible = false;
                SquaresAskedTitle.Visible = false;
                SquaresAsked.Visible = false;

                PriceOfferedLabel.Text = "";
                PriceAskedLabel.Text = "";
                SquaresAsked.Text = "";
                SquaresOffered.Text = "";


                for (int i = 0; i < 4; i++)
                {
                    if (Globals.players[i] != null && Globals.players[i].id != myId)
                    {
                        nameButtons[i].Text = Globals.players[i].name;
                        nameButtons[i].Visible = true;
                    }
                }
                RestartTrade.Visible = false;
                SendTradeButton.Visible = false;
            }
        }
        public void RemovePlayerDisconnectedFromClients(int playerId)
        {
            lock (Lock)
            {
                if (PriceOfferedLabel.InvokeRequired)
                {
                    // Call the same method on the UI thread using BeginInvoke
                    BeginInvoke(new Action<int>(RemovePlayerDisconnectedFromClients), playerId);
                }
                else
                {
                    nameButtons[playerId].Visible = false;
                    if (playerChosenId == playerId)
                    {
                        MessageBox.Show("The player that you are trading with is not in the game anymore.", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RestartTradeStatsAndAnimation();
                    }                  
                    List<Square> squaresOwned = Globals.players[playerId].GetSquares();
                    foreach (Square s in squaresOwned)
                    {
                        int squareId = s.id;
                        squaresOwners[squareId].Text = "";
                    }
                }
            }

        }
        public void MakePlayerSpectateInClients(int playerId)
        {
            lock (Lock)
            {
                if (PriceOfferedLabel.InvokeRequired)
                {
                    // Call the same method on the UI thread using BeginInvoke
                    BeginInvoke(new Action<int>(MakePlayerSpectateInClients), playerId);
                }
                else
                {
                    nameButtons[playerId].Visible = false;
                    if (playerChosenId == playerId)
                    {
                        MessageBox.Show("The player that you are trading with is not in the game anymore.", "Monopoly", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RestartTradeStatsAndAnimation();
                    }
                    List<Square> squaresOwned = Globals.players[playerId].GetSquares();
                    foreach (Square s in squaresOwned)
                    {
                        int squareId = s.id;
                        squaresOwners[squareId].Text = "";
                    }
                }
            }
        }
        private void TradeForm_FormClosing(object sender, FormClosingEventArgs e)
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
