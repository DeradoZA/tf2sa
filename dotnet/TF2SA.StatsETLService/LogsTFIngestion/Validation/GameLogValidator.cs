using FluentValidation;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Validation;

public class GameLogValidator : AbstractValidator<GameLog>
{
	public const uint MIN_GAME_LENGTH = 10 * 60;
	public const int MIN_PLAYERS = 10;
	public const int MAX_PLAYERS = 15;

	public GameLogValidator()
	{
		RuleFor(g => g.Length).NotNull().GreaterThanOrEqualTo(MIN_GAME_LENGTH);
		RuleFor(g => g.Players)
			.NotNull()
			.Must(
				(players) =>
					players?.Count > MIN_PLAYERS && players.Count < MAX_PLAYERS
			)
			.WithMessage("No players passed to game log");
		RuleForEach(g => g.Players).SetValidator(new PlayerStatsValidator());
	}
}
