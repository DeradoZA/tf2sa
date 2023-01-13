using FluentValidation;
using TF2SA.Common.Models.LogsTF.Constants;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Validation;

public class PlayerStatsValidator : AbstractValidator<PlayerStats>
{
	public const int MAX_DAMAGE = 17500;
	public const int MAX_HEALS_PER_MINUTE = 1350;

	public PlayerStatsValidator()
	{
		RuleFor(p => p.ClassStats).NotNull();

		RuleFor(p => p.Team)
			.NotNull()
			.Must(t => Enum.TryParse(t, true, out TeamId teamId))
			.WithMessage(
				"TeamId type '{PropertyValue}' is invalid. Expecting 'Red' or 'Blue'"
			);

		RuleForEach(p => p.ClassStats).SetValidator(new ClassStatsValidator());

		RuleFor(p => p.LongestKillStreak).NotNull();
		RuleFor(p => p.MedKits).NotNull();

		RuleFor(p => p.Damage)
			.NotNull()
			.NotEmpty()
			.LessThanOrEqualTo(MAX_DAMAGE);

		RuleFor(p => p.Heals)
			.NotNull()
			.Must(HaveValidHealsPerMinute)
			.WithMessage(
				"Player played medic with abnormally high heals per minute"
			);
	}

	private readonly Func<PlayerStats, int?, bool> HaveValidHealsPerMinute = (
		p,
		heals
	) =>
	{
		ClassStats? medicStats = p?.ClassStats?.SingleOrDefault(
			c =>
				(ClassId)Enum.Parse(typeof(ClassId), c.Type!, true)
				== ClassId.medic
		);
		if (
			medicStats is null
			|| medicStats.Playtime is null
			|| medicStats.Playtime == 0
		)
		{
			return true;
		}

		if ((heals / medicStats.Playtime * 60) >= MAX_HEALS_PER_MINUTE)
		{
			return false;
		}

		return true;
	};
}
