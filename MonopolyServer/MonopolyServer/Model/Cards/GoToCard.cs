using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public class GoToCard : Card
    {
        public Square to;
        public GoToCard(string message, Square to) : base(message)
        {
            this.to = to;
        }

    }
}
