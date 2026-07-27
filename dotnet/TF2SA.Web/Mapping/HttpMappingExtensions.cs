using TF2SA.Common.Models.Core;
using TF2SA.Query.Queries.GetPlayerDetail;
using TF2SA.Query.Queries.GetPlayers;
using TF2SA.Query.Queries.GetPlayersComparison;
using TF2SA.Query.Queries.Statistics.Demoman;
using TF2SA.Query.Queries.Statistics.Medic;
using TF2SA.Query.Queries.Statistics.Overall;
using TF2SA.Query.Queries.Statistics.Scout;
using TF2SA.Query.Queries.Statistics.Soldier;
using TF2SA.Web.Controllers.V1.Players.Models;
using TF2SA.Web.Controllers.V1.Statistics.Demoman;
using TF2SA.Web.Controllers.V1.Statistics.Medic;
using TF2SA.Web.Controllers.V1.Statistics.Overall;
using TF2SA.Web.Controllers.V1.Statistics.Scout;
using TF2SA.Web.Controllers.V1.Statistics.Soldier;

namespace TF2SA.Web.Mapping;

/// <summary>
/// Hand-written Domain/Result → HttpResult mapping, replacing AutoMapper's flat
/// CreateMap calls (former HttpMappingProfile). Every pair here is a same-shape
/// field-for-field copy except SteamId (ulong → string, since a 64-bit SteamID
/// exceeds JS's safe integer range) — nothing here does any real transformation.
/// </summary>
public static class HttpMappingExtensions
{
	public static PlayerHttpResult ToHttpResult(this Player player) =>
		new()
		{
			SteamId = player.SteamId.ToString(),
			PlayerName = player.PlayerName,
			LocalCountryCode = player.LocalCountryCode,
			ProfileUrl = player.ProfileUrl,
			Avatar = player.Avatar,
			AvatarMedium = player.AvatarMedium,
			AvatarFull = player.AvatarFull,
			AvatarHash = player.AvatarHash,
			RealName = player.RealName,
		};

	public static GetPlayersHttpResult ToHttpResult(
		this GetPlayersResult result
	) =>
		new()
		{
			TotalResults = result.TotalResults,
			Count = result.Count,
			Offset = result.Offset,
			Sort = result.Sort,
			SortOrder = result.SortOrder,
			FilterString = result.FilterString,
			Players = result.Players?.Select(p => p.ToHttpResult()),
		};

	public static PlayerSummaryHttpResult ToHttpResult(
		this PlayerSummaryDomain summary
	) =>
		new()
		{
			SteamId = summary.SteamId.ToString(),
			PlayerName = summary.PlayerName,
			Avatar = summary.Avatar,
			Description = summary.Description,
			NumberOfGames = summary.NumberOfGames,
			Wins = summary.Wins,
			Draws = summary.Draws,
			Losses = summary.Losses,
			WinPercentage = summary.WinPercentage,
			AverageDpm = summary.AverageDpm,
			AverageKills = summary.AverageKills,
			AverageDeaths = summary.AverageDeaths,
			AverageAssists = summary.AverageAssists,
			AverageAirshots = summary.AverageAirshots,
		};

	public static PlayerProgressionPointHttpResult ToHttpResult(
		this PlayerProgressionPointDomain point
	) =>
		new()
		{
			Bucket = point.Bucket,
			NumberOfGames = point.NumberOfGames,
			Wins = point.Wins,
			Draws = point.Draws,
			Losses = point.Losses,
			WinPercentage = point.WinPercentage,
			AverageDpm = point.AverageDpm,
			AverageKills = point.AverageKills,
			AverageDeaths = point.AverageDeaths,
			AverageAssists = point.AverageAssists,
			AverageAirshots = point.AverageAirshots,
		};

	public static PlayerClassSplitHttpResult ToHttpResult(
		this PlayerClassSplitDomain split
	) =>
		new()
		{
			ClassId = split.ClassId,
			NumberOfGames = split.NumberOfGames,
			Wins = split.Wins,
			AverageDpm = split.AverageDpm,
			AverageKills = split.AverageKills,
			AverageAssists = split.AverageAssists,
			AverageDeaths = split.AverageDeaths,
		};

