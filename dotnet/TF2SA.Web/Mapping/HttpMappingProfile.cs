using AutoMapper;
using TF2SA.Common.Models.Core;
using TF2SA.Query.Queries.GetPlayers;
using TF2SA.Query.Queries.Statistics.Demoman;
using TF2SA.Query.Queries.Statistics.Medic;
using TF2SA.Query.Queries.Statistics.Overall;
using TF2SA.Query.Queries.Statistics.Scout;
using TF2SA.Query.Queries.Statistics.Soldier;
using TF2SA.Web.Controllers.V1.Players.Models;
using TF2SA.Web.Controllers.V1.Statistics.Demoman;
using TF2SA.Web.Controllers.V1.Statistics.Medic;
using TF2SA.Web.Controllers.V1.Statistics.Overall;
using TF2SA.Web.Controllers.V1.Statistics.Scout;
using TF2SA.Web.Controllers.V1.Statistics.Soldier;

namespace TF2SA.Web.Mapping;

public class HttpMappingProfile : Profile
{
	public HttpMappingProfile()
	{
		CreateMap<GetPlayersResult, GetPlayersHttpResult>();
		CreateMap<Player, PlayerHttpResult>();

		CreateMap<GetScoutStatsResult, GetScoutStatsHttpResult>();
		CreateMap<ScoutStatDomain, ScoutStatHttpResult>();

		CreateMap<GetSoldierStatsResult, GetSoldierStatsHttpResult>();
		CreateMap<SoldierStatDomain, SoldierStatHttpResult>();

		CreateMap<GetDemomanStatsResult, GetDemomanStatsHttpResult>();
		CreateMap<DemomanStatDomain, DemomanStatHttpResult>();

		CreateMap<GetMedicStatsResult, GetMedicStatsHttpResult>();
		CreateMap<MedicStatDomain, MedicStatHttpResult>();

		CreateMap<GetOverallStatsResult, GetOverallStatsHttpResult>();
		CreateMap<OverallStatDomain, OverallStatHttpResult>();
	}
}
