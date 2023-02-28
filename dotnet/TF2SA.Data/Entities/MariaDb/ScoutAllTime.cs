﻿namespace TF2SA.Data.Entities.MariaDb;

public partial class ScoutAllTime
{
	public ulong SteamId { get; set; }
	public string? PlayerName { get; set; }
	public string? Avatar { get; set; }
	public ushort? NumberOfGames { get; set; }
	public uint? Wins { get; set; }
	public float? WinPercentage { get; set; }
	public uint? Draws { get; set; }
	public uint? Losses { get; set; }
	public float? AverageDpm { get; set; }
	public float? AverageKills { get; set; }
	public float? AverageAssists { get; set; }
	public float? AverageDeaths { get; set; }
	public float? AverageDamageTakenPm { get; set; }
	public float? AverageHealsReceivedPm { get; set; }
	public float? AverageMedKitsHp { get; set; }
	public float? AverageCapturePointsCaptured { get; set; }
	public ushort? TopKills { get; set; }
	public uint? TopKillsGameId { get; set; }
	public ushort? TopDamage { get; set; }
	public uint? TopDamageGameId { get; set; }
}
