using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TF2SA.Http.LogsTF.Client;

namespace TF2SA.Http;

public static class DependencyInjection
{
    public static void AddHttpServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddTransient<ILogsTFHttpClient, LogsTFHttpClient>();
    }
}
