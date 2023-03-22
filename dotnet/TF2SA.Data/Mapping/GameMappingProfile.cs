using AutoMapper;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Data.Entities.MariaDb;
using PlayerDto = TF2SA.Common.Models.LogsTF.GameLogModel.Player;
using PlayerDomain = TF2SA.Common.Models.Core.Player;
using TF2SA.Common.Models.LogsTF.Constants;
using TF2SA.Http.Steam.Models.PlayerSummaries;
using TF2SA.Common.Models.Core;

namespace TF2SA.Data.Mapping;

public class GameMappingProfile : Profile
{
	public GameMappingProfile()
	{
		CreateMap<GameLog, GameEntity>()
			.ForMember(
				g => g.RedScore,
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
		CreateMap<LogInfo, GameEntity>();
		CreateMap<PlayerStats, PlayerStatEntity>()
			.IncludeMembers(ps => ps.Player);
		CreateMap<PlayerDto, PlayerStatEntity>()
			.ForMember(
				ps => ps.SteamId,
				opt => opt.MapFrom(p => p.SteamId!.ConvertToUInt64())
			);
		CreateMap<PlayerStatEntity, PlayerEntity>();
		CreateMap<ClassStats, ClassStatEntity>();
		CreateMap<WeaponStats, WeaponStatEntity>();
		CreateMap<SteamPlayer, PlayerEntity>();
		CreateMap<PlayerEntity, PlayerDomain>();
		CreateMap<ScoutRecentEntity, ScoutStatDomain>();
		CreateMap<ScoutAllTimeEntity, ScoutStatDomain>();
		CreateMap<SoldierRecentEntity, SoldierStatDomain>();
		CreateMap<SoldierAllTimeEntity, SoldierStatDomain>();
		CreateMap<DemomanRecentEntity, DemomanStatDomain>();
		CreateMap<DemomanAllTimeEntity, DemomanStatDomain>();
		CreateMap<MedicRecentEntity, MedicStatDomain>();
		CreateMap<MedicAllTimeEntity, MedicStatDomain>();
		CreateMap<OverallStatsRecentEntity, OverallStatDomain>();
		CreateMap<OverallStatsAllTimeEntity, OverallStatDomain>();
	}
}
