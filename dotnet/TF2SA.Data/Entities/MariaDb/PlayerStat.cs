using System;
using System.Collections.Generic;

namespace TF2SA.Data.Entities.MariaDb
{
    public partial class PlayerStat
    {
        public PlayerStat()
        {
            ClassStats = new HashSet<ClassStat>();
            WeaponStats = new HashSet<WeaponStat>();
        }

        public uint PlayerStatsId { get; set; }
        public uint GameId { get; set; }
        public ulong SteamId { get; set; }
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

        public virtual Game Game { get; set; } = null!;
        public virtual Player Steam { get; set; } = null!;
        public virtual ICollection<ClassStat> ClassStats { get; set; }
        public virtual ICollection<WeaponStat> WeaponStats { get; set; }
    }
}
