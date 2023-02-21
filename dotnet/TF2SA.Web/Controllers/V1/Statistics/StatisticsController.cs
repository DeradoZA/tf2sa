using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Query.Queries.Statistics.Scout;
using TF2SA.Query.Queries.Statistics.Scout.GetScoutRecent;
using TF2SA.Web.Controllers.V1.Statistics.Scout;

namespace TF2SA.Web.Controllers.V1.Statistics;

[ApiController]
[Route("v1/[controller]")]
public partial class StatisticsController : ControllerBase
{
	private readonly IMediator mediator;
	private readonly IMapper mapper;

	public StatisticsController(IMediator mediator, IMapper mapper)
	{
		this.mediator = mediator;
		this.mapper = mapper;
	}
}
