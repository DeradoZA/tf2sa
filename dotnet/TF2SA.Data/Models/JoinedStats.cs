using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Models
{
    public class JoinedStats
    {
      
        public ulong SteamId { get; set; }
        public string? PlayerName { get; set; }
        public uint PlayerStatsId { get; set; }
        public uint GameId { get; set; }
        public byte TeamId { get; set; }
        public uint? DamageTaken { get; set; }
        public uint? HealsReceived { get; set; }
        public uint? MedkitsHp { get; set; }
        public byte? Airshots { get; set; }
        public byte? Headshots { get; set; }
        public byte? Backstabs { get; set; }
        public byte? Drops { get; set; }
        public uint? Heals { get; set; }
        public byte? Ubers { get; set; }
        public uint ClassStatsId { get; set; }
        public byte ClassId { get; set; }
        public ushort? Playtime { get; set; }
        public byte? Kills { get; set; }
        public byte? Assists { get; set; }
        public byte? Deaths { get; set; }
        public uint? Damage { get; set; }

        public JoinedStats(ulong SteamId, string? PlayerName, uint PlayerStatsId,
                           uint GameId, byte TeamId, uint? DamageTaken,
                           uint? HealsReceived, uint? MedkitsHp, byte? Airshots,
                           byte? Headshots, byte? Backstabs, byte? Drops,
                           uint? Heals, byte? Ubers, uint ClassStatsId,
                           byte ClassId, ushort? Playtime, byte? Kills,
                           byte? Assists, byte? Deaths, uint? Damage)
        {
            this.SteamId = SteamId;
            this.PlayerName = PlayerName;
            this.PlayerStatsId = PlayerStatsId;
            this.GameId = GameId;
            this.TeamId = TeamId;
            this.DamageTaken = DamageTaken;
            this.HealsReceived = HealsReceived;
            this.MedkitsHp = MedkitsHp;
            this.Airshots = Airshots;
            this.Headshots = Headshots;
            this.Backstabs = Backstabs;
            this.Drops = Drops;
            this.Heals = Heals;
            this.Ubers = Ubers;
            this.ClassStatsId = ClassStatsId;
            this.ClassId = ClassId;
            this.Playtime = Playtime;
            this.Kills = Kills;
            this.Assists = Assists;
            this.Deaths = Deaths;
            this.Damage = Damage;
        }

    }
}