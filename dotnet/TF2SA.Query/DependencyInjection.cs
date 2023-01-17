using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace TF2SA.Query;

public static class DependencyInjection
{
	public static void AddQueries(
		this IServiceCollection services,
		IConfiguration configuration
	)
	{
		services.AddMediatR(Assembly.GetExecutingAssembly());
	}
}
