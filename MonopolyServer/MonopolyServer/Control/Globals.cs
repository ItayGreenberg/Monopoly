using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonopolyServer
{
    public static class Globals
    {
        public static GameCtrl gameCtrl = new GameCtrl();
        public static LobbyCtrl lobbyCtrl = new LobbyCtrl();
        public static ConcurrentBag<Player> players = new ConcurrentBag<Player>();

    }
}
