using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TF2SA.Http.Base.Client;
using TF2SA.Http.Base.Serialization;
using TF2SA.Http.LogsTF.Config;
using TF2SA.Http.LogsTF.Service;

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
		services.AddTransient<IHttpClient, TF2SAHttpClient>();
		services.AddSingleton<IJsonSerializer, TF2SAJsonSerializer>();
		services.AddTransient<ILogsTFService, LogsTFService>();
	}
}
