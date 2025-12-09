using NewSocket;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MonopolyClient
{
    public class LobbyCtrl
    {
        Socket sendSocket;
        Socket receiveSocket;
        public BlockingCollection<Message> messageQueue;
        public LobbyCtrl() 
        {
            messageQueue = new BlockingCollection<Message>();
        }

        public void Init() // This function is called from the UI thread!
        {
            // Create a blocking collection to receive messages from the UI thread

            // Initialize and start the thread
            Thread thread = new Thread(ControlThread);
            thread.Start();
        }
        public void ControlThread()
        {
            //ClientForm.s_instance.UpdateDebugOutput("Started thread: ControlThread");

            // Wait for messages from the blocking collection
            foreach (Message message in messageQueue.GetConsumingEnumerable())
            {
                //ClientForm.s_instance.UpdateDebugOutput("UI thread sent " + message.messageId);
                switch (message.messageId)
                {
                    case MessageId.Connect:
                        {
                            Connect(message.serverIp);
                            break;
                        }
                    case MessageId.RegisterPlayer:
                        {
                            Register(message.name, message.id);
                            break;
                        }
                    case MessageId.ReadyToStartGame:
                        {
                            Ready();
                            break;
                        }
                    case MessageId.RollDiceClicked:
                        {
                            RollDiceFromClient();
                            break;
                        }
                    case MessageId.RollDiceEnded:
                        {
                            RollDiceEnded();
                            break;
                        }
                    case MessageId.PlayerMovingEnded:
                        {
                            PlayerMovingEnded();
                            break;
                        }
                    case MessageId.OkClicked:
                        {
                            OkClicked();
                            break;
                        }
                    case MessageId.Buy:
                        {
                            Buy();
                            break;
                        }
                    case MessageId.DontBuy:
                        {
                            DontBuy();
                            break;
                        }
                    case MessageId.UseCard:
                        {
                            UseCard();
                            break;
                        }
                    case MessageId.DontUseCard:
                        {
                            DontUseCard();
                            break;
                        }
                    case MessageId.Sell:
                        {
                            Sell();
                            break;
                        }
                    case MessageId.DontSell:
                        {
                            DontSell();
                            break;
                        }
                    case MessageId.SendMessage:
                        {
                            SendMessage(message.message);
                            break;
                        }
                    case MessageId.AskedMoneyOfferedSquares:
                        {
                            AskedMoneyOfferedSquares(message.id, message.priceAsked, message.squaresOffered);
                            break;
                        }
                    case MessageId.AskedMoneySquaresOfferedSquares:
                        {
                            AskedMoneySquaresOfferedSquares(message.id, message.priceAsked, message.squaresAsked, message.squaresOffered);
                            break;
                        }
                    case MessageId.AskedSquaresOfferedMoney:
                        {
                            AskedSquaresOfferedMoney(message.id, message.squaresAsked, message.priceOffered);
                            break;
                        }
                    case MessageId.AskedSquaresOfferedSquares:
                        {
                            AskedSquaresOfferedSquares(message.id, message.squaresAsked, message.squaresOffered);
                            break;
                        }
                    case MessageId.AskedSquaresOfferedSquaresMoney:
                        {
                            AskedSquaresOfferedSquaresMoney(message.id, message.squaresAsked, message.squaresOffered, message.priceOffered);
                            break;
                        }
                }
            }
        }

        public void Connect(string serverIp) // when connecting at first, two sockets are created, receive and send
        {
            bool failed = false;
            try
            {

                //ClientForm.s_instance.UpdateDebugOutput($"Connecting to {serverIp}:12345");
                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sendSocket.Connect(serverIp, 12345); // TO DO - let the user select IP and port
                //ClientForm.s_instance.UpdateDebugOutput($"Connected to {serverIp}:12345 with sendSocket");

                //ClientForm.s_instance.UpdateDebugOutput($"Connecting to {serverIp}:12346");
                receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                receiveSocket.Connect(serverIp, 12346); // TO DO - let the user select IP and port
                //ClientForm.s_instance.UpdateDebugOutput($"Connected to {serverIp}:12346 with receiveSocket");
            }
            catch (Exception ex)
            {
                //ClientForm.s_instance.UpdateDebugOutput("Caught an exception");
                failed = true;
            }
            if (!failed)
            {
                ClientForm.s_instance.ConnectSuccess();
                Thread thread1 = new Thread(ReceiveFromServerThread);
                thread1.Start();
            }
            else
                ClientForm.s_instance.ConnectFailed();

        } 
        public void Register(string name, int id) // this function is called from ControlThread
        {
            try
            {
                byte[] nameBytes = Encoding.UTF8.GetBytes(name);
                byte[] messageBytes = new byte[nameBytes.Length + 2 + sizeof(int)];
                messageBytes[0] = (byte)MessageId.RegisterPlayer;
                messageBytes[1] = (byte)id;
                int port = ((IPEndPoint)receiveSocket.LocalEndPoint).Port;  // since our receive socket exists already, we can get his port
                byte[] portBytes = BitConverter.GetBytes(port);
                Array.Copy(portBytes, 0, messageBytes, 2, portBytes.Length);
                Array.Copy(nameBytes, 0, messageBytes, 2 + portBytes.Length, nameBytes.Length);
                //ClientForm.s_instance.UpdateDebugOutput("Sending RegisterPlayer");
                MySocket.Send(sendSocket, messageBytes);
            }
            catch (Exception ex)
            {
                //ClientForm.s_instance.UpdateDebugOutput("Caught an exception");
            }

            if (receiveSocket == null || !receiveSocket.Connected)
            {
                // failed to connect so let's give the user another chance
                ClientForm.s_instance.RegisterFailure(id);
            }
        }
        public void Ready()
        {
            byte[] buffer = { (byte)MessageId.ReadyToStartGame };
            MySocket.Send(sendSocket, buffer);
        }

        public void ReceiveFromServerThread()
        {
            //ClientForm.s_instance.UpdateDebugOutput("Started thread: ReceiveFromServerThread");
            bool toContinue = true;
            while (toContinue)
            {
                try
                {

                    byte[] data = MySocket.Receive(receiveSocket); // data is full!
                    if (data != null && data.Length >= 1)
                    {
                        MessageId msgID = (MessageId)(data[0]);
                        // Convert the received bytes to a string

                        //ClientForm.s_instance.UpdateDebugOutput("Received data from socket: " + msgID);
                        switch (msgID)
                        {
                            case MessageId.RegisterSuccess:
                                {
                                    if (data.Length >= 3)
                                    {
                                        int id = (int)(data[1]);
                                        string msgTxt = Encoding.UTF8.GetString(data, 2, data.Length - 2);
                                        ClientForm.s_instance.RegisterSuccess(msgTxt, id);
                                    }
                                    break;
                                }
                            case MessageId.RegisterFailure:
                                {
                                    if (data.Length >= 2)
                                    {
                                        int id = (int)(data[1]);
                                        ClientForm.s_instance.RegisterFailure(id);
                                    }
                                    break;
                                }
                            case MessageId.PlayerJoined:
                                {
                                    if (data.Length >= 3)
                                    {
                                        int id = (int)(data[1]);
                                        string msgTxt = Encoding.UTF8.GetString(data, 2, data.Length - 2);
                                        ClientForm.s_instance.PlayerJoined(msgTxt, id);
                                    }
                                    break;
                                }
                            case MessageId.ReadyIsPossible:
                                {
                                    if (data.Length >= 2)
                                    {
                                        int id = (int)(data[1]);
                                        ClientForm.s_instance.ActivateReadyButton(id);
                                    }
                                    break;
                                }

                            case MessageId.ReadySuccess:
                                {
                                    if (data.Length >= 2)
                                    {
                                        int id = (int)(data[1]);
                                        ClientForm.s_instance.ReadySuccess(id);
                                    }
                                    break;

                                }

                            case MessageId.ReadyFailure:
                                {
                                    if (data.Length >= 2)
                                    {
                                        int id = (int)(data[1]);
                                        ClientForm.s_instance.ReadyFailure(id);
                                    }
                                    break;
                                }
                            case MessageId.PlayerIsReady:
                                {
                                    if (data.Length >= 2)
                                    {
                                        int id = (int)(data[1]);
                                        ClientForm.s_instance.PlayerIsReady(id);
                                    }
                                    break;
                                }
                            case MessageId.EnableRegisterInId:
                                {
                                    int id = (int)(data[1]);
                                    ClientForm.s_instance.EnableRegisterInId(id);
                                    break;
                                }
                            case MessageId.DisableReady:
                                {
                                    ClientForm.s_instance.DisableReady();
                                    break;
                                }
                            case MessageId.GameStarted:
                                {
                                    ClientForm.s_instance.GameStarted();
                                    break;
                                }
                            case MessageId.GameHadAlreadyStarted:
                                {
                                    ClientForm.s_instance.GameHadAlreadyStarted();
                                    sendSocket.Close();
                                    receiveSocket.Close();
                                    toContinue = false;
                                    break;
                                }
                            case MessageId.WhoseTurn:
                                {
                                    if (data.Length >= 2)
                                    {
                                        int id = (int)(data[1]);
                                        ClientForm.s_instance.WhoseTurn(id);
                                    }
                                    break;
                                }
                            case MessageId.RollDiceAvailable:
                                {
                                    ClientForm.s_instance.RollDiceAvailable();
                                    break;
                                }
                            case MessageId.RollDiceSuccess:
                                {
                                    //ClientForm.s_instance.UpdateDebugOutput("RollDiceSuccess");
                                    break;
                                }
                            case MessageId.RollDiceFailure:
                                {
                                    ClientForm.s_instance.RollDiceFailure();
                                    break;

                                }
                            case MessageId.SendDicesValue:
                                {
                                    if (data.Length >= 3)
                                    {
                                        int dice1 = (int)data[1];
                                        int dice2 = (int)data[2];
                                        ClientForm.s_instance.SendDicesValue(dice1, dice2);
                                    }
                                    break;
                                }
                            case MessageId.UpdateMessageLabelForEveryone:
                                {
                                    if (data.Length >= 2)
                                    {
                                        string msgTxt = Encoding.UTF8.GetString(data, 1, data.Length - 1);
                                        ClientForm.s_instance.UpdateMessageLabelForEveryone(msgTxt);
                                    }
                                    break;
                                }
                            case MessageId.BuySquareOffer:
                                {
                                    int squareId = (int)data[1];
                                    string msgTxt = Encoding.UTF8.GetString(data, 2, data.Length - 2);
                                    ClientForm.s_instance.BuySquareOffer(squareId, msgTxt);
                                    break;
                                }
                            case MessageId.BuyHouseOffer:
                                {
                                    int squareId = (int)data[1];
                                    string msgTxt = Encoding.UTF8.GetString(data, 2, data.Length - 2);
                                    ClientForm.s_instance.BuyHouseOffer(squareId, msgTxt);
                                    break;
                                }
                            case MessageId.AmountOfOutOfJailCards:
                                {
                                    int id = (int)data[1];
                                    int amount = (int)data[2];
                                    ClientForm.s_instance.AmountOfOutOfJailCards(id, amount);
                                    break;
                                }
                            case MessageId.UseOutOfJailCardOffer:
                                {
                                    string msgTxt = Encoding.UTF8.GetString(data, 1, data.Length - 1);
                                    ClientForm.s_instance.UseOutOfJailCardOffer(msgTxt);
                                    break;
                                }
                            case MessageId.UpdateMoneyLabel:
                                {
                                    int id = (int)data[1];
                                    int money = BitConverter.ToInt32(data, 2);
                                    ClientForm.s_instance.UpdateMoneyLabel(id, money);
                                    break;
                                }
                            case MessageId.UpdateSquareWithOwner:
                                {
                                    int ownerId = (int)data[1];
                                    int squareId = (int)data[2];
                                    ClientForm.s_instance.UpdateSquareWithOwner(ownerId, squareId);
                                    TradeForm.s_instance.UpdateSquareWithOwner(ownerId, squareId);
                                    break;
                                }
                            case MessageId.RemoveSquareFromOwner:
                                {
                                    int squareId = (int)data[1];
                                    int ownerId = (int)data[2];
                                    ClientForm.s_instance.RemoveSquareFromOwner(squareId, ownerId);
                                    TradeForm.s_instance.RemoveSquareFromOwner(squareId);
                                    break;
                                }
                            case MessageId.HouseAdded:
                                {
                                    int squareId = (int)data[1];
                                    int amount = (int)data[2];
                                    ClientForm.s_instance.HouseAdded(squareId, amount);
                                    TradeForm.s_instance.HouseAdded(squareId);
                                    break;
                                }
                            case MessageId.PlayerNeedsToGoToSquare:
                                {
                                    int id = (int)data[1];
                                    int squareId = (byte)data[2];
                                    ClientForm.s_instance.PlayerNeedsToGoToSquare(id, squareId);
                                    break;

                                }
                            case MessageId.MovePlayer:
                                {
                                    int id = (int)data[1];
                                    int steps = (int)data[2];
                                    ClientForm.s_instance.MovePlayer(id, steps);
                                    break;
                                }

                            case MessageId.TradingOffer:
                                {
                                    string text = Encoding.UTF8.GetString(data, 1, data.Length - 1);
                                    ClientForm.s_instance.TradingOffer(text);
                                    break;

                                }
                            case MessageId.UpdateMessageInChat:
                                {
                                    int id = (int)data[1];
                                    string message = Encoding.UTF8.GetString(data, 2, data.Length - 2);
                                    ChatForm.s_instance.AddMessage(id, message);
                                    break;
                                }
                            case MessageId.EnableSendingOffersToPlayer:
                                {
                                    int playerId = (int)data[1];
                                    Globals.players[playerId].ableToReceiveOffer = true;
                                    break;
                                }
                            case MessageId.PlayerIsNotAbleToReceiveOffer:
                                {
                                    int id = (int)data[1];
                                    Globals.players[id].ableToReceiveOffer = false;
                                    break;
                                }
                            case MessageId.TradeWasMade:
                                {
                                    string text = Encoding.UTF8.GetString(data, 1, data.Length - 1);
                                    ClientForm.s_instance.TradeWasMade(text);
                                    break;
                                }
                            case MessageId.UpdatePrivateMessageLabel:
                                {
                                    string text = Encoding.UTF8.GetString(data, 1, data.Length - 1);
                                    ClientForm.s_instance.privateMessageQueue.Add(text);
                                    break;
                                }
                            case MessageId.RemovePlayerDisconnectedFromClients:
                                {
                                    int playerId = (int)(data[1]);
                                    TradeForm.s_instance.RemovePlayerDisconnectedFromClients(playerId);
                                    ClientForm.s_instance.RemovePlayerDisconnectedFromClients(playerId);
                                    break;
                                }
                            case MessageId.MakePlayerSpectateInClients:
                                {
                                    int playerId = (int)data[1];
                                    TradeForm.s_instance.MakePlayerSpectateInClients(playerId);
                                    ClientForm.s_instance.MakePlayerSpectateInClients(playerId);
                                    break;
                                }

                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!receiveSocket.Connected)
                        break;
                }
            }
        }
        public void RollDiceFromClient()
        {
            byte[] buffer = { (byte)MessageId.RollDiceClicked };
            MySocket.Send(sendSocket, buffer);
        } // here
        public void RollDiceEnded()
        {
            byte[] buffer = { (byte)MessageId.RollDiceEnded };
            MySocket.Send(sendSocket, buffer);
        }
        public void PlayerMovingEnded()
        {
            byte[] buffer = { (byte)MessageId.PlayerMovingEnded };
            MySocket.Send(sendSocket, buffer);
        }
        public void OkClicked()
        {
            byte[] buffer = { (byte)MessageId.OkClicked };
            MySocket.Send(sendSocket, buffer);
        }
        public void Buy()
        {
            byte[] buffer = { (byte)MessageId.Buy };
            MySocket.Send(sendSocket, buffer);
        }
        public void DontBuy()
        {
            byte[] buffer = { (byte)MessageId.DontBuy };
            MySocket.Send(sendSocket, buffer);
        }
        public void UseCard()
        {
            byte[] buffer = { (byte)MessageId.UseCard };
            MySocket.Send(sendSocket, buffer);
        }
        public void DontUseCard()
        {
            byte[] buffer = { (byte)MessageId.DontUseCard };
            MySocket.Send(sendSocket, buffer);
        }
        public void SendMessage(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] buffer = new byte[messageBytes.Length + 1];
            buffer[0] = (byte)MessageId.SendMessage;
            Array.Copy(messageBytes, 0, buffer, 1, messageBytes.Length);
            MySocket.Send(sendSocket, buffer);
        }
        public void AskedMoneyOfferedSquares(int id, int priceAsked, List<Square> squaresOffered)
        {
            byte[] priceBytes = BitConverter.GetBytes(priceAsked);
            byte[] buffer = new byte[squaresOffered.Count + priceBytes.Length + 2];
            buffer[0] = (byte)MessageId.AskedMoneyOfferedSquares;
            buffer[1] = (byte)id;
            Array.Copy(priceBytes, 0, buffer, 2, priceBytes.Length);
            for (int i = 0; i < squaresOffered.Count; i++)
            {
                buffer[priceBytes.Length + 2 + i] = (byte)squaresOffered[i].id;
            }
            MySocket.Send(sendSocket, buffer);
        }
        public void AskedMoneySquaresOfferedSquares(int id, int priceAsked, List<Square> squaresAsked, List<Square> squaresOffered)
        {
            byte[] priceBytes = BitConverter.GetBytes(priceAsked);
            byte[] buffer = new byte[squaresAsked.Count + squaresOffered.Count + priceBytes.Length + 2];
            buffer[0] = (byte)MessageId.AskedMoneySquaresOfferedSquares;
            buffer[1] = (byte)id;
            Array.Copy(priceBytes, 0, buffer, 2, priceBytes.Length);
            for (int i = 0; i < squaresAsked.Count; i++)
            {
                buffer[priceBytes.Length + 2 + i] = (byte)squaresAsked[i].id;
            }
            for (int i = 0; i < squaresOffered.Count; i++)
            {
                buffer[priceBytes.Length + 2 + squaresAsked.Count + i] = (byte)squaresOffered[i].id;
            }
            MySocket.Send(sendSocket, buffer);
        }
        public void AskedSquaresOfferedMoney(int id, List<Square> squaresAsked, int priceOffered)
        {
            byte[] priceBytes = BitConverter.GetBytes(priceOffered);
            byte[] buffer = new byte[squaresAsked.Count + priceBytes.Length + 2];
            buffer[0] = (byte)MessageId.AskedSquaresOfferedMoney;
            buffer[1] = (byte)id;
            Array.Copy(priceBytes, 0, buffer, 2, priceBytes.Length);
            for (int i = 0; i < squaresAsked.Count; i++)
            {
                buffer[priceBytes.Length + 2 + i] = (byte)squaresAsked[i].id;
            }
            MySocket.Send(sendSocket, buffer);
        }
        public void AskedSquaresOfferedSquares(int id, List<Square> squaresAsked, List<Square> squaresOffered)
        {
            byte[] buffer = new byte[squaresAsked.Count + squaresOffered.Count + 2];
            buffer[0] = (byte)MessageId.AskedSquaresOfferedSquares;
            buffer[1] = (byte)id;
            for (int i = 0; i < squaresAsked.Count; i++)
            {
                buffer[2 + i] = (byte)squaresAsked[i].id;
            }
            for (int i = 0; i < squaresOffered.Count; i++)
            {
                buffer[2 + squaresAsked.Count + i] = (byte)squaresOffered[i].id;
            }
            MySocket.Send(sendSocket, buffer);
        }
        public void AskedSquaresOfferedSquaresMoney(int id, List<Square> squaresAsked, List<Square> squaresOffered, int priceOffered)
        {
            byte[] priceBytes = BitConverter.GetBytes(priceOffered);
            byte[] buffer = new byte[squaresAsked.Count + squaresOffered.Count + priceBytes.Length + 2];
            buffer[0] = (byte)MessageId.AskedSquaresOfferedSquaresMoney;
            buffer[1] = (byte)id;
            Array.Copy(priceBytes, 0, buffer, 2, priceBytes.Length);
            for (int i = 0; i < squaresAsked.Count; i++)
            {
                buffer[priceBytes.Length + 2 + i] = (byte)squaresAsked[i].id;
            }
            for (int i = 0; i < squaresOffered.Count; i++)
            {
                buffer[priceBytes.Length + 2 + squaresAsked.Count + i] = (byte)squaresOffered[i].id;
            }
            MySocket.Send(sendSocket, buffer);
        }
        public void Sell()
        {
            byte[] buffer = { (byte)MessageId.Sell };
            MySocket.Send(sendSocket, buffer);
        }
        public void DontSell()
        {
            byte[] buffer = { (byte)MessageId.DontSell };
            MySocket.Send(sendSocket, buffer);
        }




























    }
}
