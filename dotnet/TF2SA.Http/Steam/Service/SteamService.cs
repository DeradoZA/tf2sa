using System.Web;
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
		ulong[] steamids,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	private Task<EitherStrict<Error, List<SteamPlayer>>> GetPlayersSet(
		ulong[] steamids,
		CancellationToken cancellationToken
	)
	{
		string url =
			$"{steamConfig.BaseUrl}/ISteamUser/GetPlayerSummaries/v0002/";

		EitherStrict<Error, string?> queryStringResult =
			BuildSteamPlayersQueryString(steamids);
		if (queryStringResult.IsLeft)
		{
			return queryStringResult.Left;
		}
		string? queryString = queryStringResult.Right;
	}

	private EitherStrict<Error, string?> BuildSteamPlayersQueryString(
		ulong[] steamids
	)
	{
		if (string.IsNullOrEmpty(steamConfig.ApiKey))
		{
			return new SteamError("API Key required for query string");
		}

		if (steamids.Length == 0)
		{
			return new SteamError("No steamids supplied.");
		}

		if (steamids.Length > 100)
		{
			return new SteamError("Can only process 100 steamids at a time.");
		}

		var query = HttpUtility.ParseQueryString(string.Empty);
		string ids = string.Join(",", steamids);

		query["key"] = steamConfig.ApiKey;
		query["steamids"] = ids;

		if (query is null)
		{
			return new SteamError("failed to build query string");
		}

		return query.ToString();
	}
}
