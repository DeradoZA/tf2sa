using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TF2SA.Data.Analytics;

namespace TF2SA.Data;

public static class DependencyInjection
{
	public static void AddDataLayer(
		this IServiceCollection services,
		IConfiguration configuration
	)
	{
		// DuckDB store (ADR 0008, ADR 0009) — the only database.
		services.AddSingleton<IAnalyticsStore, DuckDbAnalyticsStore>();
	}
}
