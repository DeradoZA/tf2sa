namespace TF2SA.Data.Entities.MariaDb;

public partial class ClassStatEntity
{
	public uint ClassStatsId { get; set; }

	public uint PlayerStatsId { get; set; }

	public byte ClassId { get; set; }

	public byte Kills { get; set; }

	public byte Assists { get; set; }

	public byte Deaths { get; set; }

	public uint Damage { get; set; }

	public ushort Playtime { get; set; }

	public virtual PlayerStatEntity PlayerStatsEntity { get; set; } = null!;

	public virtual ICollection<WeaponStatEntity> WeaponStatsEntities { get; } =
		new List<WeaponStatEntity>();
}