	public static PlayerMapSplitHttpResult ToHttpResult(
		this PlayerMapSplitDomain split
	) =>
		new()
		{
			Map = split.Map,
			NumberOfGames = split.NumberOfGames,
			Wins = split.Wins,
			WinPercentage = split.WinPercentage,
			AverageDpm = split.AverageDpm,
			AverageKills = split.AverageKills,
			AverageDeaths = split.AverageDeaths,
		};

	public static PlayerWeaponSplitHttpResult ToHttpResult(
		this PlayerWeaponSplitDomain split
	) =>
		new()
		{
			WeaponName = split.WeaponName,
			NumberOfGames = split.NumberOfGames,
			TotalKills = split.TotalKills,
			TotalDamage = split.TotalDamage,
			Accuracy = split.Accuracy,
		};

	public static PlayerSynergyHttpResult ToHttpResult(
		this PlayerSynergyDomain synergy
	) =>
		new()
		{
			SteamId = synergy.SteamId.ToString(),
			PlayerName = synergy.PlayerName,
			Avatar = synergy.Avatar,
			NumberOfGames = synergy.NumberOfGames,
			Wins = synergy.Wins,
			WinPercentage = synergy.WinPercentage,
		};

	public static MedicDepthHttpResult ToHttpResult(
		this MedicDepthDomain depth
	) =>
		new()
		{
			NumberOfGames = depth.NumberOfGames,
			AverageAdvantagesLost = depth.AverageAdvantagesLost,
			AverageBiggestAdvantageLost = depth.AverageBiggestAdvantageLost,
			AverageUberLength = depth.AverageUberLength,
			AverageTimeToBuild = depth.AverageTimeToBuild,
			AverageTimeBeforeUsing = depth.AverageTimeBeforeUsing,
			AverageDeathsWithin20sAfterUber =
				depth.AverageDeathsWithin20sAfterUber,
		};

	public static GetPlayerDetailHttpResult ToHttpResult(
		this GetPlayerDetailResult result
	) =>
		new()
		{
			Summary = result.Summary?.ToHttpResult(),
			Bucket = result.Bucket,
			ClassId = result.ClassId,
			Progression = result.Progression.Select(p => p.ToHttpResult()),
			ClassSplits = result.ClassSplits.Select(c => c.ToHttpResult()),
			MapSplits = result.MapSplits.Select(m => m.ToHttpResult()),
			WeaponSplits = result.WeaponSplits.Select(w => w.ToHttpResult()),
			TopTeammates = result.TopTeammates.Select(s => s.ToHttpResult()),
			ToughestOpponents = result.ToughestOpponents.Select(s =>
				s.ToHttpResult()
			),
			MedicDepth = result.MedicDepth?.ToHttpResult(),
		};

	public static PlayerComparisonHttpResult ToHttpResult(
		this PlayerComparisonDomain comparison
	) =>
		new()
		{
			SteamId = comparison.SteamId.ToString(),
			PlayerName = comparison.PlayerName,
			Avatar = comparison.Avatar,
			NumberOfGames = comparison.NumberOfGames,
			Progression = comparison.Progression.Select(p => p.ToHttpResult()),
		};

	public static GetPlayersComparisonHttpResult ToHttpResult(
		this GetPlayersComparisonResult result
	) =>
		new()
		{
			Bucket = result.Bucket,
			ClassId = result.ClassId,
			Period = result.Period,
			Players = result.Players.Select(p => p.ToHttpResult()),
		};

