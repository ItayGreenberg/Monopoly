using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public class TreasureCards
    {
        public static Card[] treasureCards =
    {

            new GoToCard($"Go to {Board.squares[39].name}", Board.squares[39]),
            new GoToCard($"Go to {Board.squares[0].name}", Board.squares[0]),
            new GoToCard($"Go to {Board.squares[8].name}" , Board.squares[8]),
            new GoToCard($"Go to {Board.squares[5].name}" , Board.squares[5]),
            new GoToCard($"Go to {Board.squares[11].name}", Board.squares[11]),


            new ReceiveMoneyFromBank("Bank error in your favor. Collect 200₪", 200),
            new ReceiveMoneyFromBank("Holiday fund matures. Receive 50₪", 50),
            new ReceiveMoneyFromBank("You inherit 100₪", 100),
            new ReceiveMoneyFromBank("You have been in an accident. Receive 150₪ from insurance", 150),
            new ReceiveMoneyFromBank("You won a bet. Receive 25₪", 25),


            new PayToBankCard("Doctor’s fee. Pay 50₪", 50),
            new PayToBankCard("Pay hospital fees of 100₪", 100),
            new PayToBankCard("Pay school fees of 50₪", 50),
            new PayToBankCard("You lost a bet. Pay 25₪", 25),
            new PayToBankCard("You got robbed. you lost 200₪", 200),


            new GoToJailCard("Go to Jail. Go directly to jail, do not pass Go, do not collect 200₪"),

            new OutOfJailCard("Receive an out of jail card")

        };
    }
}
