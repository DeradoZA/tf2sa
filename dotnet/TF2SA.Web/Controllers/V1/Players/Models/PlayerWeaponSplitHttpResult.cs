namespace TF2SA.Web.Controllers.V1.Players.Models;

public class PlayerWeaponSplitHttpResult
{
	public string? WeaponName { get; set; }
	public int NumberOfGames { get; set; }
	public int TotalKills { get; set; }
	public int TotalDamage { get; set; }
	public float? Accuracy { get; set; }
}
