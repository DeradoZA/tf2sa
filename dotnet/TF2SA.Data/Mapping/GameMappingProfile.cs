using AutoMapper;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Data.Entities.MariaDb;

namespace TF2SA.Data.Mapping;

public class GameMappingProfile : Profile
{
	public GameMappingProfile()
	{
		CreateMap<GameLog, Game>()
			.ForMember(
				g => g.RedScore,
				opt =>
					opt.MapFrom(
						gl =>
							gl.Teams
								.Single(
									t =>
										string.Equals(
											"red",
											t.TeamId,
											StringComparison.InvariantCultureIgnoreCase
										)
								)
								.Score
					)
			)
			.ForMember(
				g => g.BlueScore,
				opt =>
					opt.MapFrom(
						gl =>
							gl.Teams
								.Single(
									t =>
										string.Equals(
											"blue",
											t.TeamId,
											StringComparison.InvariantCultureIgnoreCase
										)
								)
								.Score
					)
			)
			.IncludeMembers(gl => gl.Info);
		CreateMap<LogInfo, Game>();
	}
}
