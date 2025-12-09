using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public abstract class Square
    {
        public string name;
        public int id;
        public Square(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    }
}
