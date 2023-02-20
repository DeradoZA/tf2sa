using AutoMapper;
using TF2SA.Common.Models.Core;
using TF2SA.Query.Queries.GetPlayers;
using TF2SA.Query.Queries.Statistics.Scout;
using TF2SA.Web.Controllers.V1.Players.Models;
using TF2SA.Web.Controllers.V1.Statistics.Scout;

namespace TF2SA.Web.Mapping;

public class HttpMappingProfile : Profile
{
	public HttpMappingProfile()
	{
		CreateMap<GetPlayersResult, GetPlayersHttpResult>();
		CreateMap<Player, PlayerHttpResult>();

		CreateMap<GetScoutStatsResult, GetScoutStatsHttpResult>();
		CreateMap<ScoutStatDomain, ScoutStatHttpResult>();
	}
}
