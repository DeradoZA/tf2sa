using System.ComponentModel.DataAnnotations.Schema;

namespace TF2SA.Data.Entities.MariaDb;

[Table("Players")]
public partial class PlayersEntity
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

	public virtual ICollection<PlayerStatsEntity> PlayerStatsEntities { get; } =
		new List<PlayerStatsEntity>();
}
