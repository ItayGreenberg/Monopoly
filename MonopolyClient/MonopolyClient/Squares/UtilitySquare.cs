using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyClient
{
    public class UtilitySquare : Square
    {
        public int price;
        public UtilitySquare(string name, int price, int width, int height) : base(name, width, height)
        {
            this.price = price;
        }
        public override string ToString()
        {
            return "if you only have 1 utility square you would receive player steps times 4 and if you have 2 utility squares you would receieve player steps times 10";
        }
    }
}
