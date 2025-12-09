using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public class PayToBankCard : Card
    {
        public int toPay;
        public PayToBankCard(string message, int toPay) : base(message)
        {
            this.toPay = toPay;
        }

    }
}
