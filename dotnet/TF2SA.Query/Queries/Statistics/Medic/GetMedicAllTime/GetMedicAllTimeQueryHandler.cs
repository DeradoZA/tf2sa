using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.Core;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;

namespace TF2SA.Query.Queries.Statistics.Medic.GetMedicAllTime;

public class GetMedicAllTimeQueryHandler
	: IRequestHandler<
		GetMedicAllTimeQuery,
		EitherStrict<Error, GetMedicStatsResult>
	>
{
	private readonly IMapper mapper;
	private readonly IStatsRepository<MedicAllTimeEntity> repository;

	public GetMedicAllTimeQueryHandler(
		IMapper mapper,
		IStatsRepository<MedicAllTimeEntity> repository
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
			IQueryable<MedicAllTimeEntity> allQueryable =
				repository.GetAllQueryable();

			IQueryable<MedicAllTimeEntity> filteredQueryable =
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

			IOrderedQueryable<MedicAllTimeEntity> filteredSortedQueryable =
				repository.ApplySort(
					filteredQueryable,
					request.Sort,
					request.SortOrder,
					out string sortFieldUsed,
					out string sortOrderUsed
				);

			List<MedicAllTimeEntity> entities = await filteredSortedQueryable
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
