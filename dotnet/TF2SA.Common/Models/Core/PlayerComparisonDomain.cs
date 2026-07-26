namespace TF2SA.Common.Models.Core;

/// <summary>One selected player's progression within a comparison.</summary>
public class PlayerComparisonDomain
{
	public ulong SteamId { get; set; }
	public string? PlayerName { get; set; }
	public string? Avatar { get; set; }
	public int NumberOfGames { get; set; }
	public IEnumerable<PlayerProgressionPointDomain> Progression { get; set; } =
		Enumerable.Empty<PlayerProgressionPointDomain>();
}
