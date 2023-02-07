using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Http.Base.Client;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.Steam.Config;
using TF2SA.Http.Steam.Models.PlayerSummaries;
using System.Collections.Specialized;
using TF2SA.Common.Configuration;

namespace TF2SA.Http.Steam.Service;

public class SteamService : ISteamService
{
	private readonly SteamConfig steamConfig;
	private readonly ILogger<SteamService> logger;
	private readonly IHttpClient httpClient;
	private const int MAX_PLAYERS_PER_REQUEST = 100;

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

	public async Task<EitherStrict<Error, List<SteamPlayer>>> GetPlayers(
		ulong[] steamids,
		CancellationToken cancellationToken
	)
	{
		if (steamids.Length < 1)
		{
			return new SteamError("No players provided to fetch.");
		}

		List<ulong[]> playerBatches = GroupPlayerBatchesForRequest(steamids);

		List<SteamPlayer> fetchedPlayers = new();
		List<Error> errors = new();
		await Parallel.ForEachAsync(
			playerBatches,
			new ParallelOptions()
			{
				MaxDegreeOfParallelism = Constants.MAX_CONCURRENT_HTTP_THREADS,
				CancellationToken = cancellationToken
			},
			async (playerBatch, token) =>
			{
				EitherStrict<Error, List<SteamPlayer>> playersBatchResult =
					await GetPlayersSet(playerBatch, token);
				if (playersBatchResult.IsLeft)
				{
					errors.Add(playersBatchResult.Left);
					return;
				}

				fetchedPlayers.AddRange(playersBatchResult.Right);
			}
		);

		if (errors.Any())
		{
			return errors[0];
		}

		return fetchedPlayers;
	}

	private static List<ulong[]> GroupPlayerBatchesForRequest(ulong[] steamids)
	{
		List<ulong[]> playerBatches = new();
		int batchesToMake = (int)
			Math.Ceiling(steamids.Length / (double)MAX_PLAYERS_PER_REQUEST);
		for (int b = 0; b < batchesToMake; b++)
		{
			ulong[] batch = steamids
				.Skip(MAX_PLAYERS_PER_REQUEST * b)
				.Take(MAX_PLAYERS_PER_REQUEST)
				.ToArray();
			playerBatches.Add(batch);
		}
		return playerBatches;
	}

	private async Task<EitherStrict<Error, List<SteamPlayer>>> GetPlayersSet(
		ulong[] steamids,
		CancellationToken cancellationToken
	)
	{
		string url =
			$"{steamConfig.BaseUrl}/ISteamUser/GetPlayerSummaries/v0002/";

		EitherStrict<Error, string> queryStringResult =
			BuildSteamPlayersQueryString(steamids);
		if (queryStringResult.IsLeft)
		{
			return queryStringResult.Left;
		}
		string queryString = queryStringResult.Right;
		url += $"?{queryString}";

		EitherStrict<HttpError, SteamPlayerResponseRoot> playerResponse =
			await httpClient.Get<SteamPlayerResponseRoot>(
				url,
				cancellationToken
			);
		if (playerResponse.IsLeft)
		{
			return playerResponse.Left;
		}

		List<SteamPlayer>? players = playerResponse.Right.Response?.Players;
		if (players is null)
		{
			return new SteamError("No players returned in response");
		}
		return players;
	}

	private EitherStrict<Error, string> BuildSteamPlayersQueryString(
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

		NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
		string ids = string.Join(",", steamids);

		query["key"] = steamConfig.ApiKey;
		query["steamids"] = ids;

		string? queryString = query.ToString();
		if (queryString is null)
		{
			return new SteamError("Null query string created");
		}

		return queryString;
	}
}
