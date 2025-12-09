using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public class Board
    {
        public static Square[] squares = {
            new GoSquare("Go", 0),
                new StreetSquare("Mediterranean Avenue", 60, [2,10,30,90,160,250], 50, 1, 1),
                new TreasureSquare("Treasure Square",  2),
                new StreetSquare("Baltic Avenue", 60, [4,20,60,180,320,450], 50, 1,  3),
                new TaxSquare("Income Tax", 200,  4),
                new RailroadSquare("Reading Railroad", 200,  5),
                new StreetSquare("Oriental Avenue", 100, [6,30,90,270,400,550],50, 2, 6),
                new ChanceSquare("Chance", 7),
                new StreetSquare("Vermont Avenue", 100, [6,30,90,270,400,550], 50, 2, 8),
                new StreetSquare("Connecticut Avenue", 120, [8,40,100,300,450,600], 50, 2, 9),
                new JailSquare("Jail / Visit",  10),
                new StreetSquare("St. Charles Place", 140, [10, 50, 150, 450, 625, 750], 100, 3, 11),
                new UtilitySquare("Electric Company", 150, 12),
                new StreetSquare("States Avenue", 140, [10, 50, 150, 450, 625, 750], 100, 3,  13),
                new StreetSquare("Virginia Avenue", 160, [12,60,180,500,700,900], 100, 3, 14),
                new RailroadSquare("Pennsylvania Railroad", 200, 15),
                new StreetSquare("St. James Place", 180, [14,70, 200, 550, 750, 950], 100, 4,  16),
                new TreasureSquare("Community Chest",  17),
                new StreetSquare("Tennessee Avenue", 180, [14,70, 200, 550, 750, 950], 100, 4, 18),
                new StreetSquare("New York Avenue", 200, [16,80,220,600,800,1000], 100, 4, 19),
                new ParkingSquare("Free Parking", 20),
                new StreetSquare("Kentucky Avenue", 220, [18,90,250,700,875,1050],150, 5, 21),
                new ChanceSquare("Chance", 22),
                new StreetSquare("Indiana Avenue", 220, [18,90,250,700,875,1050],150, 5, 23),
                new StreetSquare("Illinois Avenue", 240, [20,100,300,750,925,1100], 150, 5, 24),
                new RailroadSquare("B. & O. Railroad", 200, 25),
                new StreetSquare("Atlantic Avenue", 260, [22,110,330,800,975,1150], 150, 6,26),
                new StreetSquare("Ventnor Avenue", 260, [22,110,330,800,975,1150], 150, 6, 27),
                new UtilitySquare("Water Works", 150, 28),
                new StreetSquare("Marvin Gardens", 280, [24,120,360,850,1025,1200], 150, 6, 29),
                new GoToJailSquare("Go To Jail", 30),
                new StreetSquare("Pacific Avenue", 300, [26,130,390,900,1100,1275], 200, 7, 31),
                new StreetSquare("North Carolina Avenue", 300, [26,130,390,900,1100,1275], 200,7, 32),
                new TreasureSquare("Community Chest", 33),
                new StreetSquare("Pennsylvania Avenue", 320, [28,150,450,1000,1200,1400], 200, 7, 34),
                new RailroadSquare("Short Line", 200, 35),
                new ChanceSquare("Chance", 36),
                new StreetSquare("Park Place", 350, [35,175,500,1100,1300,1500], 200,8, 37),
                new TaxSquare("Luxury Tax", 100, 38),
                new StreetSquare("Boardwalk", 400, [50,200,600,1400,1700,2000], 200,8, 39)

        };
    }
}
