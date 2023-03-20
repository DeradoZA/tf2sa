using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using TF2SA.Data.Entities.MariaDb;
using static TF2SA.Data.Extensions.TF2SALinqExtensions;

namespace TF2SA.Data.Repositories.MariaDb.Statistics;

public class SoldierAllTimeStatsRepository
	: StatsRepository<SoldierAllTimeEntity>
{
	public override string UpdateProcQuery =>
		"CALL UpdateSoldierAllTimeStats();";

	public override Dictionary<
		string,
		Expression<Func<SoldierAllTimeEntity, object>>
	> PropertyKeySelectors =>
		new()
		{
			{ "steamId", s => s.SteamId },
			{ "playerName", s => s.PlayerName! },
			{ "avatar", s => s.Avatar! },
			{ "numberOfGames", s => s.NumberOfGames! },
			{ "wins", s => s.Wins! },
			{ "winPercentage", s => s.WinPercentage! },
			{ "draws", s => s.Draws! },
			{ "losses", s => s.Losses! },
			{ "averageDpm", s => s.AverageDpm! },
			{ "averageKills", s => s.AverageKills! },
			{ "averageAssists", s => s.AverageAssists! },
			{ "averageDeaths", s => s.AverageDeaths! },
			{ "averageAirshots", s => s.AverageAirshots! },
			{ "averageDamageTakenPm", s => s.AverageDamageTakenPm! },
			{ "averageHealsReceivedPm", s => s.AverageHealsReceivedPm! },
			{ "averageMedKitsHp", s => s.AverageMedKitsHp! },
			{
				"averageCapturePointsCaptured",
				s => s.AverageCapturePointsCaptured!
			},
			{ "topKills", s => s.TopKills! },
			{ "topKillsGameId", s => s.TopKillsGameId! },
			{ "topDamage", s => s.TopDamage! },
			{ "topDamageGameId", s => s.TopDamageGameId! },
			{ "topAirshots", s => s.TopAirshots! },
			{ "topAirshotsGameId", s => s.TopAirshotsGameId! },
		};

	public override Tuple<
		string,
		Expression<Func<SoldierAllTimeEntity, object>>
	> DefaultSortField => new("averageDpm", s => s.AverageDpm!);

	public override SortOrder DefaultSortOrder => SortOrder.desc;

	public SoldierAllTimeStatsRepository(
		TF2SADbContext dbContext,
		ILogger<SoldierAllTimeStatsRepository> logger
	)
		: base(dbContext, logger) { }
}
