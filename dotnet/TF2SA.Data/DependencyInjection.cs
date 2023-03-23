using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Mapping;
using TF2SA.Data.Repositories.Base;
using TF2SA.Data.Repositories.MariaDb;
using TF2SA.Data.Repositories.MariaDb.Statistics;

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
		services.AddDbContext<TF2SADbContext>(
			dbContextOptions =>
				dbContextOptions
					.UseMySql(connectionString, serverVersion)
					// The following three options help with debugging, but should
					// be changed or removed for production.
					.LogTo(Console.WriteLine, LogLevel.Information)
					.EnableSensitiveDataLogging()
					.EnableDetailedErrors()
		);

		services.AddScoped<
			IPlayersRepository<PlayersEntity, ulong>,
			PlayersRepository
		>();
		services.AddScoped<
			IPlayerStatsRepository<PlayerStatsEntity, uint>,
			PlayerStatsRepository
		>();
		services.AddScoped<
			IClassStatsRepository<ClassStatsEntity, uint>,
			ClassStatsRepository
		>();
		services.AddScoped<
			IGamesRepository<GamesEntity, uint>,
			GamesRepository
		>();

		services.AddScoped<
			IStatsRepository<ScoutRecentEntity>,
			ScoutRecentStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<ScoutAllTimeEntity>,
			ScoutAllTimeStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<SoldierRecentEntity>,
			SoldierRecentStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<SoldierAllTimeEntity>,
			SoldierAllTimeStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<DemomanRecentEntity>,
			DemomanRecentStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<DemomanAllTimeEntity>,
			DemomanAllTimeStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<MedicRecentEntity>,
			MedicRecentStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<MedicAllTimeEntity>,
			MedicAllTimeStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<OverallStatsRecentEntity>,
			OverallRecentStatsRepository
		>();
		services.AddScoped<
			IStatsRepository<OverallStatsAllTimeEntity>,
			OverallAllTimeStatsRepository
		>();

		services.AddAutoMapper(typeof(GameMappingProfile));
	}
}
