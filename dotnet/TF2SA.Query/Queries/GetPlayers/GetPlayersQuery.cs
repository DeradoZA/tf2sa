using MediatR;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;

namespace TF2SA.Query.Queries.GetPlayers;

public class GetPlayersQuery : IRequest<EitherStrict<Error, List<Player>>>
{
	// currently empty, but would put query params etc
	// anything necessary to execute the query
}
