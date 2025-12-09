using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyClient
{
    public class StreetSquare : Square
    {
        public int price;
        public int[] paymentForEachHouse;
        public int costToBuyHouse;
        public int streetId;
        public StreetSquare(string name, int price, int[] paymentForEachHouse, int costToBuyHouse, int streetId, int width, int height) : base(name, width, height)
        {
            this.price = price;
            this.paymentForEachHouse = paymentForEachHouse;
            this.costToBuyHouse = costToBuyHouse;
            this.streetId = streetId;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(name);
            sb.Append("\r\n");
            sb.Append($"Price: {price}");
            sb.Append("\r\n");
            sb.Append("Each house costs " + costToBuyHouse);
            sb.Append("\r\n");
            sb.Append("For each house players would need to pay:");
            sb.Append("\r\n");
            for (int i = 0; i < paymentForEachHouse.Length; i++)
            {
                sb.Append($"for {i} houses: {paymentForEachHouse[i]}");
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

    }
}
