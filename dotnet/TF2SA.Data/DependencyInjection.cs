using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TF2SA.Data.Analytics;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Logging;
using TF2SA.Data.Mapping;
using TF2SA.Data.Repositories.Base;
using TF2SA.Data.Repositories.MariaDb;
using TF2SA.Data.Repositories.MariaDb.Statistics;
using TF2SA.Data.Services.Base;
using TF2SA.Data.Services.Mariadb;

namespace TF2SA.Data;

public static class DependencyInjection
{
	public static void AddDataLayer(
		this IServiceCollection services,
		IConfiguration configuration
	)
	{
		string connectionString =
			$"server={configuration["TF2SA_MYSQL_HOST"]};user={configuration["TF2SA_MYSQL_USR"]};password={configuration["TF2SA_MYSQL_PWD"]};database={configuration["TF2SA_MYSQL_DB"]}";
		MySqlServerVersion serverVersion = new(new Version(10, 6, 0));

		// Debug:LogDatabaseQueries defaults to false (appsettings.json) and true
		// locally (appsettings.Development.json): logs SQL, parameter values, and
		// affected row counts to the console. Off in production.
		bool logDatabaseQueries = configuration.GetValue<bool>(
			"Debug:LogDatabaseQueries"
		);
		services.AddDbContext<TF2SADbContext>(dbContextOptions =>
		{
			dbContextOptions
				.UseMySql(connectionString, serverVersion)
				.LogTo(
					Console.WriteLine,
					logDatabaseQueries ? LogLevel.Information : LogLevel.Warning
				)
				.EnableSensitiveDataLogging(logDatabaseQueries);
			if (logDatabaseQueries)
			{
				dbContextOptions.AddInterceptors(
					new SaveChangesResultLoggingInterceptor()
				);
			}
		});

		services.AddScoped<
			IPlayersRepository<Player, ulong>,
			PlayersRepository
		>();
		services.AddScoped<
			IPlayerStatsRepository<PlayerStat, uint>,
			PlayerStatsRepository
		>();
		services.AddScoped<
			IClassStatsRepository<ClassStat, uint>,
			ClassStatsRepository
		>();
		services.AddScoped<IGamesRepository<Game, uint>, GamesRepository>();
		services.AddScoped<IStatsService<ulong>, StatsService>();

		services.AddScoped<
			IStatsRepository<ScoutRecent>,
			ScoutRecentStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<ScoutAllTime>,
			ScoutAllTimeStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<SoldierRecent>,
			SoldierRecentStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<SoldierAllTime>,
			SoldierAllTimeStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<DemomanRecent>,
			DemomanRecentStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<DemomanAllTime>,
			DemomanAllTimeStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<MedicRecent>,
			MedicRecentStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<MedicAllTime>,
			MedicAllTimeStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<OverallStatsRecent>,
			OverallRecentStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<OverallStatsAllTime>,
			OverallAllTimeStatsRepository
		>();

		services.AddAutoMapper(typeof(GameMappingProfile));

		// DuckDB analytics read layer (ADR 0008). Refreshed alongside the stored
		// procs during the Phase 1 parallel run; enabled via TF2SA_ANALYTICS_ENABLED.
		services.AddSingleton<IAnalyticsStore, DuckDbAnalyticsStore>();
	}
}
