namespace TF2SA.Web.Controllers.V1.Statistics.Medic;

public class MedicStatHttpResult
{
	public string SteamId { get; set; } = string.Empty;
	public string? PlayerName { get; set; }
	public string? Avatar { get; set; }
	public ushort? NumberOfGames { get; set; }
	public uint? Wins { get; set; }
	public float? WinPercentage { get; set; }
	public uint? Draws { get; set; }
	public uint? Losses { get; set; }
	public float? AverageKills { get; set; }
	public float? AverageAssists { get; set; }
	public float? AverageDeaths { get; set; }
	public float? AverageUbers { get; set; }
	public float? AverageDrops { get; set; }
	public float? AverageHealsPm { get; set; }
	public ushort? TopHeals { get; set; }
	public uint? TopHealsGameId { get; set; }
	public ushort? TopUbers { get; set; }
	public uint? TopUbersGameId { get; set; }
	public ushort? TopDrops { get; set; }
	public uint? TopDropsGameId { get; set; }
}
