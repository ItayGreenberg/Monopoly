using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyClient
{
    public class Player
    {
        public string name;
        public Square currentSquare;
        public List<Square> squaresOwned;
        public int money;
        public int id;
        public Label moneyLabel;
        public PictureBox pictureBox;
        public bool ableToReceiveOffer = true;
        public Player(string name, Label moneyLabel, PictureBox pictureBox, int id)
        {
            currentSquare = Board.squares[0];
            this.name = name;
            squaresOwned = new List<Square>();
            this.id = id;
            money = 1500;
            this.moneyLabel = moneyLabel;
            this.pictureBox = pictureBox;
            moneyLabel.Text = money.ToString();
        }
        public bool IsOwner(Square square)
        {
            foreach(Square s in squaresOwned) 
            {
                if(s.Equals(square)) return true;
            }
            return false;
        }
        /*public bool HasAllStreet(Square square)
        {
            int streetId = ((StreetSquare)square).streetId;
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
        */
        public void RemoveSquare(int squareId)
        {
            foreach (Square s in squaresOwned)
            {
                if (s.id == squareId)
                {
                    squaresOwned.Remove(s);
                    break;
                }
            }
        }
        public List<Square> GetSquares()
        {
            return squaresOwned.ToList();
        }

    }
}
