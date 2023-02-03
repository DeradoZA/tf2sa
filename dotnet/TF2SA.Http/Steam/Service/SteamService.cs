using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Http.Base.Client;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.Steam.Config;
using TF2SA.Http.Steam.Config.Models.PlayerSummaries;

namespace TF2SA.Http.Steam.Service;

public class SteamService : ISteamService
{
	private readonly SteamConfig steamConfig;
	private readonly ILogger<SteamService> logger;
	private readonly IHttpClient httpClient;

	public SteamService(
		IOptions<SteamConfig> steamConfig,
		ILogger<SteamService> logger,
		IHttpClient httpClient
	)
	{
		this.steamConfig = steamConfig.Value;
		this.logger = logger;
		this.httpClient = httpClient;
		logger.LogInformation(
			"init key: {apiKey}, url: {baseUrl}",
			this.steamConfig.ApiKey,
			this.steamConfig.BaseUrl
		);
	}

	public Task<EitherStrict<Error, List<SteamPlayer>>> GetPlayers(
		ulong[] steamids
	)
	{
		throw new NotImplementedException();
	}

	private Task<EitherStrict<Error, List<SteamPlayer>>> GetPlayersSet(
		ulong[] steamids
	)
	{
		if (steamids.Length > 100)
		{
			return new SteamError("Can only process 100");
		}
	}
}
