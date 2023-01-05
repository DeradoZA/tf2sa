using FluentValidation;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Validation;

public class GameLogValidator : AbstractValidator<GameLog>
{
	public const uint MIN_GAME_LENGTH = 10 * 60;
	public const int MIN_PLAYERS = 10;
	public const int MAX_PLAYERS = 16;

	public GameLogValidator()
	{
		RuleFor(g => g.Duration)
			.NotNull()
			.GreaterThanOrEqualTo(MIN_GAME_LENGTH);
		RuleFor(g => g.PlayerStats)
			.NotNull()
			.Must(
				(players) =>
					players?.Count >= MIN_PLAYERS
					&& players.Count <= MAX_PLAYERS
			)
			.WithMessage(
				$"Game is likely not a 6v6 game. "
					+ $"Must contain {MIN_PLAYERS}-{MAX_PLAYERS} players."
			);
		RuleForEach(g => g.PlayerStats)
			.SetValidator(new PlayerStatsValidator())
			.Must(
				(log, playersStat) =>
				{
					bool? exists = log?.Names?.Exists(
						n => n?.SteamId == playersStat?.Player?.SteamId
					);
					if (exists is null)
					{
						return false;
					}
					return (bool)exists;
				}
			)
			.WithMessage(
				"No corresponding PlayerStats steamId matching Names list"
			);
		RuleFor(g => g.Info)
			.NotNull()
			.NotEmpty()
			.Must(
				(log, info) =>
				{
					if (info is null)
					{
						return false;
					}
					return new LogInfoValidator().Validate(info).IsValid;
				}
			)
			.WithMessage(
				"Required LogInfo properties/or the LogInfo object are null."
			);
	}
}
