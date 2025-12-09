using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyClient
{
    public abstract class Square
    {
        private static int index = 0;
        public string name;
        public int id;
        public int width;
        public int height;
        public Square(string name, int width, int height)
        {
            this.name = name;
            id = index;
            index++;
            this.width = width;
            this.height = height;
        }
    }
}
