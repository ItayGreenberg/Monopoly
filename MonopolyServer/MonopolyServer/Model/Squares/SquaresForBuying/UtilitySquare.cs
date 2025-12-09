using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public class UtilitySquare : SquaresForBuying
    {
        public UtilitySquare(string name, int price, int id) : base(name, price, id)
        {
        }

        public int NeedToPayIfStepped(int steps)
        {
            int multiplier = 0;
            if (owner.UtilitySquaresOwned== 1)
                multiplier = 4;
            else if (owner.UtilitySquaresOwned == 2)
                multiplier = 10;
            return steps * multiplier;
        }
        
    }
}
