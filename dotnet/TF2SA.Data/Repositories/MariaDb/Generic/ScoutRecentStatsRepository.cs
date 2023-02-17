using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using TF2SA.Data.Entities.MariaDb;
using static TF2SA.Data.Extensions.TF2SALinqExtensions;

namespace TF2SA.Data.Repositories.MariaDb.Generic;

public class ScoutRecentStatsRepository : StatsRepository<ScoutRecent>
{
	public override string UpdateProcQuery => "CALL UpdateScoutRecentStats();";

	public override Dictionary<
		string,
		Expression<Func<ScoutRecent, object>>
	> PropertyKeySelectors =>
		new()
		{
			{ "steamId", s => s.SteamId },
			{ "playerName", s => s.PlayerName! },
			{ "avatar", s => s.Avatar! },
			{ "numberOfGames", s => s.NumberOfGames! },
			{ "averageDpm", s => s.AverageDpm! },
			{ "averageKills", s => s.AverageKills! },
			{ "averageAssists", s => s.AverageAssists! },
			{ "averageDeaths", s => s.AverageDeaths! },
			{ "topKills", s => s.TopKills! },
			{ "topKillsGameId", s => s.TopKillsGameId! },
			{ "topDamage", s => s.TopDamage! },
			{ "topDamageGameId", s => s.TopDamageGameId! },
		};

	public override Tuple<
		string,
		Expression<Func<ScoutRecent, object>>
	> DefaultSortField => new("averageDpm", s => s.AverageDpm!);

	public override SortOrder DefaultSortOrder => SortOrder.desc;

	public ScoutRecentStatsRepository(
		TF2SADbContext dbContext,
		ILogger<ScoutRecentStatsRepository> logger
	) : base(dbContext, logger) { }
}
