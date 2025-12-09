using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections;
using System.Diagnostics;

namespace MonopolyServer
{
    public class LobbyCtrl
    {
        ConcurrentBag<Socket> receiveFromClientSockets;
        ConcurrentBag<Socket> sendToClientSockets;
        private readonly object lockForRegister = new object();
        private readonly object lockForReady = new object();
        private readonly object lockForStartGame = new object();
        private readonly object lockForOkClicked = new object();
        private readonly object lockForMovingEnded = new object();
        private readonly object lockForRollDiceEnded = new object();
        private readonly object lockForTradingOffer = new object();

        public bool ableToSendClickRollDice = false;
        public bool ableToSendRollDiceEnded = false;
        public bool ableToSendMovingEnded = false;
        public bool ableToSendClickedOk = false;
        public bool ableToSendBuyOrDontBuyClicked = false;
        public bool ableToSendUseOrDontUseClicked = false;



        public LobbyCtrl() 
        {
            receiveFromClientSockets = new ConcurrentBag<Socket>();
            sendToClientSockets = new ConcurrentBag<Socket>();
            
        }
        public void StartServer()
        {
            Thread thread1 = new Thread(listenerforReceiveThread);
            thread1.Start();
            Thread thread2 = new Thread(listenerforSendThread);
            thread2.Start();
        }

        

        public void listenerforReceiveThread()
        {
            ServerForm.s_instance.UpdateDebugOutput("Start listenerforReceiveThread");

            IPAddress ipAddress = ServerForm.s_instance.serverIp; // get the ip of the server
            int portForReceive = 12345;
            IPEndPoint localEndPointReceive = new IPEndPoint(ipAddress, portForReceive);
            Socket listenerForReceive = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenerForReceive.Bind(localEndPointReceive);
            listenerForReceive.Listen(4); // up to 4 clients that wait to be accepted are supported



            while (!Globals.gameCtrl.GameStarted())
            {
                // wait for client connections
                Socket receiveFromClientSocket = listenerForReceive.Accept();
                IPEndPoint remoteEndPoint = (IPEndPoint)receiveFromClientSocket.RemoteEndPoint;
                ServerForm.s_instance.UpdateDebugOutput("RecvSocket: Client connected from IP " + remoteEndPoint.Address + " Port " + remoteEndPoint.Port);
                receiveFromClientSockets.Add(receiveFromClientSocket);

                Thread clientThread = new Thread(new ParameterizedThreadStart(ReceiveFromClientThread)); // create a thread to receive whenever a client connects
                clientThread.Start(receiveFromClientSocket);
            }
        }

        public void listenerforSendThread() 
        {
            ServerForm.s_instance.UpdateDebugOutput("Start listenerforSendThread");
            IPAddress ipAddress = ServerForm.s_instance.serverIp;
            int portForSend = 12346;
            IPEndPoint localEndPointSend = new IPEndPoint(ipAddress, portForSend);
            Socket listenerForSend = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenerForSend.Bind(localEndPointSend);
            listenerForSend.Listen(4); // up to 4 clients that wait to be accepted are supported

            while (!Globals.gameCtrl.GameStarted())
            {
                // wait for client connections
                Socket sendToClientSocket = listenerForSend.Accept();
                IPEndPoint remoteEndPoint = (IPEndPoint)sendToClientSocket.RemoteEndPoint;
                ServerForm.s_instance.UpdateDebugOutput("SendSocket: Client connected from IP " + remoteEndPoint.Address + " Port " + remoteEndPoint.Port);

                sendToClientSockets.Add(sendToClientSocket);

                Thread sendData = new Thread(new ParameterizedThreadStart(sendStartingData));
                sendData.Start(sendToClientSocket);
            }
        }

