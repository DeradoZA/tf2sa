using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics.PlayerDetail;
using TF2SA.Data.Analytics.Rows;

namespace TF2SA.Data.Mapping;

/// <summary>
/// Hand-written Row -&gt; Domain mapping, replacing AutoMapper's flat CreateMap calls
/// (former GameMappingProfile). Every pair here is a same-shape field-for-field copy —
/// the leaderboard row types (ScoutAllTime etc.) and the PlayerDetail row types carry
/// no field AutoMapper's convention-based mapping did anything beyond copy by name.
/// </summary>
public static class DomainMappingExtensions
{
	public static ScoutStatDomain ToDomain(this ScoutRecent row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			Avatar = row.Avatar,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			WinPercentage = row.WinPercentage,
			Draws = row.Draws,
			Losses = row.Losses,
			AverageDpm = row.AverageDpm,
			AverageKills = row.AverageKills,
			AverageAssists = row.AverageAssists,
			AverageDeaths = row.AverageDeaths,
			AverageDamageTakenPm = row.AverageDamageTakenPm,
			AverageHealsReceivedPm = row.AverageHealsReceivedPm,
			AverageMedKitsHp = row.AverageMedKitsHp,
			AverageCapturePointsCaptured = row.AverageCapturePointsCaptured,
			TopKills = row.TopKills,
			TopKillsGameId = row.TopKillsGameId,
			TopDamage = row.TopDamage,
			TopDamageGameId = row.TopDamageGameId,
		};

	public static ScoutStatDomain ToDomain(this ScoutAllTime row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			Avatar = row.Avatar,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			WinPercentage = row.WinPercentage,
			Draws = row.Draws,
			Losses = row.Losses,
			AverageDpm = row.AverageDpm,
			AverageKills = row.AverageKills,
			AverageAssists = row.AverageAssists,
			AverageDeaths = row.AverageDeaths,
			AverageDamageTakenPm = row.AverageDamageTakenPm,
			AverageHealsReceivedPm = row.AverageHealsReceivedPm,
			AverageMedKitsHp = row.AverageMedKitsHp,
			AverageCapturePointsCaptured = row.AverageCapturePointsCaptured,
			TopKills = row.TopKills,
			TopKillsGameId = row.TopKillsGameId,
			TopDamage = row.TopDamage,
			TopDamageGameId = row.TopDamageGameId,
		};

	public static SoldierStatDomain ToDomain(this SoldierRecent row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			Avatar = row.Avatar,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			WinPercentage = row.WinPercentage,
			Draws = row.Draws,
			Losses = row.Losses,
			AverageDpm = row.AverageDpm,
			AverageKills = row.AverageKills,
			AverageAssists = row.AverageAssists,
			AverageDeaths = row.AverageDeaths,
			AverageAirshots = row.AverageAirshots,
			AverageDamageTakenPm = row.AverageDamageTakenPm,
			AverageHealsReceivedPm = row.AverageHealsReceivedPm,
			AverageMedKitsHp = row.AverageMedKitsHp,
			AverageCapturePointsCaptured = row.AverageCapturePointsCaptured,
			TopKills = row.TopKills,
			TopKillsGameId = row.TopKillsGameId,
			TopDamage = row.TopDamage,
			TopDamageGameId = row.TopDamageGameId,
			TopAirshots = row.TopAirshots,
			TopAirshotsGameId = row.TopAirshotsGameId,
		};

	public static SoldierStatDomain ToDomain(this SoldierAllTime row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			Avatar = row.Avatar,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			WinPercentage = row.WinPercentage,
			Draws = row.Draws,
			Losses = row.Losses,
			AverageDpm = row.AverageDpm,
			AverageKills = row.AverageKills,
			AverageAssists = row.AverageAssists,
			AverageDeaths = row.AverageDeaths,
			AverageAirshots = row.AverageAirshots,
			AverageDamageTakenPm = row.AverageDamageTakenPm,
			AverageHealsReceivedPm = row.AverageHealsReceivedPm,
			AverageMedKitsHp = row.AverageMedKitsHp,
			AverageCapturePointsCaptured = row.AverageCapturePointsCaptured,
			TopKills = row.TopKills,
			TopKillsGameId = row.TopKillsGameId,
			TopDamage = row.TopDamage,
			TopDamageGameId = row.TopDamageGameId,
			TopAirshots = row.TopAirshots,
			TopAirshotsGameId = row.TopAirshotsGameId,
		};

