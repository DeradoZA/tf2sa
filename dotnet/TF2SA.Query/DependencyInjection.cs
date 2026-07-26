using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TF2SA.Data;
using TF2SA.Query.Queries.GetPlayerDetail;
using TF2SA.Query.Queries.GetPlayers;
using TF2SA.Query.Queries.GetPlayersComparison;
using TF2SA.Query.Queries.Statistics.Demoman.GetDemomanAllTime;
using TF2SA.Query.Queries.Statistics.Demoman.GetDemomanRecent;
using TF2SA.Query.Queries.Statistics.Medic.GetMedicAllTime;
using TF2SA.Query.Queries.Statistics.Medic.GetMedicRecent;
using TF2SA.Query.Queries.Statistics.Overall.GetOverallAllTime;
using TF2SA.Query.Queries.Statistics.Overall.GetOverallRecent;
using TF2SA.Query.Queries.Statistics.Scout.GetScoutAllTime;
using TF2SA.Query.Queries.Statistics.Scout.GetScoutRecent;
using TF2SA.Query.Queries.Statistics.Soldier.GetSoldierAllTime;
using TF2SA.Query.Queries.Statistics.Soldier.GetSoldierRecent;

namespace TF2SA.Query;

public static class DependencyInjection
{
	public static void AddQueries(
		this IServiceCollection services,
		IConfiguration configuration
	)
	{
		services.AddDataLayer(configuration);

		services.AddScoped<GetPlayersQueryHandler>();
		services.AddScoped<GetPlayerDetailQueryHandler>();
		services.AddScoped<GetPlayersComparisonQueryHandler>();
		services.AddScoped<GetScoutRecentQueryHandler>();
		services.AddScoped<GetScoutAllTimeQueryHandler>();
		services.AddScoped<GetSoldierRecentQueryHandler>();
		services.AddScoped<GetSoldierAllTimeQueryHandler>();
		services.AddScoped<GetDemomanRecentQueryHandler>();
		services.AddScoped<GetDemomanAllTimeQueryHandler>();
		services.AddScoped<GetMedicRecentQueryHandler>();
		services.AddScoped<GetMedicAllTimeQueryHandler>();
		services.AddScoped<GetOverallRecentQueryHandler>();
		services.AddScoped<GetOverallAllTimeQueryHandler>();
	}
}
