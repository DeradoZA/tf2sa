namespace TF2SA.Data.Analytics.PlayerDetail;

/// <summary>
/// A row of <c>player_weapon_splits(steamid)</c> — one weapon the player has used.
/// Accuracy is null when every game that used this weapon lacked HasAccuracy.
/// </summary>
public class PlayerWeaponSplitRow
{
	public string? WeaponName { get; set; }
	public int NumberOfGames { get; set; }
	public int TotalKills { get; set; }
	public int TotalDamage { get; set; }
	public float? Accuracy { get; set; }
}
