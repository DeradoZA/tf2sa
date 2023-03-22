using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Query.Queries.Statistics.Demoman.GetDemomanAllTime;

public class GetDemomanAllTimeQueryHandler
	: IRequestHandler<
		GetDemomanAllTimeQuery,
		EitherStrict<Error, GetDemomanStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<DemomanAllTimeEntity> repository;

	public GetDemomanAllTimeQueryHandler(
		IMapper mapper,
		IStatsRepository<DemomanAllTimeEntity> repository
	)
	{
		this.mapper = mapper;
		this.repository = repository;
	}

	public async Task<EitherStrict<Error, GetDemomanStatsResult>> Handle(
		GetDemomanAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			IQueryable<DemomanAllTimeEntity> allQueryable =
				repository.GetAllQueryable();

			IQueryable<DemomanAllTimeEntity> filteredQueryable =
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

			IOrderedQueryable<DemomanAllTimeEntity> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<DemomanAllTimeEntity> entities = await filteredSortedQueryable
				.Skip(request.Offset)
				.Take(request.Count)
				.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<DemomanStatDomain> domainMappedPlayers = mapper.Map<
				List<DemomanStatDomain>
			>(entities);

			return new GetDemomanStatsResult(
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
