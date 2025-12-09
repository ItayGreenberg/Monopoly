using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public abstract class Card
    {
        public string message;

        public Card(string message)
        {
            this.message = message;
        }
    }
}
