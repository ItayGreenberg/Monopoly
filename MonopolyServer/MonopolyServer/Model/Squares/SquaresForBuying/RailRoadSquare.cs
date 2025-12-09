using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public class RailroadSquare : SquaresForBuying
    {
        public static int[] PriceForEachTrain = { 0, 25, 50, 100, 200 };
        public RailroadSquare(string name, int price, int id) : base(name, price, id)
        {
        }

        public int NeedToPayIfStepped() => PriceForEachTrain[owner.amountOfTrainsOwned];
        
    }
}
