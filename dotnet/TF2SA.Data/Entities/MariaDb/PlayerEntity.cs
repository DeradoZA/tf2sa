namespace TF2SA.Data.Entities.MariaDb;

public partial class PlayerEntity
{
	public ulong SteamId { get; set; }

	public string? LocalCountryCode { get; set; }

	public string? ProfileUrl { get; set; }

	public string? Avatar { get; set; }

	public string? AvatarMedium { get; set; }

	public string? AvatarFull { get; set; }

	public string? AvatarHash { get; set; }

	public string? PlayerName { get; set; }

	public string? RealName { get; set; }

	public virtual ICollection<PlayerStatEntity> PlayerStatsEntities { get; } =
		new List<PlayerStatEntity>();
}
