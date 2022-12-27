using FluentValidation;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Validation;

public class PlayerStatsValidator : AbstractValidator<PlayerStats>
{
	public PlayerStatsValidator()
	{
		RuleFor(p => p.ClassStats).NotNull().NotEmpty();
		RuleForEach(p => p.ClassStats).SetValidator(new ClassStatsValidator());
	}
}
