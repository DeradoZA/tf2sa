using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using TF2SA.Data.Entities.MariaDb;
using static TF2SA.Data.Extensions.TF2SALinqExtensions;

namespace TF2SA.Data.Repositories.MariaDb.Statistics;

public class MedicAllTimeStatsRepository : StatsRepository<MedicAllTime>
{
	public override string UpdateProcQuery => "CALL UpdateScoutAllTimeStats();";

	public override Dictionary<
		string,
		Expression<Func<MedicAllTime, object>>
	> PropertyKeySelectors =>
		new()
		{
			{ "steamId", s => s.SteamId },
			{ "playerName", s => s.PlayerName! },
			{ "avatar", s => s.Avatar! },
			{ "numberOfGames", s => s.NumberOfGames! },
			{ "wins", s => s.Wins! },
			{ "draws", s => s.Draws! },
			{ "losses", s => s.Losses! },
			{ "averageKills", s => s.AverageKills! },
			{ "averageAssists", s => s.AverageAssists! },
			{ "averageDeaths", s => s.AverageDeaths! },
			{ "averageUbers", s => s.AverageUbers! },
			{ "averageDrops", s => s.AverageDrops! },
			{ "averageHealsPm", s => s.AverageHealsPm! },
			{ "topHeals", s => s.TopHeals! },
			{ "topHealsGameId", s => s.TopHealsGameId! },
			{ "topUbers", s => s.TopUbers! },
			{ "topUbersGameId", s => s.TopUbersGameId! },
			{ "topDrops", s => s.TopDrops! },
			{ "topDropsGameId", s => s.TopDropsGameId! },
		};

	public override Tuple<
		string,
		Expression<Func<MedicAllTime, object>>
	> DefaultSortField => new("averageHealsPm", s => s.AverageHealsPm!);

	public override SortOrder DefaultSortOrder => SortOrder.desc;

	public MedicAllTimeStatsRepository(
		TF2SADbContext dbContext,
		ILogger<MedicAllTimeStatsRepository> logger
	) : base(dbContext, logger) { }
}
