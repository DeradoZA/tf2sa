namespace TF2SA.Data.Entities.MariaDb;

public partial class ClassStat
{
	public ClassStat()
	{
		WeaponStats = new HashSet<WeaponStat>();
	}

	public uint ClassStatsId { get; set; }
	public uint PlayerStatsId { get; set; }
	public byte ClassId { get; set; }
	public byte Kills { get; set; }
	public byte Assists { get; set; }
	public byte Deaths { get; set; }
	public uint Damage { get; set; }
	public ushort Playtime { get; set; }

	public virtual PlayerStat PlayerStats { get; set; } = null!;
	public virtual ICollection<WeaponStat> WeaponStats { get; set; }
}
