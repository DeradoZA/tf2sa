namespace TF2SA.Common.Models.LogsTF.GameLogModel;

public class KillStreak
{
	public string SteamId { get; set; } = string.Empty;
	public int Streak { get; set; }
	public int Time { get; set; }
}
