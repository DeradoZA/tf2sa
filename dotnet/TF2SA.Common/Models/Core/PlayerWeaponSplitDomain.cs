namespace TF2SA.Common.Models.Core;

public class PlayerWeaponSplitDomain
{
	public string? WeaponName { get; set; }
	public int NumberOfGames { get; set; }
	public int TotalKills { get; set; }
	public int TotalDamage { get; set; }
	public float? Accuracy { get; set; }
}
