
namespace MonopolyClient
{
    public class Board
    { 
        public static int rowSquareWidth = 96;
        public static int rowSquareHeight = 120;

        public static int columnSquareWidth = 144;
        public static int columnSquareHeight = 80;

        public static int bigSquareWidth = 144;
        public static int bigSquareHeight = 120;
        public static Square[] squares = {
            new GoSquare("Go", bigSquareWidth, bigSquareHeight),
            new StreetSquare("Mediterranean Avenue", 60, [2,10,30,90,160,250],50, 1,rowSquareWidth, rowSquareHeight),
            new TreasureSquare("Treasure Square", rowSquareWidth, rowSquareHeight),
            new StreetSquare("Baltic Avenue", 60, [4,20,60,180,320,450], 50, 1, rowSquareWidth, rowSquareHeight),
            new TaxSquare("Income Tax", 200, rowSquareWidth, rowSquareHeight),
            new RailroadSquare("Reading Railroad", 200, rowSquareWidth, rowSquareHeight),
            new StreetSquare("Oriental Avenue", 100, [6,30,90,270,400,550],50, 2, rowSquareWidth, rowSquareHeight),
            new ChanceSquare("Chance", rowSquareWidth, rowSquareHeight),
            new StreetSquare("Vermont Avenue", 100, [6,30,90,270,400,550],50, 2, rowSquareWidth, rowSquareHeight),
            new StreetSquare("Connecticut Avenue", 120, [8,40,100,300,450,600],50, 2, rowSquareWidth, rowSquareHeight),
            new JailSquare("Jail / Visit", bigSquareWidth, bigSquareHeight),
            new StreetSquare("St. Charles Place", 140, [10, 50, 150, 450, 625, 750],100, 3, columnSquareWidth, columnSquareHeight),
            new UtilitySquare("Electric Company", 150,  columnSquareWidth, columnSquareHeight),
            new StreetSquare("States Avenue", 140, [10, 50, 150, 450, 625, 750], 100, 3, columnSquareWidth, columnSquareHeight),
            new StreetSquare("Virginia Avenue", 160, [12,60,180,500,700,900],100, 3, columnSquareWidth, columnSquareHeight),
            new RailroadSquare("Pennsylvania Railroad", 200,  columnSquareWidth, columnSquareHeight),
            new StreetSquare("St. James Place", 180, [14,70, 200, 550, 750, 950],100, 4, columnSquareWidth, columnSquareHeight),
            new TreasureSquare("Community Chest",  columnSquareWidth, columnSquareHeight),
            new StreetSquare("Tennessee Avenue", 180, [14,70, 200, 550, 750, 950],100, 4, columnSquareWidth, columnSquareHeight),
            new StreetSquare("New York Avenue", 200, [16,80,220,600,800,1000],100, 4, columnSquareWidth, columnSquareHeight),
            new ParkingSquare("Free Parking",  bigSquareWidth, bigSquareHeight),
            new StreetSquare("Kentucky Avenue", 220, [18,90,250,700,875,1050],150, 5, rowSquareWidth , rowSquareHeight),
            new ChanceSquare("Chance", rowSquareWidth , rowSquareHeight),
            new StreetSquare("Indiana Avenue", 220, [18,90,250,700,875,1050], 150, 5, rowSquareWidth , rowSquareHeight),
            new StreetSquare("Illinois Avenue", 240, [20,100,300,750,925,1100], 150, 5, rowSquareWidth , rowSquareHeight),
            new RailroadSquare("B. & O. Railroad", 200, rowSquareWidth , rowSquareHeight),
            new StreetSquare("Atlantic Avenue", 260, [22,110,330,800,975,1150], 150, 6, rowSquareWidth , rowSquareHeight),
            new StreetSquare("Ventnor Avenue", 260, [22,110,330,800,975,1150], 150, 6, rowSquareWidth , rowSquareHeight),
            new UtilitySquare("Water Works", 150, rowSquareWidth , rowSquareHeight),
            new StreetSquare("Marvin Gardens", 280, [24,120,360,850,1025,1200], 150, 6, rowSquareWidth , rowSquareHeight),
            new GoToJailSquare("Go To Jail", bigSquareWidth, rowSquareHeight),
            new StreetSquare("Pacific Avenue", 300, [26,130,390,900,1100,1275], 200, 7, columnSquareWidth, columnSquareHeight),
           new StreetSquare("North Carolina Avenue", 300, [26,130,390,900,1100,1275],200, 7, columnSquareWidth, columnSquareHeight),
            new TreasureSquare("Community Chest", columnSquareWidth, columnSquareHeight),
            new StreetSquare("Pennsylvania Avenue", 320, [28,150,450,1000,1200,1400],200, 7, columnSquareWidth, columnSquareHeight),
            new RailroadSquare("Short Line", 200, columnSquareWidth, columnSquareHeight),
            new ChanceSquare("Chance", columnSquareWidth, columnSquareHeight),
            new StreetSquare("Park Place", 350, [35,175,500,1100,1300,1500],200, 8, columnSquareWidth, columnSquareHeight),
            new TaxSquare("Luxury Tax", 100, columnSquareWidth, columnSquareHeight),
            new StreetSquare("Boardwalk", 400, [50,200,600,1400,1700,2000],200, 8, bigSquareWidth, bigSquareHeight)
        };

    }
}
