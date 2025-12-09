using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace MonopolyServer
{
    public class GameCtrl
    {
        private Random rnd = new Random();
        public int currentPlayerIndex; // the index in the players list
        public int currentPlayerId; // the current player id (a member of the player class)
        private Player currentPlayer;
        private bool gameStarted;
        private int doubleStreak;
        public bool rolledADouble = false;
        public int dice1;
        public int dice2;

        public TaskCompletionSource<bool> rollDiceClicked = new TaskCompletionSource<bool>();
        public TaskCompletionSource<bool> rollDiceEnded = new TaskCompletionSource<bool>();
        public TaskCompletionSource<bool> movingEnded = new TaskCompletionSource<bool>();
        public TaskCompletionSource<bool> everyoneClickedOk = new TaskCompletionSource<bool>();
        public TaskCompletionSource<bool> playerClickedBuyOrDontBuyButton = new TaskCompletionSource<bool>();
        public TaskCompletionSource<bool> playerClickedUseOrDontUseButton = new TaskCompletionSource<bool>();

        public bool toBuy;
        public bool toUseCard;


        public Queue<Trade> tradeQueue;
        public Queue<Player> playersToRemove;

        public GameCtrl()
        {
            gameStarted = false;
            tradeQueue = new Queue<Trade>();
            playersToRemove = new Queue<Player>();

        }
        public bool GameStarted() => gameStarted;
        public async void StartGame() // async allows to use await in action
        {
            gameStarted = true;
            Globals.lobbyCtrl.SendGameStarted();
            Globals.lobbyCtrl.DisconnectUnregisteredPlayers();
            currentPlayerIndex = rnd.Next(0, Globals.players.Count);
            while (GameContinues())
            {
                if (playersToRemove.Count > 0)
                {
                    rollDiceClicked = new TaskCompletionSource<bool>();
                    playerClickedBuyOrDontBuyButton = new TaskCompletionSource<bool>();
                    playerClickedUseOrDontUseButton = new TaskCompletionSource<bool>();
                }

                while (playersToRemove.Count > 0)
                {
                    Player player = playersToRemove.Dequeue();
                    if (currentPlayerId == player.GetId())
                    {
                        currentPlayerIndex = (currentPlayerIndex + 1) % Globals.players.Count;
                    }
                    ServerForm.s_instance.UpdateDebugOutput($"removing player {player.GetName()}");
                    player.receiveSocket.Close();
                    player.sendSocket.Close();


                    player.RemovePlayerAssets();
                    Globals.lobbyCtrl.RemovePlayerThatDisconnectedFromClients(player.GetId());
                    string message = $"{player.GetName()} has disconnected.";
                    await SendMessage(message);
                }

                if (!GameContinues())
                    break;
                while (tradeQueue.Count > 0)
                {
                    Trade trade = tradeQueue.Dequeue();
                    switch (trade.tradeType)
                    {
                        case MessageId.AskedMoneyOfferedSquares:
                            {
                                await AskedMoneyOfferedSquares(trade);
                                break;
                            }
                        case MessageId.AskedMoneySquaresOfferedSquares:
                            {
                                await AskedMoneySquaresOfferedSquares(trade);
                                break;
                            }
                        case MessageId.AskedSquaresOfferedMoney:
                        {
                                await AskedSquaresOfferedMoney(trade);
                                break;
                        }
                        case MessageId.AskedSquaresOfferedSquaresMoney:
                        {
                                await AskedSquaresOfferedSquaresMoney(trade);
                                break;
                        }
                        case MessageId.AskedSquaresOfferedSquares:
                        {
                                await AskedSquaresOfferedSquares(trade);
                                break;
                        }
                    }
                }

                currentPlayer = Globals.players.ElementAt(currentPlayerIndex);
                if (currentPlayer.inGame)
                {
                    currentPlayerId = currentPlayer.GetId();
                    Globals.lobbyCtrl.SendWhoseTurn(currentPlayerId);
                    await HandleTurn();


                    ServerForm.s_instance.UpdateDebugOutput($"Turn is over for {currentPlayer.GetName()}");
                    if (!rolledADouble)
                        currentPlayerIndex = (currentPlayerIndex + 1) % Globals.players.Count;
                }   
                else
                {
                    currentPlayerIndex = (currentPlayerIndex + 1) % Globals.players.Count;
                }
            }

            while (playersToRemove.Count > 0)
            {
                Player player = playersToRemove.Dequeue();
                if (currentPlayerId == player.GetId())
                {
                    currentPlayerIndex = (currentPlayerIndex + 1) % Globals.players.Count;
                }
                ServerForm.s_instance.UpdateDebugOutput($"removing player {player.GetName()}");
                player.receiveSocket.Close();
                player.sendSocket.Close();


                player.RemovePlayerAssets();
                Globals.lobbyCtrl.RemovePlayerThatDisconnectedFromClients(player.GetId());
                string message = $"{player.GetName()} has disconnected.";
                await SendMessage(message);
            }

            Player winner = null;
            foreach (Player p in Globals.players)
            {
                if (p.inGame)
                {
                    winner = p;
                    break;
                }
            }
            if (winner != null)
            {
                string message = $"{winner.GetName()} is the winner!";
                await SendMessage(message);
            }

        }
        public bool GameContinues()
        {
            int inGameCount = 0;
            foreach (Player player in Globals.players)
            {
                if (player.inGame)
                    inGameCount++;
            }
            return inGameCount > 1;
        }
        public async Task SendMessage(string text) // waiting for players to accept is included         
        {
            Globals.lobbyCtrl.UpdateMessageLabelForEveryone(text);
            await everyoneClickedOk.Task;
            foreach (Player p in Globals.players)
            {
                p.okClicked = false;
            }
            everyoneClickedOk = new TaskCompletionSource<bool>();
        }
        public async Task AskedMoneyOfferedSquares(Trade trade)
        {
            bool tradeIsPossible = 
                trade.receiver.money > trade.priceRequested &&
                isSquaresOkForTrading(trade.offerer, trade.squaresOffered);
            if (tradeIsPossible)
            {
                trade.receiver.money -= trade.priceRequested;
                trade.offerer.money += trade.priceRequested;
                Globals.lobbyCtrl.UpdatePlayerMoneyLabel(trade.offerer.GetId(), trade.offerer.money);
                Globals.lobbyCtrl.UpdatePlayerMoneyLabel(trade.receiver.GetId(), trade.receiver.money);


                SwitchSquaresOwnershipFromPlayerToPlayer(trade.offerer, trade.receiver, trade.squaresOffered);


                StringBuilder sb = new StringBuilder();
                sb.Append($"{trade.receiver.GetName()} gave {trade.offerer.GetName()} {trade.priceRequested}₪ for: ");
                sb.Append(GetSquaresString(trade.squaresOffered));
                string text = sb.ToString();
                await SendTradeWasMade(text);
            }
            else // trade failed - update the offerer and the receiver
            {
                await TradeFailed(trade);
            }
            trade.receiver.ableToReceiveOffer = true;
            Globals.lobbyCtrl.EnableSendingOffersToPlayer(trade.receiver.GetId());
        }
        public async Task AskedMoneySquaresOfferedSquares(Trade trade)
        {
            bool tradeIsPossible =
                trade.receiver.money > trade.priceRequested &&
                isSquaresOkForTrading(trade.receiver, trade.squaresAsked) &&
                isSquaresOkForTrading(trade.offerer, trade.squaresOffered);

            if (tradeIsPossible)
            {
                trade.receiver.money -= trade.priceRequested;
                trade.offerer.money += trade.priceRequested;
                Globals.lobbyCtrl.UpdatePlayerMoneyLabel(trade.offerer.GetId(), trade.offerer.money);
                Globals.lobbyCtrl.UpdatePlayerMoneyLabel(trade.receiver.GetId(), trade.receiver.money);

                SwitchSquaresOwnershipFromPlayerToPlayer(trade.offerer, trade.receiver, trade.squaresOffered);
                SwitchSquaresOwnershipFromPlayerToPlayer(trade.receiver, trade.offerer, trade.squaresAsked);

                StringBuilder sb = new StringBuilder();
                sb.Append($"{trade.receiver.GetName()} gave {trade.offerer.GetName()} {trade.priceRequested}₪, squares: ");
                sb.Append(GetSquaresString(trade.squaresAsked) + ". ");

                sb.Append($"In return {trade.receiver.GetName()} got squares: ");
                sb.Append(GetSquaresString(trade.squaresOffered));
                string text = sb.ToString();
                await SendTradeWasMade(text);
            }
            else
            {
                await TradeFailed(trade);
            }
            trade.receiver.ableToReceiveOffer = true;
            Globals.lobbyCtrl.EnableSendingOffersToPlayer(trade.receiver.GetId());
        }
        public async Task AskedSquaresOfferedMoney(Trade trade) // same as the AskedMoneyOfferedSquares but fliped
        {
            bool tradeIsPossible =
                trade.offerer.money > trade.priceOffered &&
                isSquaresOkForTrading(trade.receiver, trade.squaresAsked);
            if (tradeIsPossible)
            {
                trade.offerer.money -= trade.priceOffered;
                trade.receiver.money += trade.priceOffered;
                Globals.lobbyCtrl.UpdatePlayerMoneyLabel(trade.offerer.GetId(), trade.offerer.money);
                Globals.lobbyCtrl.UpdatePlayerMoneyLabel(trade.receiver.GetId(), trade.receiver.money);


                SwitchSquaresOwnershipFromPlayerToPlayer(trade.receiver, trade.offerer, trade.squaresAsked);


                StringBuilder sb = new StringBuilder();
                sb.Append($"{trade.offerer.GetName()} gave {trade.receiver.GetName()} {trade.priceOffered}₪. In return {trade.offerer.GetName()} got: ");
                sb.Append(GetSquaresString(trade.squaresAsked));
                string text = sb.ToString();
                await SendTradeWasMade(text);
            }
            else // trade failed - update the offerer and the receiver
            {
                await TradeFailed(trade);
            }
            trade.receiver.ableToReceiveOffer = true;
            Globals.lobbyCtrl.EnableSendingOffersToPlayer(trade.receiver.GetId());
        }
        public async Task AskedSquaresOfferedSquaresMoney(Trade trade) // same as AskedMoneySquaresOfferedSquares but fliped
        {
            bool tradeIsPossible =
                trade.offerer.money > trade.priceOffered &&
                isSquaresOkForTrading(trade.receiver, trade.squaresAsked) &&
                isSquaresOkForTrading(trade.offerer, trade.squaresOffered);

            if (tradeIsPossible)
            {
                trade.offerer.money -= trade.priceOffered;
                trade.receiver.money += trade.priceOffered;
                Globals.lobbyCtrl.UpdatePlayerMoneyLabel(trade.offerer.GetId(), trade.offerer.money);
                Globals.lobbyCtrl.UpdatePlayerMoneyLabel(trade.receiver.GetId(), trade.receiver.money);

                SwitchSquaresOwnershipFromPlayerToPlayer(trade.offerer, trade.receiver, trade.squaresOffered);
                SwitchSquaresOwnershipFromPlayerToPlayer(trade.receiver, trade.offerer, trade.squaresAsked);

                StringBuilder sb = new StringBuilder();
                sb.Append($"{trade.offerer.GetName()} gave {trade.receiver.GetName()} {trade.priceOffered}₪, squares: ");
                sb.Append(GetSquaresString(trade.squaresOffered) + ". ");

                sb.Append($"In return {trade.offerer.GetName()} got squares: ");
                sb.Append(GetSquaresString(trade.squaresAsked));
                string text = sb.ToString();
                await SendTradeWasMade(text);
            }
            else
            {
                await TradeFailed(trade);
            }
            trade.receiver.ableToReceiveOffer = true;
            Globals.lobbyCtrl.EnableSendingOffersToPlayer(trade.receiver.GetId());
        }
        public async Task AskedSquaresOfferedSquares(Trade trade)
        {
            bool tradeIsPossible =
                isSquaresOkForTrading(trade.receiver, trade.squaresAsked) &&
                isSquaresOkForTrading(trade.offerer, trade.squaresOffered);

            if (tradeIsPossible)
            {
                SwitchSquaresOwnershipFromPlayerToPlayer(trade.offerer, trade.receiver, trade.squaresOffered);
                SwitchSquaresOwnershipFromPlayerToPlayer(trade.receiver, trade.offerer, trade.squaresAsked);

                StringBuilder sb = new StringBuilder();
                sb.Append($"{trade.receiver.GetName()} gave {trade.offerer.GetName()}: ");
                sb.Append(GetSquaresString(trade.squaresAsked) + ". ");

                sb.Append($"In return {trade.receiver.GetName()} got: ");
                sb.Append(GetSquaresString(trade.squaresOffered));
                string text = sb.ToString();
                await SendTradeWasMade(text);
            }
            else
            {
                await TradeFailed(trade);
            }
            trade.receiver.ableToReceiveOffer = true;
            Globals.lobbyCtrl.EnableSendingOffersToPlayer(trade.receiver.GetId());
        }
        public bool isSquaresOkForTrading(Player owner, List<Square> squares)
        {
            foreach (Square s in squares)
            {
                int squareId = s.id;
                if (!owner.IsOwner(squareId) || !Globals.gameCtrl.NoHousesInStreet(squareId))
                {
                    return false;
                }
            }
            return true;
        }
        public void SwitchSquaresOwnershipFromPlayerToPlayer(Player from, Player to, List<Square> squares)
        {
            foreach (Square square in squares)
            {
                SquaresForBuying squaretoAdd = (SquaresForBuying)Board.squares[square.id];
                squaretoAdd.owner = to;
                squaretoAdd.isOwned = true;

                to.AddSquare(squaretoAdd);
                if (squaretoAdd is RailroadSquare)
                {
                    to.amountOfTrainsOwned++;
                    from.amountOfTrainsOwned--;
                }
                else if (squaretoAdd is UtilitySquare)
                {
                    to.UtilitySquaresOwned++;
                    from.UtilitySquaresOwned--;
                }
                from.RemoveSquare(square.id);

                Globals.lobbyCtrl.RemoveSquareFromOwner(square.id, from.GetId());
                Globals.lobbyCtrl.UpdateSquareWithOwner(square.id, to.GetId());
            }
        }               
        public string GetSquaresString(List<Square> squares) // example - avenue (square 1), water company (square 23)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < squares.Count - 1; i++)
            {
                sb.Append($"{squares[i].name} (square {squares[i].id}), ");
            }
            sb.Append($"{squares[squares.Count - 1].name} (square {squares[squares.Count - 1].id})");
            return sb.ToString();
        }
        public async Task SendTradeWasMade(string text)
        {
            Globals.lobbyCtrl.SendTradeWasMade(text);
            await everyoneClickedOk.Task;
            foreach (Player p in Globals.players)
            {
                p.okClicked = false;
            }
            everyoneClickedOk = new TaskCompletionSource<bool>();
        }
        public async Task TradeFailed(Trade trade)
        {
            string text = $"The trade that {trade.offerer.GetName()} offered to {trade.receiver.GetName()} has been canceled because one of them does not meet the terms of the offer";
            await SendTradeWasMade(text);
        }
        public bool NoHousesInStreet(int squareId)
        {
            if (Board.squares[squareId] is StreetSquare)
            {
                StreetSquare streetSquare = (StreetSquare)Board.squares[squareId];
                foreach (Square s in Board.squares)
                {
                    if (s is StreetSquare)
                    {
                        StreetSquare currentStreetSquare = (StreetSquare)s;
                        if (currentStreetSquare.streetId == streetSquare.streetId)
                        {
                            if (currentStreetSquare.numOfHouses != 0)
                            {
                                ServerForm.s_instance.UpdateDebugOutput("there are houses");
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            return true;
        }       
        public async Task HandleTurn()
        {
            if (currentPlayer.inParking)
            {
                ServerForm.s_instance.UpdateDebugOutput("in parking square");
                await HandlePlayerInParking();
            }
            else if (currentPlayer.inJail)
            {
                ServerForm.s_instance.UpdateDebugOutput("in jail square");
                await HandlePlayerInJail();
            }
            else
            {
                ServerForm.s_instance.UpdateDebugOutput("in a normal square");
                int sqaureIdBeforeMoving = currentPlayer.currentSquare.id;
                await DoRollDice();
                if (dice1 == dice2)
                {
                    string text = $"{currentPlayer.GetName()} rolled a double!";
                    await SendMessage(text);
                    doubleStreak++;
                    rolledADouble = true;
                    if (doubleStreak == 3)
                    {
                        await HandlePassedMaxDoubleStreak();
                    }
                    else
                    {
                        await MovePlayer(currentPlayer, dice1, dice2);
                        if (currentPlayer.currentSquare.id < sqaureIdBeforeMoving)
                        {
                            await PlayerPassedGo(currentPlayer);
                        }
                        await HandleSquare();
                    }
                }
                else
                {
                    doubleStreak = 0;
                    rolledADouble = false;
                    await MovePlayer(currentPlayer, dice1, dice2);
                    if (currentPlayer.currentSquare.id < sqaureIdBeforeMoving)
                    {
                        await PlayerPassedGo(currentPlayer);
                    }
                    await HandleSquare();

                }


            }
        }
        public async Task HandlePlayerInParking()
        {
            if (currentPlayer.turnsToWaitInParking > 0)
            {
                currentPlayer.turnsToWaitInParking--;
                string text = $"{currentPlayer.GetName()} need to wait {currentPlayer.turnsToWaitInParking} turns in parking after this turn";
                await SendMessage(text);
            }
            else
            {
                currentPlayer.inParking = false;
                string text = $"{currentPlayer.GetName()} is free from parking";
                await SendMessage(text);
                await DoRollDice();

                if (dice1 == dice2)
                {
                    doubleStreak++;
                    rolledADouble = true;
                    text = $"{currentPlayer.GetName()} rolled a double!";
                    await SendMessage(text);
                    await MovePlayer(currentPlayer, dice1, dice2);
                    await HandleSquare();
                }
                else
                {
                    await MovePlayer(currentPlayer, dice1, dice2);
                    await HandleSquare();
                }
            }

        }
        public async Task HandlePlayerInJail()
        {
            if (currentPlayer.turnsToWaitInJail > 0)
            {
                currentPlayer.turnsToWaitInJail--;
                if (currentPlayer.outOfJailCards > 0)
                {
                    string text = $"currently you have {currentPlayer.outOfJailCards} out of jail card/s. would you like to use one?";
                    await SendUseOutOfJailCardOffer(currentPlayer, text);
                    if (toUseCard)
                    {
                        currentPlayer.outOfJailCards--;
                        currentPlayer.inJail = false;
                        currentPlayer.turnsToWaitInJail = 0;
                        Globals.lobbyCtrl.SendAmountOfOutOfJailCards(currentPlayer.GetId(), currentPlayer.outOfJailCards);
                        text = $"{currentPlayer.GetName()} have used an out of jail card. As a result he is now free from jail";
                        await SendMessage(text);
                        await DoRollDice();
                        if (dice1 == dice2)
                        {
                            doubleStreak++;
                            rolledADouble = true;
                            text = $"{currentPlayer.GetName()} rolled a double!";
                            await SendMessage(text);
                            await MovePlayer(currentPlayer, dice1, dice2);
                            await HandleSquare();
                        }
                        else
                        {
                            await MovePlayer(currentPlayer, dice1, dice2);
                            await HandleSquare();
                        }

                    }

                }
                else
                {
                    await DoRollDice();

                    if (dice1 == dice2)
                    {
                        currentPlayer.inJail = false;
                        currentPlayer.turnsToWaitInJail = 0;
                        doubleStreak++;
                        rolledADouble = true;
                        string text = $"{currentPlayer.GetName()} rolled a double! he is free from jail";
                        await SendMessage(text);
                        await MovePlayer(currentPlayer, dice1, dice2);
                        await HandleSquare();
                    }
                    else
                    {
                        string text = $"{currentPlayer.GetName()} didnt roll a double. after this turn {currentPlayer.GetName()} would have to wait {currentPlayer.turnsToWaitInJail} in jail unless he rolls a double";
                        await SendMessage(text);
                    }
                }
            }
            else
            {
                currentPlayer.inJail = false;
                string text = $"{currentPlayer.GetName()} is free from jail";
                await SendMessage(text);
                await DoRollDice();

                if (dice1 == dice2)
                {
                    doubleStreak++;
                    rolledADouble = true;
                    text = $"{currentPlayer.GetName()} rolled a double!";
                    await SendMessage(text);
                    await MovePlayer(currentPlayer, dice1, dice2);
                    await HandleSquare();
                }
                else
                {
                    await MovePlayer(currentPlayer, dice1, dice2);
                    await HandleSquare();
                }
            }
        }
        public async Task SendUseOutOfJailCardOffer(Player player, string text)
        {
            Globals.lobbyCtrl.UseOutOfJailCardOffer(player, text);
            await playerClickedUseOrDontUseButton.Task;
            playerClickedUseOrDontUseButton = new TaskCompletionSource<bool>();
        }
        public async Task DoRollDice()
        {
            Globals.lobbyCtrl.RollDiceAvailable(currentPlayer);
            await rollDiceClicked.Task;
            rollDiceClicked = new TaskCompletionSource<bool>();


            RollDices();
            Globals.lobbyCtrl.SendDicesValue(dice1, dice2);
            await rollDiceEnded.Task;


            foreach (Player p in Globals.players)
            {
                p.rollDiceEnded = false;
            }
            rollDiceEnded = new TaskCompletionSource<bool>();
        }
        public async Task MovePlayer(Player player, int dice1, int dice2)
        {
            currentPlayer.MovePlayer(dice1 + dice2);
            Globals.lobbyCtrl.MovePlayer(currentPlayer, dice1 + dice2);
            await movingEnded.Task;
            ResetMovingEndedStats();
        }    
        public async Task HandleSquare()
        {
            Square currentSquare = currentPlayer.currentSquare;
            ServerForm.s_instance.UpdateDebugOutput($"landed on a street square with Id: {currentSquare.id}");
            if (currentSquare is StreetSquare)
            {
                await HandleStreetSquare();
            }
            else if (currentSquare is TaxSquare)
            {
                await HandleTaxSquare(); 
            }
            else if (currentSquare is TreasureSquare)
            {
                await HandleTreasureSquare();
            }

            else if (currentSquare is ChanceSquare)
            {
                await HandleChanceSquare();
            }
            else if (currentSquare is RailroadSquare)
            {
                await HandleRailroadSquare();
            }
            else if (currentSquare is UtilitySquare)
            {
                await HandleUtilitySquare();
            }

            else if (currentSquare is ParkingSquare)
            {
                await HandleParkingSquare();
            }

            else if (currentSquare is GoToJailSquare)
            {
                await HandleGoToJailSquare();
            }
        }
        public async Task HandleStreetSquare()
        {
            StreetSquare streetSquare = (StreetSquare)currentPlayer.currentSquare;
            if (streetSquare.isOwned)
            {
                if (streetSquare.owner.Equals(currentPlayer)) // currentPlayer is the owner
                {
                    if (currentPlayer.HasAllStreet() && streetSquare.costToBuyHouse < currentPlayer.money && streetSquare.numOfHouses < 5)
                    {
                        string text = $"would you like to add an house to your current square ({streetSquare.name}) for {streetSquare.costToBuyHouse}₪?";
                        await SendBuyHouseOffer(currentPlayer, text);
                        if (toBuy)
                        {
                            streetSquare.numOfHouses++;
                            text = $"{currentPlayer.GetName()} bought a house in {streetSquare.name} for {streetSquare.costToBuyHouse}₪";
                            currentPlayer.money -= streetSquare.costToBuyHouse;
                            Globals.lobbyCtrl.UpdatePlayerMoneyLabel(currentPlayerId, currentPlayer.money);
                            Globals.lobbyCtrl.HouseAdded(currentPlayer.currentSquare.id, streetSquare.numOfHouses);
                            await SendMessage(text);
                        }
                    }
                }
                else // someone owns it
                {
                    int needToPay = streetSquare.NeedToPayIfStepped();
                    if (needToPay < currentPlayer.money)
                    {
                        string text = $"{currentPlayer.GetName()} needs to pay {needToPay}₪ to {streetSquare.owner.GetName()}";
                        await SendMessage(text);
                        currentPlayer.money -= needToPay;
                        streetSquare.owner.money += needToPay;
                        Globals.lobbyCtrl.UpdatePlayerMoneyLabel(currentPlayerId, currentPlayer.money);
                        Globals.lobbyCtrl.UpdatePlayerMoneyLabel(streetSquare.owner.GetId(), streetSquare.owner.money);

                    }
                    else
                    {
                        MakePlayerSpectate(currentPlayer);
                        string text = $"{currentPlayer.GetName()} is not able to pay {needToPay}₪. Therefor, he will be removed from the game and he will be spectating";
                        await SendMessage(text);
                    }
                }
            }
            else // no owner
            {
                int price = streetSquare.price;
                if (price < currentPlayer.money) // able to pay
                {
                    string text = $"would you like to buy {streetSquare.name}?";
                    await SendBuySquareOffer(currentPlayer, text);
                    if (toBuy)
                    {
                        currentPlayer.AddSquare(streetSquare);
                        streetSquare.owner = currentPlayer;
                        streetSquare.isOwned = true;
                        text = $"{currentPlayer.GetName()} bought {streetSquare.name}";
                        currentPlayer.money -= price;
                        Globals.lobbyCtrl.UpdatePlayerMoneyLabel(currentPlayerId, currentPlayer.money);
                        Globals.lobbyCtrl.UpdateSquareWithOwner(currentPlayer.currentSquare.id, currentPlayer.GetId());
                        await SendMessage(text);
                    }
                }
            }
        }
        public async Task HandleTaxSquare()
        {
            TaxSquare taxSquare = (TaxSquare)currentPlayer.currentSquare;
            int needToPay = taxSquare.amountToPay;
            if (needToPay < currentPlayer.money)
            {
                string text = $"{currentPlayer.GetName()} needs to pay {needToPay}₪ because he landed on a tax square";
                await SendMessage(text);
                currentPlayer.money -= needToPay;
                Globals.lobbyCtrl.UpdatePlayerMoneyLabel(currentPlayerId, currentPlayer.money);
            }
            else
            {
                MakePlayerSpectate(currentPlayer);
                string text = $"{currentPlayer.GetName()} is not able to pay {needToPay}₪. Therefor, he will be removed from the game and he will be spectating";
                await SendMessage(text);
            }
        }
        public async Task HandleTreasureSquare()
        {
            Card card = TreasureCards.treasureCards[rnd.Next(0, TreasureCards.treasureCards.Length)];
            await HandleCard(card);
        }
        public async Task HandleChanceSquare()
        {
            Card card = ChanceCards.chanceCards[rnd.Next(0, ChanceCards.chanceCards.Length)];
            await HandleCard(card);
        }
        public async Task HandleRailroadSquare()
        {
            RailroadSquare railroadSquare = (RailroadSquare)currentPlayer.currentSquare;
            if (railroadSquare.isOwned)
            {
                if (!railroadSquare.owner.Equals(currentPlayer)) // currentPlayer is not the owner
                {
                    int needToPay = railroadSquare.NeedToPayIfStepped();
                    if (needToPay < currentPlayer.money)
                    {
                        string text = $"{currentPlayer.GetName()} needs to pay {needToPay}₪ to {railroadSquare.owner.GetName()}";
                        await SendMessage(text);
                        currentPlayer.money -= needToPay;
                        railroadSquare.owner.money += needToPay;
                        Globals.lobbyCtrl.UpdatePlayerMoneyLabel(currentPlayerId, currentPlayer.money);
                        Globals.lobbyCtrl.UpdatePlayerMoneyLabel(railroadSquare.owner.GetId(), railroadSquare.owner.money);
                    }
                    else
                    {
                        MakePlayerSpectate(currentPlayer);
                        string text = $"{currentPlayer.GetName()} is not able to pay {needToPay}₪. Therefor, he will be removed from the game and he will be spectating";
                        await SendMessage(text);
                    }
                }
            }
            else // no owner
            {
                int price = railroadSquare.price;
                if (price < currentPlayer.money) // able to pay
                {
                    string text = $"would you like to buy {railroadSquare.name}?";
                    await SendBuySquareOffer(currentPlayer, text);
                    if (toBuy)
                    {
                        currentPlayer.AddSquare(railroadSquare);
                        railroadSquare.owner = currentPlayer;
                        railroadSquare.isOwned = true;
                        currentPlayer.amountOfTrainsOwned++;
                        text = $"{currentPlayer.GetName()} bought {railroadSquare.name}";
                        currentPlayer.money -= price;
                        Globals.lobbyCtrl.UpdatePlayerMoneyLabel(currentPlayerId, currentPlayer.money);
                        Globals.lobbyCtrl.UpdateSquareWithOwner(currentPlayer.currentSquare.id, currentPlayer.GetId());
                        await SendMessage(text);
                    }
                }
            }
        }
        public async Task HandleUtilitySquare()
        {
            UtilitySquare utilitySquare = (UtilitySquare)currentPlayer.currentSquare;
            if (utilitySquare.isOwned)
            {
                if (!utilitySquare.owner.Equals(currentPlayer))
                {
                    int needToPay = utilitySquare.NeedToPayIfStepped(dice1 + dice2);
                    if (needToPay < currentPlayer.money)
                    {
                        string text = $"{currentPlayer.GetName()} needs to pay {needToPay}₪ to {utilitySquare.owner.GetName()}";
                        await SendMessage(text);
                        currentPlayer.money -= needToPay;
                        utilitySquare.owner.money += needToPay;
                        Globals.lobbyCtrl.UpdatePlayerMoneyLabel(currentPlayerId, currentPlayer.money);
                        Globals.lobbyCtrl.UpdatePlayerMoneyLabel(utilitySquare.owner.GetId(), utilitySquare.owner.money);
                    }
                    else
                    {
                        MakePlayerSpectate(currentPlayer);
                        string text = $"{currentPlayer.GetName()} is not able to pay {needToPay}₪. Therefor, he will be removed from the game and he will be spectating";
                        await SendMessage(text);
                    }
                }
            }
            else
            {
                int price = utilitySquare.price;
                if (price < currentPlayer.money) // able to pay
                {
                    string text = $"would you like to buy {utilitySquare.name}?";
                    await SendBuySquareOffer(currentPlayer, text);
                    if (toBuy)
                    {
                        currentPlayer.AddSquare(utilitySquare);
                        currentPlayer.UtilitySquaresOwned++;
                        utilitySquare.owner = currentPlayer;
                        utilitySquare.isOwned = true;
                        text = $"{currentPlayer.GetName()} bought {utilitySquare.name}";
                        currentPlayer.money -= price;
                        Globals.lobbyCtrl.UpdatePlayerMoneyLabel(currentPlayerId, currentPlayer.money);
                        Globals.lobbyCtrl.UpdateSquareWithOwner(currentPlayer.currentSquare.id, currentPlayer.GetId());
                        await SendMessage(text);
                    }
                }            
            }
        }
        public async Task HandleParkingSquare()
        {
            currentPlayer.inParking = true;
            currentPlayer.turnsToWaitInParking = 2;
            string text = $"{currentPlayer.GetName()} needs to wait 2 more turns after this turn until he can play";
            await SendMessage(text);
            rolledADouble = false;
            doubleStreak = 0;
        }
        public async Task HandleGoToJailSquare()
        {
            currentPlayer.inJail = true;
            currentPlayer.turnsToWaitInJail = 3;
            string text = $"{currentPlayer.GetName()} was sent to jail, he will need to wait 3 turns unless he rolls a double";
            await SendMessage(text);
            Globals.lobbyCtrl.PlayerNeedsToGoToSquare(currentPlayer.GetId(), Board.squares[10].id);
            await movingEnded.Task;
            ResetMovingEndedStats();
            currentPlayer.MovePlayer(Board.squares[10]);

            doubleStreak = 0;
            rolledADouble = false;
        }
        public async Task HandleCard(Card card)
        {
            ServerForm.s_instance.UpdateDebugOutput(card.message);
            await SendMessage(card.message);

            if (card is GoToCard)
            {
                GoToCard goTo = (GoToCard)card;
                int squareIdBeforeMoving = currentPlayer.currentSquare.id;
                currentPlayer.MovePlayer(goTo.to);
                Globals.lobbyCtrl.PlayerNeedsToGoToSquare(currentPlayer.GetId(), goTo.to.id);// here
                await movingEnded.Task;
                ResetMovingEndedStats();

                if (goTo.to.id < squareIdBeforeMoving)
                    await PlayerPassedGo(currentPlayer);

                await HandleSquare();
            }
            else if (card is GoToJailCard)
            {
                currentPlayer.MovePlayer(Board.squares[10]);
                Globals.lobbyCtrl.PlayerNeedsToGoToSquare(currentPlayer.GetId(), 10);// here
                await movingEnded.Task;
                ResetMovingEndedStats();
                currentPlayer.inJail = true;
                currentPlayer.turnsToWaitInJail = 3;
                doubleStreak = 0;
                rolledADouble = false;

            }
            else if (card is OutOfJailCard)
            {
                currentPlayer.outOfJailCards++;
                Globals.lobbyCtrl.SendAmountOfOutOfJailCards(currentPlayer.GetId(), currentPlayer.outOfJailCards);
            }
            else if (card is PayToBankCard) 
            {
                PayToBankCard payToBankCard = (PayToBankCard)card;
                if (currentPlayer.money > payToBankCard.toPay)
                {
                    currentPlayer.money -= payToBankCard.toPay;
                    Globals.lobbyCtrl.UpdatePlayerMoneyLabel(currentPlayerId, currentPlayer.money);
                }
                else
                {
                    MakePlayerSpectate(currentPlayer);
                    string text = $"{currentPlayer.GetName()} is not able to pay {payToBankCard.toPay}₪. Therefor, he will be removed from the game and he will be spectating";
                    await SendMessage(text);
                }
            }
            else if (card is ReceiveMoneyFromBank)
            {
                ReceiveMoneyFromBank receiveMoneyFromBank = (ReceiveMoneyFromBank)card;
                currentPlayer.money += receiveMoneyFromBank.toReceive;
                Globals.lobbyCtrl.UpdatePlayerMoneyLabel(currentPlayerId, currentPlayer.money);
            }


        }
        public async Task SendBuySquareOffer(Player player, string text)
        {
            Globals.lobbyCtrl.BuySquareOffer(player, text);
            await playerClickedBuyOrDontBuyButton.Task;
            playerClickedBuyOrDontBuyButton = new TaskCompletionSource<bool>();
        }
        public async Task SendBuyHouseOffer(Player player, string text)
        {
            Globals.lobbyCtrl.BuyHouseOffer(player, text);
            await playerClickedBuyOrDontBuyButton.Task;
            playerClickedBuyOrDontBuyButton = new TaskCompletionSource<bool>();
        }
        public async Task PlayerPassedGo(Player player)
        {
            string text = $"{player.GetName()} finished a lap, he would now receive 200₪";
            player.money += 200;
            Globals.lobbyCtrl.UpdatePlayerMoneyLabel(currentPlayer.GetId(), player.money);
            await SendMessage(text);
        }                       
        public void ResetMovingEndedStats()
        {
            foreach (Player player in Globals.players)
            {
                player.movingEnded = false;
            }
            movingEnded = new TaskCompletionSource<bool>();
        }
        public void RollDices()
        {
            dice1 = rnd.Next(1, 7);
            dice2 = rnd.Next(1, 7);
        }
        public async Task HandlePassedMaxDoubleStreak()
        {
            string text = $"{currentPlayer.GetName()} have passed the max double streak. as a result, he will be sent to jail";
            await SendMessage(text);
            currentPlayer.MovePlayer(Board.squares[10]);
            Globals.lobbyCtrl.PlayerNeedsToGoToSquare(currentPlayer.GetId(), 10);// here
            await movingEnded.Task;
            ResetMovingEndedStats();
            currentPlayer.inJail = true;
            currentPlayer.turnsToWaitInJail = 3;
            doubleStreak = 0;
            rolledADouble = false;
        }        
        public void MakePlayerSpectate(Player player)
        {
            player.RemovePlayerAssets();
            Globals.lobbyCtrl.MakePlayerSpectateInClients(player.GetId());
            currentPlayer.inGame = false;
        }
    }
}