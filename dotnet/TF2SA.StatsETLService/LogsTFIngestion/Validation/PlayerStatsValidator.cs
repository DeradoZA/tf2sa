using FluentValidation;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Validation;

public class PlayerStatsValidator : AbstractValidator<PlayerStats>
{
	public const int MAX_DAMAGE = 17500;
	public const int MAX_HEALS_PER_MINUTE = 1300;

	public PlayerStatsValidator()
	{
		RuleFor(p => p.ClassStats).NotNull().NotEmpty();
		RuleForEach(p => p.ClassStats).SetValidator(new ClassStatsValidator());

		RuleFor(p => p.Damage)
			.NotNull()
			.NotEmpty()
			.LessThanOrEqualTo(MAX_DAMAGE);

		RuleFor(p => p.Heals).NotNull().Must(HaveValidHealsPerMinute);
	}

	private readonly Func<PlayerStats, int?, bool> HaveValidHealsPerMinute = (
		p,
		heals
	) =>
	{
		ClassStats? medicStats = p?.ClassStats?.SingleOrDefault(
			c =>
				string.Equals(
					"medic",
					c.Type,
					StringComparison.InvariantCultureIgnoreCase
				)
		);
		if (
			medicStats is null
			|| medicStats.TotalTime is null
			|| medicStats.TotalTime == 0
		)
		{
			return true;
		}

		if ((heals / medicStats.TotalTime * 60) >= MAX_HEALS_PER_MINUTE)
		{
			return false;
		}

		return true;
	};
}
