using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TF2SA.Http.LogsTF.Models
{
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
	
	public class TeamStats
	{
		public int Score { get; set; }
		public int Kills { get; set; }
		public int Deaths { get; set; }
		[JsonPropertyName("dmg")]
		public int Damage { get; set; }
		[JsonPropertyName("charges")]
		public int UberCharges { get; set; }
		public int Drops { get; set; }
		[JsonPropertyName("firstcaps")]
		public int FirstCaptures { get; set; }
		[JsonPropertyName("caps")]
		public int Captures { get; set; }
	}

	public class PlayerStats
	{
		public string? Team { get; set; }
		[JsonPropertyName("class_stats")]
		public ClassStats[]? ClassStats { get; set; }
		public int Kills { get; set; }
		public int Deaths { get; set; }
		public int Assists { get; set; }
		public string? Kapd { get; set; }
		public string? Kpd { get; set; }
		[JsonPropertyName("dmg")]
		public int Damage { get; set; }
		[JsonPropertyName("dmg_real")]
		public int DamageReal { get; set; }
		[JsonPropertyName("dt")]
		public int DamageTaken { get; set; }
		[JsonPropertyName("dt_real")]
		public int DamageTakenReal { get; set; }
		[JsonPropertyName("hr")]
		public int HealsReceived { get; set; }
		[JsonPropertyName("1ks")]
		public int OneKillStreaks { get; set; }
		[JsonPropertyName("as")]
		public int Airshots { get; set; }
		public int Dapd { get; set; }
		public int Dapm { get; set; }
		public int Ubers { get; set; }
		public object? UberTypes { get; set; }
		public int Drops { get; set; }
	}

	public class ClassStats
	{
		public string? Type { get; set; }	
		public int Kills { get; set; }
		public int Assists { get; set; }
		public int Deaths { get; set; }
		[JsonPropertyName("dmg")]
		public int Damage { get; set; }
		[JsonPropertyName("weapon")]
		public Dictionary<string, WeaponStats>? Weapons { get; set; }
		public int TotalTime { get; set; }

	}

    public class WeaponStats
    {
		public int Kills { get; set; }
		[JsonPropertyName("dmg")]
		public int Damage { get; set; }
		[JsonPropertyName("avg_dmg")]
		public int AverageDamage { get; set; }
		public int Shots { get; set; }
		public int Hits { get; set; }
    }
}
