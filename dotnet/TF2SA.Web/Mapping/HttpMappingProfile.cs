using AutoMapper;
using TF2SA.Common.Models.Core;
using TF2SA.Query.Queries.GetPlayers;
using TF2SA.Query.Queries.GetScoutRecent;
using TF2SA.Web.Controllers.V1.Players.Models;
using TF2SA.Web.Controllers.V1.Statistics.Models.GetScoutRecent;

namespace TF2SA.Web.Mapping;

public class HttpMappingProfile : Profile
{
	public HttpMappingProfile()
	{
		CreateMap<GetPlayersResult, GetPlayersHttpResult>();
		CreateMap<Player, PlayerHttpResult>();

		CreateMap<GetScoutRecentResult, GetScoutRecentHttpResult>();
		CreateMap<ScoutRecentDomain, ScoutRecentHttpResult>();
	}
}
