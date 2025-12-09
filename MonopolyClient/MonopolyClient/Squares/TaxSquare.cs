using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyClient
{
    public class TaxSquare : Square
    {
        public int amountToPay;
        public TaxSquare(string name, int amountToPay, int width, int height) : base(name, width, height) 
        {
            this.amountToPay = amountToPay;
        }

    }
}
