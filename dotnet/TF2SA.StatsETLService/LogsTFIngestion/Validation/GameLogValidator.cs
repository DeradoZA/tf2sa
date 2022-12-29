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
		RuleFor(g => g.Length).NotNull().GreaterThanOrEqualTo(MIN_GAME_LENGTH);
		RuleFor(g => g.Players)
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
		RuleForEach(g => g.Players).SetValidator(new PlayerStatsValidator());
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
