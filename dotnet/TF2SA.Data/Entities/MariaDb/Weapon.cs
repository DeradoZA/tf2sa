using System;
using System.Collections.Generic;

namespace TF2SA.Data.Entities.MariaDb
{
    public partial class Weapon
    {
        public Weapon()
        {
            WeaponStats = new HashSet<WeaponStat>();
        }

        public ushort WeaponId { get; set; }
        public string? WeaponName { get; set; }

        public virtual ICollection<WeaponStat> WeaponStats { get; set; }
    }
}
