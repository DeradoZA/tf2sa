using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.Statistics.Demoman;
using TF2SA.Query.Queries.Statistics.Demoman.GetDemomanAllTime;
using TF2SA.Query.Queries.Statistics.Demoman.GetDemomanRecent;
using TF2SA.Query.Queries.Statistics.Medic;
using TF2SA.Query.Queries.Statistics.Medic.GetMedicAllTime;
using TF2SA.Query.Queries.Statistics.Medic.GetMedicRecent;
using TF2SA.Query.Queries.Statistics.Overall;
using TF2SA.Query.Queries.Statistics.Overall.GetOverallAllTime;
using TF2SA.Query.Queries.Statistics.Overall.GetOverallRecent;
using TF2SA.Query.Queries.Statistics.Scout;
using TF2SA.Query.Queries.Statistics.Scout.GetScoutAllTime;
using TF2SA.Query.Queries.Statistics.Scout.GetScoutRecent;
using TF2SA.Query.Queries.Statistics.Soldier;
using TF2SA.Query.Queries.Statistics.Soldier.GetSoldierAllTime;
using TF2SA.Query.Queries.Statistics.Soldier.GetSoldierRecent;
using TF2SA.Web.Controllers.V1.Statistics.Demoman;
using TF2SA.Web.Controllers.V1.Statistics.Medic;
using TF2SA.Web.Controllers.V1.Statistics.Overall;
using TF2SA.Web.Controllers.V1.Statistics.Scout;
using TF2SA.Web.Controllers.V1.Statistics.Soldier;

namespace TF2SA.Web.Controllers.V1.Statistics;

[ApiController]
[Route("v1/[controller]")]
public partial class StatisticsController : ControllerBase
{
	private readonly GetScoutRecentQueryHandler getScoutRecentHandler;
	private readonly GetScoutAllTimeQueryHandler getScoutAllTimeHandler;
	private readonly GetSoldierRecentQueryHandler getSoldierRecentHandler;
	private readonly GetSoldierAllTimeQueryHandler getSoldierAllTimeHandler;
	private readonly GetDemomanRecentQueryHandler getDemomanRecentHandler;
	private readonly GetDemomanAllTimeQueryHandler getDemomanAllTimeHandler;
	private readonly GetMedicRecentQueryHandler getMedicRecentHandler;
	private readonly GetMedicAllTimeQueryHandler getMedicAllTimeHandler;
	private readonly GetOverallRecentQueryHandler getOverallRecentHandler;
	private readonly GetOverallAllTimeQueryHandler getOverallAllTimeHandler;
	private readonly IMapper mapper;

	public StatisticsController(
		GetScoutRecentQueryHandler getScoutRecentHandler,
		GetScoutAllTimeQueryHandler getScoutAllTimeHandler,
		GetSoldierRecentQueryHandler getSoldierRecentHandler,
		GetSoldierAllTimeQueryHandler getSoldierAllTimeHandler,
		GetDemomanRecentQueryHandler getDemomanRecentHandler,
		GetDemomanAllTimeQueryHandler getDemomanAllTimeHandler,
		GetMedicRecentQueryHandler getMedicRecentHandler,
		GetMedicAllTimeQueryHandler getMedicAllTimeHandler,
		GetOverallRecentQueryHandler getOverallRecentHandler,
		GetOverallAllTimeQueryHandler getOverallAllTimeHandler,
		IMapper mapper
	)
	{
		this.getScoutRecentHandler = getScoutRecentHandler;
		this.getScoutAllTimeHandler = getScoutAllTimeHandler;
		this.getSoldierRecentHandler = getSoldierRecentHandler;
		this.getSoldierAllTimeHandler = getSoldierAllTimeHandler;
		this.getDemomanRecentHandler = getDemomanRecentHandler;
		this.getDemomanAllTimeHandler = getDemomanAllTimeHandler;
		this.getMedicRecentHandler = getMedicRecentHandler;
		this.getMedicAllTimeHandler = getMedicAllTimeHandler;
		this.getOverallRecentHandler = getOverallRecentHandler;
		this.getOverallAllTimeHandler = getOverallAllTimeHandler;
		this.mapper = mapper;
	}
}