	public static DemomanStatDomain ToDomain(this DemomanRecent row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			Avatar = row.Avatar,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			WinPercentage = row.WinPercentage,
			Draws = row.Draws,
			Losses = row.Losses,
			AverageDpm = row.AverageDpm,
			AverageKills = row.AverageKills,
			AverageAssists = row.AverageAssists,
			AverageDeaths = row.AverageDeaths,
			AverageAirshots = row.AverageAirshots,
			AverageDamageTakenPm = row.AverageDamageTakenPm,
			AverageHealsReceivedPm = row.AverageHealsReceivedPm,
			AverageMedKitsHp = row.AverageMedKitsHp,
			AverageCapturePointsCaptured = row.AverageCapturePointsCaptured,
			TopKills = row.TopKills,
			TopKillsGameId = row.TopKillsGameId,
			TopDamage = row.TopDamage,
			TopDamageGameId = row.TopDamageGameId,
			TopAirshots = row.TopAirshots,
			TopAirshotsGameId = row.TopAirshotsGameId,
		};

	public static DemomanStatDomain ToDomain(this DemomanAllTime row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			Avatar = row.Avatar,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			WinPercentage = row.WinPercentage,
			Draws = row.Draws,
			Losses = row.Losses,
			AverageDpm = row.AverageDpm,
			AverageKills = row.AverageKills,
			AverageAssists = row.AverageAssists,
			AverageDeaths = row.AverageDeaths,
			AverageAirshots = row.AverageAirshots,
			AverageDamageTakenPm = row.AverageDamageTakenPm,
			AverageHealsReceivedPm = row.AverageHealsReceivedPm,
			AverageMedKitsHp = row.AverageMedKitsHp,
			AverageCapturePointsCaptured = row.AverageCapturePointsCaptured,
			TopKills = row.TopKills,
			TopKillsGameId = row.TopKillsGameId,
			TopDamage = row.TopDamage,
			TopDamageGameId = row.TopDamageGameId,
			TopAirshots = row.TopAirshots,
			TopAirshotsGameId = row.TopAirshotsGameId,
		};

	public static MedicStatDomain ToDomain(this MedicRecent row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			Avatar = row.Avatar,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			WinPercentage = row.WinPercentage,
			Draws = row.Draws,
			Losses = row.Losses,
			AverageKills = row.AverageKills,
			AverageAssists = row.AverageAssists,
			AverageDeaths = row.AverageDeaths,
			AverageUbers = row.AverageUbers,
			AverageDrops = row.AverageDrops,
			AverageHealsPm = row.AverageHealsPm,
			TopHeals = row.TopHeals,
			TopHealsGameId = row.TopHealsGameId,
			TopUbers = row.TopUbers,
			TopUbersGameId = row.TopUbersGameId,
			TopDrops = row.TopDrops,
			TopDropsGameId = row.TopDropsGameId,
		};

	public static MedicStatDomain ToDomain(this MedicAllTime row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			Avatar = row.Avatar,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			WinPercentage = row.WinPercentage,
			Draws = row.Draws,
			Losses = row.Losses,
			AverageKills = row.AverageKills,
			AverageAssists = row.AverageAssists,
			AverageDeaths = row.AverageDeaths,
			AverageUbers = row.AverageUbers,
			AverageDrops = row.AverageDrops,
			AverageHealsPm = row.AverageHealsPm,
			TopHeals = row.TopHeals,
			TopHealsGameId = row.TopHealsGameId,
			TopUbers = row.TopUbers,
			TopUbersGameId = row.TopUbersGameId,
			TopDrops = row.TopDrops,
			TopDropsGameId = row.TopDropsGameId,
		};

	public static OverallStatDomain ToDomain(this OverallStatsRecent row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			Avatar = row.Avatar,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			WinPercentage = row.WinPercentage,
			Draws = row.Draws,
			Losses = row.Losses,
			AverageDpm = row.AverageDpm,
			AverageKills = row.AverageKills,
			AverageAssists = row.AverageAssists,
			AverageDeaths = row.AverageDeaths,
			AverageAirshots = row.AverageAirshots,
			AverageHeadshots = row.AverageHeadshots,
			AverageBackstabs = row.AverageBackstabs,
			AverageDamageTakenPm = row.AverageDamageTakenPm,
			AverageHealsReceivedPm = row.AverageHealsReceivedPm,
			AverageMedKitsHp = row.AverageMedKitsHp,
			AverageCapturePointsCaptured = row.AverageCapturePointsCaptured,
			TopKills = row.TopKills,
			TopKillsGameId = row.TopKillsGameId,
			TopDamage = row.TopDamage,
			TopDamageGameId = row.TopDamageGameId,
			TopAirshots = row.TopAirshots,
			TopAirshotsGameId = row.TopAirshotsGameId,
		};

