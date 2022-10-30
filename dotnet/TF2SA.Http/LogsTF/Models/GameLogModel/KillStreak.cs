namespace TF2SA.Http.LogsTF.Models.GameLogModel;

public class KillStreak
{
	public string SteamId { get; set; } = string.Empty;
	public int Streak { get; set; }
	public int Time { get; set; }
}
