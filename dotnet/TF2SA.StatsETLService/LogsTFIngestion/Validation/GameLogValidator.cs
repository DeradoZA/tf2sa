using FluentValidation;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Validation;

public class GameLogValidator : AbstractValidator<GameLog>
{
	public const uint MIN_GAME_LENGTH = 10 * 60;
	public const int MIN_PLAYERS = 10;
	public const int MAX_PLAYERS = 15;
	public const int MAX_DAMAGE = 15;
	public const int MAX_HEALS_PER_MIN = 1400;

	public GameLogValidator()
	{
		RuleFor(g => g.Length).GreaterThanOrEqualTo(MIN_GAME_LENGTH);
		RuleFor(g => g.Players.Count).LessThanOrEqualTo(MAX_PLAYERS);
	}
}
