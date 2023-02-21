using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Query.Queries.Statistics.Soldier.GetSoldierAllTime;

public class GetSoldierAllTimeQueryHandler
	: IRequestHandler<
		GetSoldierAllTimeQuery,
		EitherStrict<Error, GetSoldierStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<SoldierAllTime> repository;

	public GetSoldierAllTimeQueryHandler(
		IMapper mapper,
		IStatsRepository<SoldierAllTime> repository
	)
	{
		this.mapper = mapper;
		this.repository = repository;
	}

	public async Task<EitherStrict<Error, GetSoldierStatsResult>> Handle(
		GetSoldierAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			IQueryable<SoldierAllTime> allQueryable =
				repository.GetAllQueryable();

			IQueryable<SoldierAllTime> filteredQueryable =
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

			IOrderedQueryable<SoldierAllTime> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<SoldierAllTime> entities = await filteredSortedQueryable
				.Skip(request.Offset)
				.Take(request.Count)
				.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<SoldierStatDomain> domainMappedPlayers = mapper.Map<
				List<SoldierStatDomain>
			>(entities);

			return new GetSoldierStatsResult(
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
