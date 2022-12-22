namespace TF2SA.Data.Entities.MariaDb;

public partial class WeaponStat
{
	public uint PlayerStatsId { get; set; }
	public ushort WeaponId { get; set; }
	public byte Kills { get; set; }
	public uint Damage { get; set; }
	public double? Accuracy { get; set; }

	public virtual PlayerStat PlayerStats { get; set; } = null!;
	public virtual Weapon Weapon { get; set; } = null!;
}
