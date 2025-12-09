using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public class Player
    {
        private readonly object squaresOwnedLock = new object();
        private string name;
        private int id;
        public bool movingEnded;
        public bool okClicked;
        public bool rollDiceEnded;
        public Square currentSquare;
        public int outOfJailCards;
        private List<Square> squaresOwned;
        public int money;
        public int turnsToWaitInJail;
        public bool inJail;
        public int turnsToWaitInParking;
        public bool inParking;
        public int amountOfTrainsOwned;
        public int UtilitySquaresOwned;
        public bool ready;
        public Socket receiveSocket;
        public Socket sendSocket;
        public int sendSocketPort;
        public bool inGame;
        public bool ableToReceiveOffer;
        public Trade trade = new Trade();
        public Player(string name, int id)
        {
            inGame = true;
            okClicked = false;
            movingEnded = false;
            rollDiceEnded = false;
            ableToReceiveOffer = true;
            currentSquare = Board.squares[0];
            this.name = name;
            squaresOwned = new List<Square>();
            money = 1500;
            turnsToWaitInJail = 0;
            turnsToWaitInParking = 0;
            inJail = false;
            inParking = false;
            this.id = id;
        }

        public string GetName() => name;
        public int GetId() => id;
        public void MovePlayer(int amount)
        {
            currentSquare = Board.squares[(currentSquare.id + amount)% 40];
        }
        public void MovePlayer(Square square)
        {
            currentSquare = square;
        }
        public bool IsOwner(Square square)
        {
            lock (squaresOwnedLock)
            {
                foreach (Square s in squaresOwned)
                {
                    if (s.Equals(square)) return true;
                }
                return false;
            }
        }
        public bool IsOwner(int squareId)
        {
            return IsOwner(Board.squares[squareId]);
        }
        public void AddSquare(Square square)
        {
            lock (squaresOwnedLock)
            {
                squaresOwned.Add(square);
            }
        }
        public void RemoveSquare(int squareId) 
        {
            lock (squaresOwnedLock)
            {
                foreach (Square square in squaresOwned)
                {
                    if (square.id == squareId)
                    {
                        squaresOwned.Remove(square);
                        break;
                    }
                }
            }
        }
        public bool HasAllStreet()
        {
            int streetId = ((StreetSquare)currentSquare).streetId;
            foreach (Square s in Board.squares)
            {
                if (s is StreetSquare)
                {
                    if (((StreetSquare)s).streetId == streetId)
                    {
                        if (!IsOwner(s))
                            return false;
                    }
                }
            }
            return true;
        }
        public void RemovePlayerAssets()
        {
            lock (squaresOwnedLock)
            {
                money = 0;
                foreach (Square square in squaresOwned)
                {
                    if (square is StreetSquare)
                    {
                        StreetSquare streetSquare = (StreetSquare)square;
                        streetSquare.numOfHouses = 0;
                        streetSquare.owner = null;
                        streetSquare.isOwned = false;
                    }
                    else if (square is RailroadSquare)
                    {
                        RailroadSquare railroadSquare = (RailroadSquare)square;
                        railroadSquare.owner = null;
                        railroadSquare.isOwned = false;
                    }
                    else if (square is UtilitySquare)
                    {
                        UtilitySquare utilitySquare = (UtilitySquare)square;
                        utilitySquare.owner = null;
                        utilitySquare.isOwned = false;
                    }
                }
                squaresOwned.Clear();
            }
        }
    }
}
