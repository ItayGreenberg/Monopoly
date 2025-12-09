using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public class TaxSquare : Square
    {
        public int amountToPay;
        public TaxSquare(string name, int amountToPay, int id) : base(name, id)
        {
            this.amountToPay = amountToPay;
        }
    }
}