	public static OverallStatDomain ToDomain(this OverallStatsAllTime row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			Avatar = row.Avatar,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			WinPercentage = row.WinPercentage,
			Draws = row.Draws,
			Losses = row.Losses,
			AverageDpm = row.AverageDpm,
			AverageKills = row.AverageKills,
			AverageAssists = row.AverageAssists,
			AverageDeaths = row.AverageDeaths,
			AverageAirshots = row.AverageAirshots,
			AverageHeadshots = row.AverageHeadshots,
			AverageBackstabs = row.AverageBackstabs,
			AverageDamageTakenPm = row.AverageDamageTakenPm,
			AverageHealsReceivedPm = row.AverageHealsReceivedPm,
			AverageMedKitsHp = row.AverageMedKitsHp,
			AverageCapturePointsCaptured = row.AverageCapturePointsCaptured,
			TopKills = row.TopKills,
			TopKillsGameId = row.TopKillsGameId,
			TopDamage = row.TopDamage,
			TopDamageGameId = row.TopDamageGameId,
			TopAirshots = row.TopAirshots,
			TopAirshotsGameId = row.TopAirshotsGameId,
		};

	public static PlayerSummaryDomain ToDomain(this PlayerSummaryRow row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			Avatar = row.Avatar,
			Description = row.Description,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			Draws = row.Draws,
			Losses = row.Losses,
			WinPercentage = row.WinPercentage,
			AverageDpm = row.AverageDpm,
			AverageKills = row.AverageKills,
			AverageDeaths = row.AverageDeaths,
			AverageAssists = row.AverageAssists,
			AverageAirshots = row.AverageAirshots,
		};

	public static PlayerProgressionPointDomain ToDomain(
		this PlayerProgressionRow row
	) =>
		new()
		{
			Bucket = row.Bucket,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			Draws = row.Draws,
			Losses = row.Losses,
			WinPercentage = row.WinPercentage,
			AverageDpm = row.AverageDpm,
			AverageKills = row.AverageKills,
			AverageDeaths = row.AverageDeaths,
			AverageAssists = row.AverageAssists,
			AverageAirshots = row.AverageAirshots,
		};

	public static PlayerClassSplitDomain ToDomain(
		this PlayerClassSplitRow row
	) =>
		new()
		{
			ClassId = row.ClassId,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			AverageDpm = row.AverageDpm,
			AverageKills = row.AverageKills,
			AverageAssists = row.AverageAssists,
			AverageDeaths = row.AverageDeaths,
		};

	public static PlayerMapSplitDomain ToDomain(this PlayerMapSplitRow row) =>
		new()
		{
			Map = row.Map,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			WinPercentage = row.WinPercentage,
			AverageDpm = row.AverageDpm,
			AverageKills = row.AverageKills,
			AverageDeaths = row.AverageDeaths,
		};

	public static PlayerWeaponSplitDomain ToDomain(
		this PlayerWeaponSplitRow row
	) =>
		new()
		{
			WeaponName = row.WeaponName,
			NumberOfGames = row.NumberOfGames,
			TotalKills = row.TotalKills,
			TotalDamage = row.TotalDamage,
			Accuracy = row.Accuracy,
		};

	public static PlayerSynergyDomain ToDomain(this PlayerSynergyRow row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			Avatar = row.Avatar,
			NumberOfGames = row.NumberOfGames,
			Wins = row.Wins,
			WinPercentage = row.WinPercentage,
		};

	public static MedicDepthDomain ToDomain(this MedicDepthRow row) =>
		new()
		{
			NumberOfGames = row.NumberOfGames,
			AverageAdvantagesLost = row.AverageAdvantagesLost,
			AverageBiggestAdvantageLost = row.AverageBiggestAdvantageLost,
			AverageUberLength = row.AverageUberLength,
			AverageTimeToBuild = row.AverageTimeToBuild,
			AverageTimeBeforeUsing = row.AverageTimeBeforeUsing,
			AverageDeathsWithin20sAfterUber =
				row.AverageDeathsWithin20sAfterUber,
		};
}
