using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Query.Queries.Statistics.Overall.GetOverallAllTime;

public class GetOverallAllTimeQueryHandler
	: IRequestHandler<
		GetOverallAllTimeQuery,
		EitherStrict<Error, GetOverallStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<OverallStatsAllTime> repository;

	public GetOverallAllTimeQueryHandler(
		IMapper mapper,
		IStatsRepository<OverallStatsAllTime> repository
	)
	{
		this.mapper = mapper;
		this.repository = repository;
	}

	public async Task<EitherStrict<Error, GetOverallStatsResult>> Handle(
		GetOverallAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			IQueryable<OverallStatsAllTime> allQueryable =
				repository.GetAllQueryable();

			IQueryable<OverallStatsAllTime> filteredQueryable =
				repository.ApplyFilter(
					allQueryable,
					request.FilterField,
					request.FilterValue,
					out string filterFieldUsed,
					out string filterValueUsed
				);

			int totalCount = await filteredQueryable.CountAsync(
				cancellationToken: cancellationToken
			);

			IOrderedQueryable<OverallStatsAllTime> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<OverallStatsAllTime> entities = await filteredSortedQueryable
				.Skip(request.Offset)
				.Take(request.Count)
				.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<OverallStatDomain> domainMappedPlayers = mapper.Map<
				List<OverallStatDomain>
			>(entities);

			return new GetOverallStatsResult(
				totalCount,
				domainMappedPlayers.Count(),
				request.Offset,
				sortFieldUsed,
				sortOrderUsed,
				filterFieldUsed,
				filterValueUsed,
				domainMappedPlayers
			);
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
