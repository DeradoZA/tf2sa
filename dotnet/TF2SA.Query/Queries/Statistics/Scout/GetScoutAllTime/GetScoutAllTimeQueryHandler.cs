using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Query.Queries.Statistics.Scout.GetScoutAllTime;

public class GetScoutAllTimeQueryHandler
	: IRequestHandler<
		GetScoutAllTimeQuery,
		EitherStrict<Error, GetScoutStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<ScoutAllTimeEntity> repository;

	public GetScoutAllTimeQueryHandler(
		IMapper mapper,
		IStatsRepository<ScoutAllTimeEntity> repository
	)
	{
		this.mapper = mapper;
		this.repository = repository;
	}

	public async Task<EitherStrict<Error, GetScoutStatsResult>> Handle(
		GetScoutAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			IQueryable<ScoutAllTimeEntity> allQueryable =
				repository.GetAllQueryable();

			IQueryable<ScoutAllTimeEntity> filteredQueryable =
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

			IOrderedQueryable<ScoutAllTimeEntity> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<ScoutAllTimeEntity> scoutStatEntites =
				await filteredSortedQueryable
					.Skip(request.Offset)
					.Take(request.Count)
					.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<ScoutStatDomain> domainMappedPlayers = mapper.Map<
				List<ScoutStatDomain>
			>(scoutStatEntites);

			return new GetScoutStatsResult(
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
