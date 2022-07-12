using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TF2SA.Data.Constants;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Repositories.Base;
using TF2SA.Data.Repositories.MariaDb;
using TF2SA.Data.Services.Base;
using TF2SA.Data.Services.Mariadb;

namespace TF2SA.Data
{
    public static class DependencyInjection
    {
        public static void AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString =
                $"server={configuration["TF2SA_MYSQL_HOST"]};user={configuration["TF2SA_MYSQL_USR"]};password={configuration["TF2SA_MYSQL_PWD"]};database={configuration["TF2SA_MYSQL_DB"]}";
            var serverVersion = new MySqlServerVersion(new Version(10, 6, 0));
            services.AddDbContext<TF2SADbContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(connectionString, serverVersion)
                    // The following three options help with debugging, but should
                    // be changed or removed for production.
                    .LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );

            services.AddScoped<IPlayersRepository<Player, ulong>, PlayersRepository>();
            services.AddScoped<IPlayerStatsRepository<PlayerStat, uint>, PlayerStatsRepository>();
            services.AddScoped<IClassStatsRepository<ClassStat, uint>, ClassStatsRepository>();
            services.AddScoped<IGamesRepository<Game, uint>, GamesRepository>();
            services.AddScoped<IStatsService, StatsService>();
        }
    }
}