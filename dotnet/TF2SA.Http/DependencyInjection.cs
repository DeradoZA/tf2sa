using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TF2SA.Http.LogsTF.Client;
using TF2SA.Http.LogsTF.Config;

namespace TF2SA.Http;

public static class DependencyInjection
{
	public static void AddHttpServices(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.Configure<LogsTFConfig>(
			configuration.GetSection(LogsTFConfig.LogsTFConfigSection));
		services.AddHttpClient();
		services.AddTransient<ILogsTFService, LogsTFHttpClient>();
	}
}
