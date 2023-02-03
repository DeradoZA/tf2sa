using Monad;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.Steam.Config.Models.PlayerSummaries;

namespace TF2SA.Http.Steam.Service;

public interface ISteamService
{
	public Task<EitherStrict<HttpError, SteamPlayersResponse>> GetPlayers(
		ulong[] steamids
	);
}
