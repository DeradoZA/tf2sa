using FluentValidation;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Validation;

public class WeaponStatsValidator : AbstractValidator<WeaponStats>
{
	public WeaponStatsValidator()
	{
		RuleFor(w => w.WeaponName).NotNull().NotEmpty();
		RuleFor(w => w.Kills).NotNull();
	}
}
