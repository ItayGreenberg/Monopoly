using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyClient
{
    public class Message
    {
        public MessageId messageId;
        public List<Square> squaresOffered;
        public List<Square> squaresAsked;
        public int priceOffered;
        public int priceAsked;
        public string serverIp;
        public string name;
        public int id;
        public string message;
    }
}