	public static ScoutStatHttpResult ToHttpResult(this ScoutStatDomain stat) =>
		new()
		{
			SteamId = stat.SteamId.ToString(),
			PlayerName = stat.PlayerName,
			Avatar = stat.Avatar,
			NumberOfGames = stat.NumberOfGames,
			Wins = stat.Wins,
			WinPercentage = stat.WinPercentage,
			Draws = stat.Draws,
			Losses = stat.Losses,
			AverageDpm = stat.AverageDpm,
			AverageKills = stat.AverageKills,
			AverageAssists = stat.AverageAssists,
			AverageDeaths = stat.AverageDeaths,
			AverageDamageTakenPm = stat.AverageDamageTakenPm,
			AverageHealsReceivedPm = stat.AverageHealsReceivedPm,
			AverageMedKitsHp = stat.AverageMedKitsHp,
			AverageCapturePointsCaptured = stat.AverageCapturePointsCaptured,
			TopKills = stat.TopKills,
			TopKillsGameId = stat.TopKillsGameId,
			TopDamage = stat.TopDamage,
			TopDamageGameId = stat.TopDamageGameId,
		};

	public static GetScoutStatsHttpResult ToHttpResult(
		this GetScoutStatsResult result
	) =>
		new()
		{
			TotalResults = result.TotalResults,
			Count = result.Count,
			Offset = result.Offset,
			Sort = result.Sort,
			SortOrder = result.SortOrder,
			FilterField = result.FilterField,
			FilterValue = result.FilterValue,
			Players = result.Players?.Select(p => p.ToHttpResult()),
		};

	public static SoldierStatHttpResult ToHttpResult(
		this SoldierStatDomain stat
	) =>
		new()
		{
			SteamId = stat.SteamId.ToString(),
			PlayerName = stat.PlayerName,
			Avatar = stat.Avatar,
			NumberOfGames = stat.NumberOfGames,
			Wins = stat.Wins,
			WinPercentage = stat.WinPercentage,
			Draws = stat.Draws,
			Losses = stat.Losses,
			AverageDpm = stat.AverageDpm,
			AverageKills = stat.AverageKills,
			AverageAssists = stat.AverageAssists,
			AverageDeaths = stat.AverageDeaths,
			AverageAirshots = stat.AverageAirshots,
			AverageDamageTakenPm = stat.AverageDamageTakenPm,
			AverageHealsReceivedPm = stat.AverageHealsReceivedPm,
			AverageMedKitsHp = stat.AverageMedKitsHp,
			AverageCapturePointsCaptured = stat.AverageCapturePointsCaptured,
			TopKills = stat.TopKills,
			TopKillsGameId = stat.TopKillsGameId,
			TopDamage = stat.TopDamage,
			TopDamageGameId = stat.TopDamageGameId,
			TopAirshots = stat.TopAirshots,
			TopAirshotsGameId = stat.TopAirshotsGameId,
		};

	public static GetSoldierStatsHttpResult ToHttpResult(
		this GetSoldierStatsResult result
	) =>
		new()
		{
			TotalResults = result.TotalResults,
			Count = result.Count,
			Offset = result.Offset,
			Sort = result.Sort,
			SortOrder = result.SortOrder,
			FilterField = result.FilterField,
			FilterValue = result.FilterValue,
			Players = result.Players?.Select(p => p.ToHttpResult()),
		};

	public static DemomanStatHttpResult ToHttpResult(
		this DemomanStatDomain stat
	) =>
		new()
		{
			SteamId = stat.SteamId.ToString(),
			PlayerName = stat.PlayerName,
			Avatar = stat.Avatar,
			NumberOfGames = stat.NumberOfGames,
			Wins = stat.Wins,
			WinPercentage = stat.WinPercentage,
			Draws = stat.Draws,
			Losses = stat.Losses,
			AverageDpm = stat.AverageDpm,
			AverageKills = stat.AverageKills,
			AverageAssists = stat.AverageAssists,
			AverageDeaths = stat.AverageDeaths,
			AverageAirshots = stat.AverageAirshots,
			AverageDamageTakenPm = stat.AverageDamageTakenPm,
			AverageHealsReceivedPm = stat.AverageHealsReceivedPm,
			AverageMedKitsHp = stat.AverageMedKitsHp,
			AverageCapturePointsCaptured = stat.AverageCapturePointsCaptured,
			TopKills = stat.TopKills,
			TopKillsGameId = stat.TopKillsGameId,
			TopDamage = stat.TopDamage,
			TopDamageGameId = stat.TopDamageGameId,
			TopAirshots = stat.TopAirshots,
			TopAirshotsGameId = stat.TopAirshotsGameId,
		};

