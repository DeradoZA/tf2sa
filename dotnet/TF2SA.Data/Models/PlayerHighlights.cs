using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class PlayerHighlights
    {
        public PlayerHighlights(ulong steamID,
                                uint? highestScoutDamage, 
                                byte? highestScoutKills,
                                uint? highestSoldierDamage,
                                byte? highestSoldierKills, 
                                byte? highestSoldierAirshots, 
                                uint? highestDemomanDamage, 
                                byte? highestDemomanKills, 
                                byte? highestDemomanAirshots, 
                                byte? highestDrops, 
                                uint? highestHeals, 
                                byte? highestUbers)
        {
            this.SteamID = steamID;
            this.HighestScoutDamage = highestScoutDamage;
            this.HighestScoutKills = highestScoutKills;
            this.HighestSoldierDamage = highestSoldierDamage;
            this.HighestSoldierKills = highestSoldierKills;
            this.HighestSoldierAirshots = highestSoldierAirshots;
            this.HighestDemomanDamage = highestDemomanDamage;
            this.HighestDemomanKills = highestDemomanKills;
            this.HighestDemomanAirshots = highestDemomanAirshots;
            this.HighestDrops = highestDrops;
            this.HighestHeals = highestHeals;
            this.HighestUbers = highestUbers;
        }

        public ulong SteamID { get; set; }
        public uint? HighestScoutDamage { get; set; }
        public byte? HighestScoutKills { get; set; }
        public uint? HighestSoldierDamage { get; set; }
        public byte? HighestSoldierKills { get; set; }
        public byte? HighestSoldierAirshots { get; set; }
        public uint? HighestDemomanDamage { get; set; }
        public byte? HighestDemomanKills { get; set; }
        public byte? HighestDemomanAirshots { get; set; }
        public byte? HighestDrops { get; set; }
        public uint? HighestHeals { get; set; }
        public byte? HighestUbers { get; set; }
    }
}