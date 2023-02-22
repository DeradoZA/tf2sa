using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;
using TF2SA.Query.Queries.Statistics.Medic.GetMedicAllTime;

namespace TF2SA.Query.Queries.Statistics.Medic.GetMedicRecent;

public class GetMedicRecentQueryHandler
	: IRequestHandler<
		GetMedicAllTimeQuery,
		EitherStrict<Error, GetMedicStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<MedicRecent> repository;

	public GetMedicRecentQueryHandler(
		IMapper mapper,
		IStatsRepository<MedicRecent> repository
	)
	{
		this.mapper = mapper;
		this.repository = repository;
	}

	public async Task<EitherStrict<Error, GetMedicStatsResult>> Handle(
		GetMedicAllTimeQuery request,
		CancellationToken cancellationToken
	)
	{
		try
		{
			IQueryable<MedicRecent> allQueryable = repository.GetAllQueryable();

			IQueryable<MedicRecent> filteredQueryable = repository.ApplyFilter(
				allQueryable,
				request.FilterField,
				request.FilterValue,
				out string filterFieldUsed,
				out string filterValueUsed
			);

			int totalCount = await filteredQueryable.CountAsync(
				cancellationToken: cancellationToken
			);

			IOrderedQueryable<MedicRecent> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<MedicRecent> entities = await filteredSortedQueryable
				.Skip(request.Offset)
				.Take(request.Count)
				.ToListAsync(cancellationToken: cancellationToken);

			IEnumerable<MedicStatDomain> domainMappedPlayers = mapper.Map<
				List<MedicStatDomain>
			>(entities);

			return new GetMedicStatsResult(
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
