namespace TF2SA.Common.Models.LogsTF.GameLogModel;

[Serializable]
public class KillStreak
{
	public string? SteamId { get; set; }
	public int? Streak { get; set; }
	public int? Time { get; set; }
}
