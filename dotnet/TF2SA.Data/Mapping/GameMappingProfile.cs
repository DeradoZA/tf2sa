using AutoMapper;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Data.Entities.MariaDb;
using PlayerEntity = TF2SA.Data.Entities.MariaDb.Player;
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
		CreateMap<GameLog, Game>()
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
		CreateMap<LogInfo, Game>();
		CreateMap<PlayerStats, PlayerStat>().IncludeMembers(ps => ps.Player);
		CreateMap<PlayerDto, PlayerStat>()
			.ForMember(
				ps => ps.SteamId,
				opt => opt.MapFrom(p => p.SteamId!.ConvertToUInt64())
			);
		CreateMap<PlayerStat, PlayerEntity>();
		CreateMap<ClassStats, ClassStat>();
		CreateMap<WeaponStats, WeaponStat>();
		CreateMap<SteamPlayer, PlayerEntity>();
		CreateMap<PlayerEntity, PlayerDomain>();
		CreateMap<ScoutRecent, ScoutStatDomain>();
		CreateMap<ScoutAllTime, ScoutStatDomain>();
		CreateMap<SoldierRecent, SoldierStatDomain>();
		CreateMap<SoldierAllTime, SoldierStatDomain>();
		CreateMap<DemomanRecent, DemomanStatDomain>();
		CreateMap<DemomanAllTime, DemomanStatDomain>();
		CreateMap<MedicRecent, MedicStatDomain>();
		CreateMap<MedicAllTime, MedicStatDomain>();
		CreateMap<OverallStatsRecent, OverallStatDomain>();
		CreateMap<OverallStatsAllTime, OverallStatDomain>();
	}
}