	public static GetDemomanStatsHttpResult ToHttpResult(
		this GetDemomanStatsResult result
	) =>
		new()
		{
			TotalResults = result.TotalResults,
			Count = result.Count,
			Offset = result.Offset,
			Sort = result.Sort,
			SortOrder = result.SortOrder,
			FilterField = result.FilterField,
			FilterValue = result.FilterValue,
			Players = result.Players?.Select(p => p.ToHttpResult()),
		};

	public static MedicStatHttpResult ToHttpResult(this MedicStatDomain stat) =>
		new()
		{
			SteamId = stat.SteamId.ToString(),
			PlayerName = stat.PlayerName,
			Avatar = stat.Avatar,
			NumberOfGames = stat.NumberOfGames,
			Wins = stat.Wins,
			WinPercentage = stat.WinPercentage,
			Draws = stat.Draws,
			Losses = stat.Losses,
			AverageKills = stat.AverageKills,
			AverageAssists = stat.AverageAssists,
			AverageDeaths = stat.AverageDeaths,
			AverageUbers = stat.AverageUbers,
			AverageDrops = stat.AverageDrops,
			AverageHealsPm = stat.AverageHealsPm,
			TopHeals = stat.TopHeals,
			TopHealsGameId = stat.TopHealsGameId,
			TopUbers = stat.TopUbers,
			TopUbersGameId = stat.TopUbersGameId,
			TopDrops = stat.TopDrops,
			TopDropsGameId = stat.TopDropsGameId,
		};

	public static GetMedicStatsHttpResult ToHttpResult(
		this GetMedicStatsResult result
	) =>
		new()
		{
			TotalResults = result.TotalResults,
			Count = result.Count,
			Offset = result.Offset,
			Sort = result.Sort,
			SortOrder = result.SortOrder,
			FilterField = result.FilterField,
			FilterValue = result.FilterValue,
			Players = result.Players?.Select(p => p.ToHttpResult()),
		};

	public static OverallStatHttpResult ToHttpResult(
		this OverallStatDomain stat
	) =>
		new()
		{
			SteamId = stat.SteamId.ToString(),
			PlayerName = stat.PlayerName,
			Avatar = stat.Avatar,
			NumberOfGames = stat.NumberOfGames,
			Wins = stat.Wins,
			WinPercentage = stat.WinPercentage,
			Draws = stat.Draws,
			Losses = stat.Losses,
			AverageDpm = stat.AverageDpm,
			AverageKills = stat.AverageKills,
			AverageAssists = stat.AverageAssists,
			AverageDeaths = stat.AverageDeaths,
			AverageAirshots = stat.AverageAirshots,
			AverageHeadshots = stat.AverageHeadshots,
			AverageBackstabs = stat.AverageBackstabs,
			AverageDamageTakenPm = stat.AverageDamageTakenPm,
			AverageHealsReceivedPm = stat.AverageHealsReceivedPm,
			AverageMedKitsHp = stat.AverageMedKitsHp,
			AverageCapturePointsCaptured = stat.AverageCapturePointsCaptured,
			TopKills = stat.TopKills,
			TopKillsGameId = stat.TopKillsGameId,
			TopDamage = stat.TopDamage,
			TopDamageGameId = stat.TopDamageGameId,
			TopAirshots = stat.TopAirshots,
			TopAirshotsGameId = stat.TopAirshotsGameId,
		};

	public static GetOverallStatsHttpResult ToHttpResult(
		this GetOverallStatsResult result
	) =>
		new()
		{
			TotalResults = result.TotalResults,
			Count = result.Count,
			Offset = result.Offset,
			Sort = result.Sort,
			SortOrder = result.SortOrder,
			FilterField = result.FilterField,
			FilterValue = result.FilterValue,
			Players = result.Players?.Select(p => p.ToHttpResult()),
		};
}