        public void sendStartingData(object s)
        {
            Socket sendSocket = (Socket)s;

            try
            {
                // Check if the socket is connected
                if (sendSocket != null && sendSocket.Connected)
                {
                    ServerForm.s_instance.UpdateDebugOutput("Socket is connected.");
                    foreach (Player p in Globals.players)
                    {
                        byte[] nameBytes = Encoding.UTF8.GetBytes(p.GetName());
                        byte[] messageBytes = new byte[nameBytes.Length + 2];
                        messageBytes[0] = (byte)MessageId.PlayerJoined;
                        messageBytes[1] = (byte)(p.GetId());
                        Array.Copy(nameBytes, 0, messageBytes, 2, nameBytes.Length);

                        // Call the Send method with error handling
                        MySocket.Send(sendSocket, messageBytes);
                    }
                }
                else
                {
                    // Socket is not connected, handle this case appropriately
                    ServerForm.s_instance.UpdateDebugOutput("Socket is not connected.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during sending
                ServerForm.s_instance.UpdateDebugOutput($"Exception occurred while sending data: {ex.Message}");
            }
        }
        public void ReceiveFromClientThread(object socket)
        {
            Player player = null;

            Socket receiveFromClientSocket = (Socket)socket;
            bool toContinue = true;
            while (toContinue)
            { //needs to be in a try and except method! if not whenever the client close the program the server will crash!
                try
                {
                    byte[] data = MySocket.Receive(receiveFromClientSocket); // data is full!
                    if (data != null)
                    {
                        if (data.Length > 0)
                        {
                            MessageId msgID = (MessageId)(data[0]);
                            if (msgID == MessageId.RegisterPlayer)
                            {
                                if (data.Length >= 7)
                                {
                                    if (Globals.players.Count > 2)
                                    {
                                        ServerForm.s_instance.UpdateDebugOutput("");
                                    }
                                    int id = (int)data[1];
                                    int port = BitConverter.ToInt32(data, 2); // starting at index 2
                                    string name = Encoding.UTF8.GetString(data, 6, data.Length - 6);
                                    ServerForm.s_instance.UpdateDebugOutput($"{name} connected with id {id}");
                                    player = new Player(name, id);
                                    player.sendSocketPort = port;
                                    player.receiveSocket = receiveFromClientSocket;
                                    RegisterPlayer(player);
                                }                        
                            }
                            else if (player != null)
                            {
                                switch (msgID)
                                {
                                    case MessageId.ReadyToStartGame:
                                        {
                                            if (player != null && Globals.players.Count >= 2 && player.sendSocket != null) // registered and enough players
                                            {
                                                ReadySuccess(player);
                                                PlayerIsReady(player);
                                                StartIfAllReady();
                                            }
                                            else
                                            {
                                                ServerForm.s_instance.UpdateDebugOutput($"ready error");
                                                if (player != null && player.sendSocket != null)
                                                    ReadyFailure(player);
                                            }
                                            break;
                                        }

                                    case MessageId.RollDiceClicked:
                                        {
                                            if (ableToSendClickRollDice && player.GetId() == Globals.gameCtrl.currentPlayerId)
                                            {
                                                ableToSendClickRollDice = false;
                                                RollDiceSuccess(player);
                                            }
                                            else
                                            {
                                                RollDiceFailure(player);
                                            }

                                            break;
                                        }

                                    case MessageId.RollDiceEnded:
                                        {
                                            if (ableToSendRollDiceEnded && !player.rollDiceEnded)
                                                RollDiceEnded(player);
                                            break;
                                        }

                                    case MessageId.PlayerMovingEnded:
                                        {
                                            if (ableToSendMovingEnded && !player.movingEnded)
                                                PlayerMovingEnded(player);
                                            break;
                                        }

                                    case MessageId.OkClicked:
                                        {
                                            if (ableToSendClickedOk && !player.okClicked)
                                                OkClicked(player);
                                            break;
                                        }

                                    case MessageId.Buy:
                                        {
                                            if (ableToSendBuyOrDontBuyClicked && Globals.gameCtrl.currentPlayerId == player.GetId())
                                            {
                                                ableToSendBuyOrDontBuyClicked = false;
                                                Buy();
                                            }
                                            break;
                                        }

                                    case MessageId.DontBuy:
                                        {
                                            if (ableToSendBuyOrDontBuyClicked && Globals.gameCtrl.currentPlayerId == player.GetId())
                                            {
                                                ableToSendBuyOrDontBuyClicked = false;
                                                DontBuy();
                                            }
                                            break;
                                        }

                                    case MessageId.UseCard:
                                        {
                                            if (ableToSendUseOrDontUseClicked && Globals.gameCtrl.currentPlayerId == player.GetId())
                                            {
                                                ableToSendUseOrDontUseClicked = false;
                                                UseCard();
                                            }
                                            break;
                                        }

                                    case MessageId.DontUseCard:
                                        {
                                            if (ableToSendUseOrDontUseClicked && Globals.gameCtrl.currentPlayerId == player.GetId())
                                            {
                                                ableToSendUseOrDontUseClicked = false;
                                                DontUseCard();
                                            }
                                            break;
                                        }

                                    case MessageId.SendMessage:
                                        {
                                            string message = Encoding.UTF8.GetString(data, 1, data.Length - 1);
                                            if (message.Length <= 100)
                                            {
                                                UpdateMessageInChat(player.GetId(), message);
                                            }
                                            break;
                                        }
                                    
                                    case MessageId.AskedMoneyOfferedSquares:
                                        {
                                            AskedMoneyOfferedSquares(player, data);
                                            break;
                                        }

                                    case MessageId.AskedMoneySquaresOfferedSquares:
                                        {
                                            AskedMoneySquaresOfferedSquares(player, data);
                                            break;
                                        }

                                    case MessageId.AskedSquaresOfferedMoney:
                                        {
                                            AskedSquaresOfferedMoney(player, data);
                                            break;
                                        }
                                    case MessageId.AskedSquaresOfferedSquares:
                                        {
                                            AskedSquaresOfferedSquares(player, data);
                                            break;
                                        }
                                    case MessageId.AskedSquaresOfferedSquaresMoney:
                                        {
                                            AskedSquaresOfferedSquaresMoney(player, data);
                                            break;
                                        }

                                    case MessageId.Sell:
                                        {
                                            if (!player.ableToReceiveOffer)
                                            {
                                                Sell(player);
                                            }
                                            break;
                                        }

                                    case MessageId.DontSell:
                                        {
                                            if (!player.ableToReceiveOffer)
                                            {
                                                DontSell(player);
                                            }
                                            break;
                                        }

                                }
                            }
                        }
                    }
                    else
                    {
                        if (player != null)
                        {
                            player.receiveSocket.Close();
                            if (player.sendSocket != null)
                                player.sendSocket.Close();
                            RemovePlayerFromPlayersList(player);
                            if (Globals.gameCtrl.GameStarted())
                            {
                                Globals.gameCtrl.playersToRemove.Enqueue(player);
                                player.movingEnded = true;
                                player.okClicked = true;
                                player.rollDiceEnded = true;
                                if (Globals.gameCtrl.currentPlayerId == player.GetId())
                                {
                                    Globals.gameCtrl.rollDiceClicked.SetResult(true);

                                    Globals.gameCtrl.toBuy = false;
                                    Globals.gameCtrl.playerClickedBuyOrDontBuyButton.SetResult(true);

                                    Globals.gameCtrl.toUseCard = false;
                                    Globals.gameCtrl.playerClickedUseOrDontUseButton.SetResult(true);

                                    OkClicked(player); // check if everyone clicked ok
                                    RollDiceEnded(player);// check if everyone finished rolling the dices
                                    PlayerMovingEnded(player);// check if everyone finished moveing
                                }
                                else
                                {
                                    OkClicked(player); // check if everyone clicked ok
                                    RollDiceEnded(player);// check if everyone finished rolling the dices
                                    PlayerMovingEnded(player);// check if everyone finished moveing
                                }
                            }
                            else 
                            {
                                byte[] buffer = { (byte)MessageId.EnableRegisterInId, (byte)player.GetId() };
                                foreach (Socket s in sendToClientSockets)
                                {
                                    MySocket.Send(s, buffer);
                                }
                                if (Globals.players.Count == 1)
                                {
                                    byte[] newBuffer = { (byte)MessageId.DisableReady };
                                    MySocket.Send(Globals.players.ElementAt(0).sendSocket, newBuffer);
                                    Globals.players.ElementAt(0).ready = false;
                                }
                                toContinue = false;
                                StartIfAllReady();
                            }
                        }                                                                                                                         
                        break;
                    }
                }
                catch (Exception ex) 
                {
                    break;
                }
            }
        }
        public void RegisterPlayer(Player player)
        {
            lock (lockForRegister)
            {

                string name = player.GetName();
                if (SetSendSocketForPlayer(player) == true && !string.IsNullOrEmpty(name) && CheckIfIdIsTaken(player.GetId()))
                {
                    if (!Globals.gameCtrl.GameStarted())
                    {

                        ServerForm.s_instance.UpdateDebugOutput($"{name} was registered");
                        Globals.players.Add(player);
                        byte[] nameBytes = Encoding.UTF8.GetBytes(name);
                        byte[] messageBytes = new byte[nameBytes.Length + 2];
                        messageBytes[0] = (byte)MessageId.RegisterSuccess;
                        messageBytes[1] = (byte)(player.GetId());
                        Array.Copy(nameBytes, 0, messageBytes, 2, nameBytes.Length);
                        ServerForm.s_instance.UpdateDebugOutput("Sending RegisterSuccess");
                        MySocket.Send(player.sendSocket, messageBytes);
                        PlayerJoined(player);
                        if (Globals.players.Count >= 2)
                        {
                            SendReadyIsPossible();
                            UpdatePlayerWithWhoIsReady(player);
                        }
                    }
                    else
                    {
                        byte[] buffer = { (byte)MessageId.GameHadAlreadyStarted, (byte)player.GetId() };
                        MySocket.Send(player.sendSocket, buffer);
                    }

                }
                else
                {
                    ServerForm.s_instance.UpdateDebugOutput("RegisterFailure");
                    byte[] messageBytes = { (byte)MessageId.RegisterFailure, (byte)0 };
                    if (player.sendSocket != null && player.sendSocket.Connected)
                    {
                        MySocket.Send(player.sendSocket, messageBytes);
                    }
                }
            }
        }
        public bool SetSendSocketForPlayer(Player player)
        {
            if (player == null)
                return false;


            foreach (Socket s in sendToClientSockets)
            {
                if (s != null && s.Connected && !(s.Poll(1, SelectMode.SelectRead) && s.Available == 0))
                {

                    IPEndPoint remoteEndPoint = (IPEndPoint)s.RemoteEndPoint;
                    if (remoteEndPoint.Port == player.sendSocketPort &&
                        remoteEndPoint.Address.Equals(((IPEndPoint)player.receiveSocket.RemoteEndPoint).Address))
                    {
                        player.sendSocket = s;
                        return true;
                    }
                }
            }
            return false;
        }
        public bool CheckIfIdIsTaken(int id)
        {
            foreach (Player p in Globals.players)
            {
                if (p.GetId() == id) return false;
            }
            return true;
        }
        public void PlayerJoined(Player player)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(player.GetName());
            byte[] messageBytes = new byte[nameBytes.Length + 2];
            messageBytes[0] = (byte)MessageId.PlayerJoined;
            messageBytes[1] = (byte)player.GetId();
            Array.Copy(nameBytes, 0, messageBytes, 2, nameBytes.Length);

            foreach (Socket s in sendToClientSockets)
            {
                if (!s.Equals(player.sendSocket))
                {
                    // send the name of the newly joined player to the existing player
                    MySocket.Send(s, messageBytes);
                }
            }
        }
        public void SendReadyIsPossible()
        {
            foreach (Player player in Globals.players)
            {
                byte[] buffer = { (byte)MessageId.ReadyIsPossible, (byte)player.GetId() };
                ServerForm.s_instance.UpdateDebugOutput("Sending ReadyIsPossible to " + player.GetName());
                MySocket.Send(player.sendSocket, buffer);
            }
        }
        public void UpdatePlayerWithWhoIsReady(object pl)
        {
            lock (lockForReady)
            {
                Player player = (Player)pl;
                foreach (Player p in Globals.players)
                {
                    if (!player.Equals(p))
                    {
                        if (p.ready)
                        {
                            byte[] buffer = { (byte)MessageId.PlayerIsReady, (byte)p.GetId() };
                            MySocket.Send(player.sendSocket, buffer);
                        }
                    }
                }
            }
        }
        public void ReadySuccess(Player player)
        {

            //name = MakeUniqueName(name);
            player.ready = true;
            ServerForm.s_instance.UpdateDebugOutput($"{player.GetName()} is ready");
            byte[] messageBytes = { (byte)MessageId.ReadySuccess, (byte)player.GetId() };
            ServerForm.s_instance.UpdateDebugOutput($"Sending {player.GetName()} is ready");
            MySocket.Send(player.sendSocket, messageBytes);

        }
        public void ReadyFailure(Player player)
        {
            ServerForm.s_instance.UpdateDebugOutput($"{player.GetName()} failed to ready");
            byte[] messageBytes = new byte[2];
            messageBytes[0] = (byte)MessageId.ReadyFailure;
            messageBytes[1] = (byte)player.GetId();
            ServerForm.s_instance.UpdateDebugOutput($"Sending {player.GetName()} failed to ready");
            MySocket.Send(player.sendSocket, messageBytes);
        }
        public void PlayerIsReady(Player player)
        {
            lock (lockForReady)
            {
                byte[] buffer = { (byte)MessageId.PlayerIsReady, (byte)player.GetId() };
                foreach (Player p in Globals.players)
                {
                    ServerForm.s_instance.UpdateDebugOutput($"sending {player.GetName()} is ready to {p.GetName()}");
                    MySocket.Send(p.sendSocket, buffer);
                }
            }
        }

        public void StartIfAllReady()
        {
            lock (lockForStartGame)
            {
                if (Globals.players.Count >= 2)
                {
                    bool allReady = true;
                    foreach (Player player in Globals.players)
                    {
                        if (!player.ready)
                        {
                            allReady = false;
                            break;
                        }
                    }
                    if (allReady)
                    {
                        if (!Globals.gameCtrl.GameStarted())
                        {
                            Thread game = new Thread(Globals.gameCtrl.StartGame);
                            game.Start();
                        }
                    }
                }
            }
        }
        // after game started
        public void SendGameStarted()
        {

            byte[] buffer = { (byte)MessageId.GameStarted };
            foreach (Player p in Globals.players)
            {
                try
                {
                    ServerForm.s_instance.UpdateDebugOutput($"sending game started to {p.GetName()}");
                    MySocket.Send(p.sendSocket, buffer);
                }
                catch (Exception ex)
                {
                    ServerForm.s_instance.UpdateDebugOutput("caught an exception");
                }
            }
        }
        public void DisconnectUnregisteredPlayers()
        {
            foreach (Socket sendSocket in sendToClientSockets)
            {
                if (sendSocket != null)
                {
                    bool found = false;
                    foreach (Player p in Globals.players)
                    {
                        if (p.sendSocket.Equals(sendSocket))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        byte[] buffer = { (byte)MessageId.GameHadAlreadyStarted };
                        MySocket.Send(sendSocket, buffer);
                        sendSocket.Close();
                        ServerForm.s_instance.UpdateDebugOutput("disconnected socket");
                    }
                }
            }
            foreach (Socket receiveSocket in receiveFromClientSockets)
            {
                if (receiveSocket != null)
                {
                    bool found = false;
                    foreach (Player p in Globals.players)
                    {
                        if (p.receiveSocket.Equals(receiveSocket))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        receiveSocket.Close();
                }
            }
        }
        public void SendWhoseTurn(int id)
        {
            byte[] buffer = { (byte)MessageId.WhoseTurn, (byte)id };
            foreach (Player p in Globals.players)
            {
                ServerForm.s_instance.UpdateDebugOutput($"sending WhoseTurn: (id:{id}) to {p.GetName()} with id {p.GetId()}");
                MySocket.Send(p.sendSocket, buffer);
            }
        }
        public void RollDiceAvailable(Player player)
        {
            ableToSendClickRollDice = true;
            ServerForm.s_instance.UpdateDebugOutput($"sending roll dice available to {player.GetName()}");
            byte[] buffer = { (byte)MessageId.RollDiceAvailable, 0 };
            MySocket.Send(player.sendSocket, buffer);
        }
        public void RollDiceSuccess(Player player)
        {
            ServerForm.s_instance.UpdateDebugOutput("RollDiceSuccess");
            byte[] buffer = { (byte)MessageId.RollDiceSuccess, (byte)player.GetId() };
            MySocket.Send(player.sendSocket, buffer);
            Globals.gameCtrl.rollDiceClicked.SetResult(true);
        }
        public void RollDiceFailure(Player player)
        {
            ServerForm.s_instance.UpdateDebugOutput("RollDiceFailure");
            byte[] buffer = { (byte)MessageId.RollDiceFailure, (byte)player.GetId() };
            MySocket.Send(player.sendSocket, buffer);

        }
        public void SendDicesValue(int dice1, int dice2)
        {
            ableToSendRollDiceEnded = true;
            ServerForm.s_instance.UpdateDebugOutput($"Sending dices values {Globals.gameCtrl.dice1},{Globals.gameCtrl.dice2}");
            byte[] buffer = { (byte)MessageId.SendDicesValue, (byte)dice1, (byte)dice2 };
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }
        public void RollDiceEnded(Player player)
        {
            lock (lockForRollDiceEnded)
            {
                player.rollDiceEnded = true;
                ServerForm.s_instance.UpdateDebugOutput($"roll dice ended for {player.GetName()}");

                bool rollDiceEndedForEveryone = true;
                foreach (Player p in Globals.players)
                {
                    if (!p.rollDiceEnded)
                    {
                        ServerForm.s_instance.UpdateDebugOutput($"roll dice did not end for {p.GetName()}");
                        rollDiceEndedForEveryone = false;
                        break;
                    }
                }
                if (rollDiceEndedForEveryone)
                {
                    ableToSendRollDiceEnded = false;
                    ServerForm.s_instance.UpdateDebugOutput("roll dice ended for everyone");
                    Globals.gameCtrl.rollDiceEnded.SetResult(true);
                }
            }
        }
        public void MovePlayer(Player player, int steps)
        {
            ableToSendMovingEnded = true;
            byte[] buffer = { (byte)MessageId.MovePlayer, (byte)player.GetId(), (byte)steps };
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }
        public void PlayerNeedsToGoToSquare(int id, int squareId)
        {
            ableToSendMovingEnded = true;
            byte[] buffer = { (byte)MessageId.PlayerNeedsToGoToSquare, (byte)id, (byte)squareId };
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }
        public void PlayerMovingEnded(Player player)
        {
            lock (lockForMovingEnded)
            {
                player.movingEnded = true;
                ServerForm.s_instance.UpdateDebugOutput($"moving ended for {player.GetName()}");

                bool playerMovingEndedForEveryone = true;
                foreach (Player p in Globals.players)
                {
                    if (!p.movingEnded)
                    {
                        ServerForm.s_instance.UpdateDebugOutput($"moving did not end for {p.GetName()}");
                        playerMovingEndedForEveryone = false;
                        break;
                    }
                }
                if (playerMovingEndedForEveryone)
                {
                    ableToSendMovingEnded = false;
                    ServerForm.s_instance.UpdateDebugOutput("MovingEnded for everyone");
                    Globals.gameCtrl.movingEnded.SetResult(true);
                }
            }
        }
        public void BuySquareOffer(Player player, string text)
        {
            ableToSendBuyOrDontBuyClicked = true;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] buffer = new byte[textBytes.Length + 2];
            buffer[0] = (byte)MessageId.BuySquareOffer;
            buffer[1] = (byte)player.currentSquare.id;
            Array.Copy(textBytes, 0, buffer, 2, textBytes.Length);
            MySocket.Send(player.sendSocket, buffer);

        }
        public void BuyHouseOffer(Player player, string text)
        {
            ableToSendBuyOrDontBuyClicked = true;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] buffer = new byte[textBytes.Length + 2];
            buffer[0] = (byte)MessageId.BuyHouseOffer;
            buffer[1] = (byte)player.currentSquare.id;
            Array.Copy(textBytes, 0, buffer, 2, textBytes.Length);
            MySocket.Send(player.sendSocket, buffer);
        }
        public void Buy()
        {
            Globals.gameCtrl.toBuy = true;
            Globals.gameCtrl.playerClickedBuyOrDontBuyButton.SetResult(true);
        }
        public void DontBuy()
        {
            Globals.gameCtrl.toBuy = false;
            Globals.gameCtrl.playerClickedBuyOrDontBuyButton.SetResult(true);
        }
        public void UpdateSquareWithOwner(int squareId, int ownerId)
        {
            byte[] buffer = { (byte)MessageId.UpdateSquareWithOwner, (byte)ownerId, (byte)squareId };
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }
        public void HouseAdded(int squareId, int amountOfHouses)
        {
            byte[] buffer = { (byte)MessageId.HouseAdded, (byte)squareId, (byte)amountOfHouses };
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }
        public void UpdatePlayerMoneyLabel(int id, int money)
        {
            byte[] moneyBytes = BitConverter.GetBytes(money);
            byte[] buffer = new byte[2 + moneyBytes.Length];
            buffer[0] = (byte)MessageId.UpdateMoneyLabel;
            buffer[1] = (byte)id;
            Array.Copy(moneyBytes, 0, buffer, 2, moneyBytes.Length);
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }

        }
        public void UseOutOfJailCardOffer(Player player, string text)
        {
            ableToSendUseOrDontUseClicked = true;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] buffer = new byte[textBytes.Length + 1];
            buffer[0] = (byte)MessageId.UseOutOfJailCardOffer;
            Array.Copy(textBytes, 0, buffer, 1, textBytes.Length);
            MySocket.Send(player.sendSocket, buffer);
        }
        public void UseCard()
        {
            Globals.gameCtrl.toUseCard = true;
            Globals.gameCtrl.playerClickedUseOrDontUseButton.SetResult(true);
        }
        public void DontUseCard()
        {
            Globals.gameCtrl.toUseCard = false;
            Globals.gameCtrl.playerClickedUseOrDontUseButton.SetResult(true);
        }
        public void UpdateMessageLabelForEveryone(string text)
        {
            ableToSendClickedOk = true;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] buffer = new byte[textBytes.Length + 1];
            buffer[0] = (byte)MessageId.UpdateMessageLabelForEveryone;
            Array.Copy(textBytes, 0, buffer, 1, textBytes.Length);
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }
        public void OkClicked(Player player)
        {
            lock (lockForOkClicked)
            {
                player.okClicked = true;
                ServerForm.s_instance.UpdateDebugOutput($"{player.GetName()} clicked ok");

                bool everyoneClickedOk = true;
                foreach (Player p in Globals.players)
                {
                    if (!p.okClicked)
                    {
                        ServerForm.s_instance.UpdateDebugOutput($"{p.GetName()} didnt click ok yet");
                        everyoneClickedOk = false;
                        break;
                    }
                }
                if (everyoneClickedOk)
                {
                    ableToSendClickedOk = false;
                    ServerForm.s_instance.UpdateDebugOutput("everyone cliecked ok");
                    Globals.gameCtrl.everyoneClickedOk.SetResult(true);
                }
            }

        }
        public void SendAmountOfOutOfJailCards(int id, int amount)
        {
            byte[] buffer = { (byte)MessageId.AmountOfOutOfJailCards, (byte)id, (byte)amount };

            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }


        public void UpdateMessageInChat(int id, string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] buffer = new byte[2 + messageBytes.Length];
            buffer[0] = (byte)MessageId.UpdateMessageInChat;
            buffer[1] = (byte)id;
            Array.Copy(messageBytes, 0, buffer, 2, messageBytes.Length);
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }
        public void AskedMoneyOfferedSquares(Player offerer, byte[] data)// need try and except
        {
            lock (lockForTradingOffer)
            {
                if (data.Length >= 6)
                {
                    int playerChosenId = (int)data[1];
                    Player playerChosen = GetPlayer(playerChosenId);
                    if (playerChosen != null && playerChosenId != offerer.GetId())
                    {
                        if (playerChosen.ableToReceiveOffer)
                        {
                            int priceAsked = BitConverter.ToInt32(data, 2);
                            if (playerChosen.money > priceAsked)
                            {
                                bool tradeIsPossible = true;
                                List<Square> squaresOffered = new List<Square>();
                                for (int i = 6; i < data.Length; i++)
                                {
                                    int squareId = (int)data[i];
                                    if (!offerer.IsOwner(squareId) || !Globals.gameCtrl.NoHousesInStreet(squareId))
                                    {
                                        tradeIsPossible = false;
                                        break;
                                    }
                                    squaresOffered.Add(Board.squares[squareId]);
                                }


                                if (tradeIsPossible && squaresOffered.Count >= 1 && squaresOffered.Count <= 3)
                                {
                                    playerChosen.trade.tradeType = MessageId.AskedMoneyOfferedSquares;
                                    playerChosen.trade.offerer = offerer;
                                    playerChosen.trade.receiver = playerChosen;
                                    playerChosen.trade.squaresOffered = squaresOffered;
                                    playerChosen.trade.priceRequested = priceAsked;

                                    StringBuilder sb = new StringBuilder();
                                    sb.Append($"{offerer.GetName()} asks you for {priceAsked}₪. In return {offerer.GetName()} will give you: ");
                                    sb.Append(Globals.gameCtrl.GetSquaresString(squaresOffered));
                                    string text = sb.ToString();
                                    SendTradeOffer(playerChosen, text);
                                    playerChosen.ableToReceiveOffer = false; // ToDo - needs to update clients as well
                                    SendPlayerIsNotAbleToReceiveOffer(playerChosenId);
                                }
                                else
                                {
                                    SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");
                                }
                            }
                            else
                            {
                                SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");
                            }
                        }
                        else
                        {
                            SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");
                        }
                    }
                    else
                    {
                        SendPrivateMessageToPlayer(offerer, "Error in the trade");
                    }
                }
                else
                {
                    SendPrivateMessageToPlayer(offerer, $"Error in trade");
                }
            }
        }
        public void AskedMoneySquaresOfferedSquares(Player offerer, byte[] data)
        {
            lock (lockForTradingOffer)
            {
                if (data.Length >= 6)
                {
                    int playerChosenId = (int)data[1];
                    Player playerChosen = GetPlayer(playerChosenId);
                    if (playerChosen != null && playerChosenId != offerer.GetId())
                    {
                        if (playerChosen.ableToReceiveOffer)
                        {
                            int priceAsked = BitConverter.ToInt32(data, 2);
                            if (playerChosen.money > priceAsked)
                            {
                                bool tradeIsPossible = true;
                                List<Square> squaresOffered = new List<Square>();
                                List<Square> squaresRequested = new List<Square>();
                                for (int i = 6; i < data.Length; i++)
                                {
                                    int squareId = (int)data[i];
                                    if (Globals.gameCtrl.NoHousesInStreet(squareId))
                                    {
                                        if (offerer.IsOwner(squareId))
                                        {
                                            squaresOffered.Add(Board.squares[squareId]);
                                        }
                                        else if (playerChosen.IsOwner(squareId))
                                        {
                                            squaresRequested.Add(Board.squares[squareId]);
                                        }
                                        else
                                        {
                                            tradeIsPossible = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        tradeIsPossible = false;
                                        break;
                                    }
                                }


                                if (tradeIsPossible && squaresOffered.Count >= 1 && squaresOffered.Count <= 3 && squaresRequested.Count >= 1 && squaresRequested.Count <= 3)
                                {
                                    playerChosen.trade.tradeType = MessageId.AskedMoneySquaresOfferedSquares;
                                    playerChosen.trade.offerer = offerer;
                                    playerChosen.trade.receiver = playerChosen;
                                    playerChosen.trade.squaresOffered = squaresOffered;
                                    playerChosen.trade.squaresAsked = squaresRequested;
                                    playerChosen.trade.priceRequested = priceAsked;

                                    StringBuilder sb = new StringBuilder();
                                    sb.Append($"{offerer.GetName()} asks you for {priceAsked}₪, for: ");
                                    sb.Append(Globals.gameCtrl.GetSquaresString(squaresRequested) + ". ");


                                    sb.Append("In return he will give you: ");
                                    sb.Append(Globals.gameCtrl.GetSquaresString(squaresOffered));


                                    string text = sb.ToString();
                                    SendTradeOffer(playerChosen, text);
                                    playerChosen.ableToReceiveOffer = false;
                                    SendPlayerIsNotAbleToReceiveOffer(playerChosenId);
                                }
                                else
                                {
                                    SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");

                                }
                            }
                            else
                            {
                                SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");

                            }
                        }
                        else
                        {
                            SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");

                        }
                    }
                    else
                    {
                        SendPrivateMessageToPlayer(offerer, "Error in the trade");

                    }
                }
                else
                {
                    SendPrivateMessageToPlayer(offerer, $"Error in trade");

                }
            }
        }
        public void AskedSquaresOfferedMoney(Player offerer, byte[] data)
        {
            lock (lockForTradingOffer)
            {
                if (data.Length >= 6)
                {
                    int playerChosenId = (int)data[1];
                    Player playerChosen = GetPlayer(playerChosenId);
                    if (playerChosen != null && playerChosenId != offerer.GetId())
                    {
                        if (playerChosen.ableToReceiveOffer)
                        {
                            int priceOffered = BitConverter.ToInt32(data, 2);
                            if (offerer.money > priceOffered)
                            {
                                bool tradeIsPossible = true;
                                List<Square> squaresAsked = new List<Square>();
                                for (int i = 6; i < data.Length; i++)
                                {
                                    int squareId = (int)data[i];
                                    if (!playerChosen.IsOwner(squareId) || !Globals.gameCtrl.NoHousesInStreet(squareId))
                                    {
                                        tradeIsPossible = false;
                                        break;
                                    }
                                    squaresAsked.Add(Board.squares[squareId]);
                                }


                                if (tradeIsPossible && squaresAsked.Count >= 1 && squaresAsked.Count <= 3)
                                {
                                    playerChosen.trade.tradeType = MessageId.AskedSquaresOfferedMoney;
                                    playerChosen.trade.offerer = offerer;
                                    playerChosen.trade.receiver = playerChosen;
                                    playerChosen.trade.squaresAsked = squaresAsked;
                                    playerChosen.trade.priceOffered = priceOffered;

                                    StringBuilder sb = new StringBuilder();
                                    sb.Append($"{offerer.GetName()} asks you for: ");
                                    sb.Append(Globals.gameCtrl.GetSquaresString(squaresAsked) + ". ");
                                    sb.Append($"In return he will give you {priceOffered}₪");
                                    string text = sb.ToString();
                                    SendTradeOffer(playerChosen, text);
                                    playerChosen.ableToReceiveOffer = false; // ToDo - needs to update clients as well
                                    SendPlayerIsNotAbleToReceiveOffer(playerChosenId);
                                }
                                else
                                {
                                    SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");

                                }
                            }
                            else
                            {
                                SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");

                            }
                        }
                        else
                        {
                            SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");

                        }
                    }
                    else
                    {
                        SendPrivateMessageToPlayer(offerer, $"Error in trade");
                    }
                }
                else
                {
                    SendPrivateMessageToPlayer(offerer, $"Error in trade");
                }
            }
        }
        public void AskedSquaresOfferedSquaresMoney(Player offerer, byte[] data)
        {
            lock (lockForTradingOffer)
            {
                if (data.Length >= 6)
                {
                    
                    int playerChosenId = (int)data[1];
                    Player playerChosen = GetPlayer(playerChosenId);
                    if (playerChosen != null && playerChosenId != offerer.GetId())
                    {
                        if (playerChosen.ableToReceiveOffer)
                        {
                            int priceOffered = BitConverter.ToInt32(data, 2);
                            if (offerer.money > priceOffered)
                            {
                                bool tradeIsPossible = true;
                                List<Square> squaresOffered = new List<Square>();
                                List<Square> squaresRequested = new List<Square>();
                                for (int i = 6; i < data.Length; i++)
                                {
                                    int squareId = (int)data[i];
                                    if (Globals.gameCtrl.NoHousesInStreet(squareId))
                                    {
                                        if (offerer.IsOwner(squareId))
                                        {
                                            squaresOffered.Add(Board.squares[squareId]);
                                        }
                                        else if (playerChosen.IsOwner(squareId))
                                        {
                                            squaresRequested.Add(Board.squares[squareId]);
                                        }
                                        else
                                        {
                                            tradeIsPossible = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        tradeIsPossible = false;
                                        break;
                                    }
                                }


                                if (tradeIsPossible && squaresOffered.Count >= 1 && squaresOffered.Count <= 3 && squaresRequested.Count >= 1 && squaresRequested.Count <= 3)
                                {
                                    playerChosen.trade.tradeType = MessageId.AskedSquaresOfferedSquaresMoney;
                                    playerChosen.trade.offerer = offerer;
                                    playerChosen.trade.receiver = playerChosen;
                                    playerChosen.trade.squaresAsked = squaresRequested;
                                    playerChosen.trade.squaresOffered = squaresOffered;
                                    playerChosen.trade.priceOffered = priceOffered;

                                    StringBuilder sb = new StringBuilder();
                                    sb.Append($"{offerer.GetName()} asks you for: ");
                                    sb.Append(Globals.gameCtrl.GetSquaresString(squaresRequested) + ". ");


                                    sb.Append($"In return he will give you {priceOffered}₪, squares: ");
                                    sb.Append(Globals.gameCtrl.GetSquaresString(squaresOffered));


                                    string text = sb.ToString();
                                    SendTradeOffer(playerChosen, text);
                                    playerChosen.ableToReceiveOffer = false;
                                    SendPlayerIsNotAbleToReceiveOffer(playerChosenId);
                                }
                                else
                                {
                                    SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");

                                }
                            }
                            else
                            {
                                SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");

                            }
                        }
                        else
                        {
                            SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");

                        }
                    }
                    else
                    {
                        SendPrivateMessageToPlayer(offerer, $"Error in trade");

                    }
                }
                else
                {
                    SendPrivateMessageToPlayer(offerer, $"Error in trade");

                }
            }
        }
        public void AskedSquaresOfferedSquares(Player offerer, byte[] data)
        {
            lock (lockForTradingOffer)
            {
                if (data.Length >= 2)
                {
                    int playerChosenId = (int)data[1];
                    Player playerChosen = GetPlayer(playerChosenId);
                    if (playerChosen != null && playerChosenId != offerer.GetId())
                    {
                        if (playerChosen.ableToReceiveOffer)
                        {
                            bool tradeIsPossible = true;
                            List<Square> squaresOffered = new List<Square>();
                            List<Square> squaresRequested = new List<Square>();
                            for (int i = 2; i < data.Length; i++)
                            {
                                int squareId = (int)data[i];
                                if (Globals.gameCtrl.NoHousesInStreet(squareId))
                                {
                                    if (offerer.IsOwner(squareId))
                                    {
                                        squaresOffered.Add(Board.squares[squareId]);
                                    }
                                    else if (playerChosen.IsOwner(squareId))
                                    {
                                        squaresRequested.Add(Board.squares[squareId]);
                                    }
                                    else
                                    {
                                        tradeIsPossible = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    tradeIsPossible = false;
                                    break;
                                }
                            }


                            if (tradeIsPossible && squaresOffered.Count >= 1 && squaresOffered.Count <= 3 && squaresRequested.Count >= 1 && squaresRequested.Count <= 3)
                            {
                                playerChosen.trade.tradeType = MessageId.AskedSquaresOfferedSquares;
                                playerChosen.trade.offerer = offerer;
                                playerChosen.trade.receiver = playerChosen;
                                playerChosen.trade.squaresAsked = squaresRequested;
                                playerChosen.trade.squaresOffered = squaresOffered;

                                StringBuilder sb = new StringBuilder();
                                sb.Append($"{offerer.GetName()} asks you for: ");
                                sb.Append(Globals.gameCtrl.GetSquaresString(squaresRequested) + ". ");


                                sb.Append($"In return he will give you: ");
                                sb.Append(Globals.gameCtrl.GetSquaresString(squaresOffered));


                                string text = sb.ToString();
                                SendTradeOffer(playerChosen, text);
                                playerChosen.ableToReceiveOffer = false;
                                SendPlayerIsNotAbleToReceiveOffer(playerChosenId);
                            }
                            else
                            {
                                SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");

                            }
                        }
                        else
                        {
                            SendPrivateMessageToPlayer(offerer, $"The trade that you offered to {playerChosen.GetName()} is canceled");
                        }
                    }
                    else
                    {
                        SendPrivateMessageToPlayer(offerer, $"Error in trade");

                    }
                }
                else
                {
                    SendPrivateMessageToPlayer(offerer, $"Error in trade");

                }
            }
        }
        public Player GetPlayer(int id)
        {
            foreach (Player p in Globals.players)
            {
                if (p.GetId() == id)
                    return p;
            }
            return null;
        }
        public void SendPlayerIsNotAbleToReceiveOffer(int playerId)
        {
            byte[] buffer = { (byte)MessageId.PlayerIsNotAbleToReceiveOffer, (byte)playerId };
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }
        public void SendPrivateMessageToPlayer(Player player, string text)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] buffer = new byte[textBytes.Length + 1];
            buffer[0] = (byte)MessageId.UpdatePrivateMessageLabel;
            Array.Copy(textBytes, 0, buffer, 1, textBytes.Length);
            MySocket.Send(player.sendSocket, buffer);
        }
        public void SendTradeOffer(Player playerChosen, string text)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] buffer = new byte[textBytes.Length + 1];
            buffer[0] = (byte)MessageId.TradingOffer;
            Array.Copy(textBytes, 0, buffer, 1, textBytes.Length);
            MySocket.Send(playerChosen.sendSocket, buffer);
        }
        public void Sell(Player receiver)
        {
            Globals.gameCtrl.tradeQueue.Enqueue(receiver.trade);
        }
        public void DontSell(Player receiver) 
        {
            string text = $"{receiver.GetName()} has declined your offer";
            SendPrivateMessageToPlayer(receiver.trade.offerer, text);
            receiver.ableToReceiveOffer = true;
            EnableSendingOffersToPlayer(receiver.GetId());
        }
        public void SendTradeWasMade(string text)
        {
            ableToSendClickedOk = true;
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] buffer = new byte[textBytes.Length + 1];
            buffer[0] = (byte)MessageId.TradeWasMade;
            Array.Copy(textBytes, 0, buffer, 1, textBytes.Length);
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }
        public void EnableSendingOffersToPlayer(int playerId)
        {
            byte[] buffer = { (byte)MessageId.EnableSendingOffersToPlayer, (byte)playerId };
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }
        public void RemovePlayerFromPlayersList(Player player)
        {
            // Temporary bag to hold players not equal to the player to remove
            ConcurrentBag<Player> tempBag = new ConcurrentBag<Player>();

            // Iterate through the original bag
            foreach (Player p in Globals.players)
            {
                // If the player is not the one we want to remove, add it to the temporary bag
                if (!p.Equals(player))
                {
                    tempBag.Add(p);
                }
            }
            Globals.players.Clear();

            // Copy elements from the temporary bag back to the original bag
            foreach (Player p in tempBag)
            {
                Globals.players.Add(p);
            }
        }        
        public void RemoveSquareFromOwner(int squareId, int ownerId)
        {
            byte[] buffer = { (byte)MessageId.RemoveSquareFromOwner, (byte)squareId, (byte)ownerId };
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }       
        public void RemovePlayerThatDisconnectedFromClients(int playerId)
        {
            byte[] buffer = { (byte)MessageId.RemovePlayerDisconnectedFromClients, (byte)playerId };
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        } 
        public void MakePlayerSpectateInClients(int playerId)
        {
            byte[] buffer = { (byte)MessageId.MakePlayerSpectateInClients, (byte)playerId };
            foreach (Player p in Globals.players)
            {
                MySocket.Send(p.sendSocket, buffer);
            }
        }      
    }
}
