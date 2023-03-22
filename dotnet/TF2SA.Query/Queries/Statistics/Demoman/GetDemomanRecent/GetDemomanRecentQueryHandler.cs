using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Query.Queries.Statistics.Demoman.GetDemomanRecent;

public class GetDemomanRecentQueryHandler
	: IRequestHandler<
		GetDemomanRecentQuery,
		EitherStrict<Error, GetDemomanStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<DemomanRecentEntity> repository;

	public GetDemomanRecentQueryHandler(
		IMapper mapper,
		IStatsRepository<DemomanRecentEntity> repository
	)
	{
		this.mapper = mapper;
		this.repository = repository;
	}

	public async Task<EitherStrict<Error, GetDemomanStatsResult>> Handle(
		GetDemomanRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			IQueryable<DemomanRecentEntity> allQueryable =
				repository.GetAllQueryable();

			IQueryable<DemomanRecentEntity> filteredQueryable =
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

			IOrderedQueryable<DemomanRecentEntity> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<DemomanRecentEntity> entities = await filteredSortedQueryable
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
