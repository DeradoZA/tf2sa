using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Analytics;
using TF2SA.Data.Analytics.Rows;
using TF2SA.Data.Errors;

namespace TF2SA.Query.Queries.GetPlayers;

public class GetPlayersQueryHandler
{
	private readonly IAnalyticsStore analyticsStore;

	public GetPlayersQueryHandler(IAnalyticsStore analyticsStore)
	{
		this.analyticsStore = analyticsStore;
	}

	public async Task<EitherStrict<Error, GetPlayersResult>> Handle(
		GetPlayersQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			StatsPage<PlayerRow> page =
				await analyticsStore.GetPlayersPageAsync(
					request.Sort,
					request.SortOrder,
					request.FilterString,
					request.Offset,
					request.Count,
					cancellationToken
				);

			List<Player> players = page.Rows.Select(ToPlayer).ToList();

			return new GetPlayersResult
			{
				TotalResults = page.Total,
				Players = players,
				Offset = request.Offset,
				Count = players.Count,
				Sort = page.SortUsed,
				SortOrder = page.SortOrderUsed,
				FilterString = page.FilterValueUsed,
			};
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}

	private static Player ToPlayer(PlayerRow row) =>
		new()
		{
			SteamId = row.SteamId,
			PlayerName = row.PlayerName,
			LocalCountryCode = row.LocalCountryCode,
			ProfileUrl = row.ProfileUrl,
			Avatar = row.Avatar,
			AvatarMedium = row.AvatarMedium,
			AvatarFull = row.AvatarFull,
			AvatarHash = row.AvatarHash,
			RealName = row.RealName,
		};
}
