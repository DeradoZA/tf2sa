using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class PlayerGamesCount
    {
        public ulong SteamId { get; set; }
        public int NumberOfGames { get; set; }
        public PlayerGamesCount(ulong steamId, int numberOfGames)
        {
            this.SteamId = steamId;
            this.NumberOfGames = numberOfGames;
        }


    }
}