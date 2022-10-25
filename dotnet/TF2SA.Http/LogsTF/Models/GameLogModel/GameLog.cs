namespace TF2SA.Http.LogsTF.Models.GameLogModel;

public class GameLog
{
	public int? Version { get; set; }
	public Dictionary<string, TeamStats>? Teams { get; set; }
	public uint Length { get; set; }
	public Dictionary<string, PlayerStats>? Players { get; set; }
	public object? Names { get; set; }
	public object? Rounds { get; set; }
	public object? HealSpread { get; set; }
	public object? ClassKills { get; set; }
	public object? ClassDeaths { get; set; }
	public object? ClassKillAssists { get; set; }
	public object? Chat { get; set; }
	public object? Info { get; set; }
	public object? KillStreaks { get; set; }
	public bool? Success { get; set; }
}
