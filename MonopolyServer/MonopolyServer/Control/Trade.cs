using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public class Trade
    {
        public MessageId tradeType;
        public Player offerer;
        public Player receiver;
        public List<Square> squaresOffered;
        public List<Square> squaresAsked;
        public int priceOffered;
        public int priceRequested;
    }
}
