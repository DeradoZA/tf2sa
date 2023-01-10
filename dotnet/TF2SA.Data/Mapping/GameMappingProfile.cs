using AutoMapper;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Data.Entities.MariaDb;
using PlayerEntity = TF2SA.Data.Entities.MariaDb.Player;
using PlayerDto = TF2SA.Common.Models.LogsTF.GameLogModel.Player;
using TF2SA.Common.Models.LogsTF.Constants;

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
							gl!.Teams!
								// TODO use enum parse to fetch team
								// milestone: 7
								.Single(
									t =>
										(TeamId)
											Enum.Parse(
												typeof(TeamId),
												t.TeamId!,
												true
											) == TeamId.Red
								)
								.Score
					)
			)
			.ForMember(
				g => g.BlueScore,
				opt =>
					opt.MapFrom(
						gl =>
							gl!.Teams!
								.Single(
									t =>
										(TeamId)
											Enum.Parse(
												typeof(TeamId),
												t.TeamId!,
												true
											) == TeamId.Blue
								)
								.Score
					)
			)
			.IncludeMembers(gl => gl.Info);
		CreateMap<LogInfo, Game>();
		CreateMap<PlayerStats, PlayerStat>().IncludeMembers(ps => ps.Player);
		CreateMap<PlayerDto, PlayerStat>()
			.ForMember(
				ps => ps.SteamId,
				// TODO validate that there is always a steamid to fetch
				// milestone: StatsETL
				opt => opt.MapFrom(p => p.SteamId!.ConvertToUInt64())
			);
		CreateMap<PlayerStat, PlayerEntity>();
		CreateMap<ClassStats, ClassStat>();
		// TODO: Validate WeaponStats
		// milestone: StatsETL
		CreateMap<WeaponStats, WeaponStat>();
	}
}
