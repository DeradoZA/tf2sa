using Monad;
using TF2SA.Common.Errors;
using TF2SA.Http.Steam.Models.PlayerSummaries;

namespace TF2SA.Http.Steam.Service;

public interface ISteamService
{
	public Task<EitherStrict<Error, List<SteamPlayer>>> GetPlayers(
		ulong[] steamids,
		CancellationToken cancellationToken
	);
}
