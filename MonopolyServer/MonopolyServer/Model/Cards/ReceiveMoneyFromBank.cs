using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public class ReceiveMoneyFromBank : Card
    {
        public int toReceive;
        public ReceiveMoneyFromBank(string message, int toReceive) : base(message)
        {
            this.toReceive = toReceive;
        }
    }
}
