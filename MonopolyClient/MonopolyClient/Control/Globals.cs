using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyClient
{
    public class Globals
    {
        public static LobbyCtrl LobbyCtrl = new LobbyCtrl();
        public static Player[] players = new Player[4];
    }
}
