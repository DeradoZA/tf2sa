using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.MariaDb.Generic;

namespace TF2SA.Query.Queries.GetScoutRecent;

public class GetScoutRecentQueryHandler
	: IRequestHandler<
		GetScoutRecentQuery,
		EitherStrict<Error, GetScoutRecentResult>
	>
{
	private readonly IMapper mapper;
	private readonly ILogger<GetScoutRecentQueryHandler> logger;
	private readonly IStatsRepository<ScoutRecent> repository;

	public GetScoutRecentQueryHandler(
		IMapper mapper,
		ILogger<GetScoutRecentQueryHandler> logger,
		IStatsRepository<ScoutRecent> repository
	)
	{
		this.mapper = mapper;
		this.logger = logger;
		this.repository = repository;
	}

	public async Task<EitherStrict<Error, GetScoutRecentResult>> Handle(
		GetScoutRecentQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			IQueryable<ScoutRecent> scoutRecentAllQueryable =
				repository.GetAllQueryable();

			IQueryable<ScoutRecent> filteredQueryable = repository.ApplyFilter(
				scoutRecentAllQueryable,
				request.FilterField,
				request.FilterValue,
				out string filterFieldUsed,
				out string filterValueUsed
			);

			int totalCount = await filteredQueryable.CountAsync(
				cancellationToken: cancellationToken
			);

			IOrderedQueryable<ScoutRecent> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<ScoutRecent> scoutRecentEntities =
				await filteredSortedQueryable
					.Skip(request.Offset)
					.Take(request.Count)
					.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<ScoutRecentDomain> scouts = mapper.Map<
				List<ScoutRecentDomain>
			>(scoutRecentEntities);

			return new GetScoutRecentResult(
				totalCount,
				scouts.Count(),
				request.Offset,
				sortFieldUsed,
				sortOrderUsed,
				filterFieldUsed,
				filterValueUsed,
				scouts
			);
		}
		catch (Exception e)
		{
			return new DatabaseError(e.Message);
		}
	}
}
