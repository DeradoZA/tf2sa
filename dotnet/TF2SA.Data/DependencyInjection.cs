using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TF2SA.Data
{
    public static class DependencyInjection
    {
        public static void AddMariaDb(this IServiceCollection services, IConfiguration configuration)
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
        }
    }
}