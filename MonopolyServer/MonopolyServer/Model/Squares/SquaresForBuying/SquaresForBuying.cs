using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public abstract class SquaresForBuying : Square
    {
        public int price;
        public Player owner;
        public bool isOwned;
        public SquaresForBuying(string name, int price, int id) : base(name, id)
        {
            this.price = price;
            isOwned = false;

        }
    }


}
