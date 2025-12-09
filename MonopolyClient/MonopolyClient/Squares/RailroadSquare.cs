using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyClient
{
    public class RailroadSquare : Square
    {
        public static int[] PriceForEachTrain = {0, 25, 50, 100, 200 };
        public int price;
        public RailroadSquare(string name, int price, int width, int height) : base(name, width, height)
        {
            this.price = price;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Price is: {price} \r\n");
            sb.Append("Payment for each train you will have: \r\n");
            for (int i = 0; i < PriceForEachTrain.Length; i++)
            {
                sb.Append($"For {i}: {PriceForEachTrain[i]} \r\n");
            }
            return sb.ToString();
        }
    }
}
