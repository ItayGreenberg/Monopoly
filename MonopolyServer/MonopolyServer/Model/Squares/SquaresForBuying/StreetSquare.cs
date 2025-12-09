using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public class StreetSquare : SquaresForBuying
    {
        public int numOfHouses;
        public int[] paymentForEachHouse;
        public int costToBuyHouse;
        public int streetId;
        public StreetSquare(string name, int price, int[] priceForEachHouse, int amountToBuyHouse, int streetId, int id) : base(name,price, id)
        {
            numOfHouses = 0;
            this.paymentForEachHouse = priceForEachHouse;
            this.costToBuyHouse = amountToBuyHouse;
            this.streetId = streetId;
        }
        public int NeedToPayIfStepped() => paymentForEachHouse[numOfHouses];        
        
    }
}
