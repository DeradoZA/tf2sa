using FluentValidation;
using TF2SA.Common.Models.LogsTF.Constants;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Validation;

public class ClassStatsValidator : AbstractValidator<ClassStats>
{
	public ClassStatsValidator()
	{
		RuleFor(cs => cs.Type)
			.NotNull()
			.Must(t => Enum.TryParse(t, true, out ClassId classId))
			.WithMessage("Class type '{PropertyValue}' is invalid ");
		RuleFor(cs => cs.Kills).NotNull();
		RuleFor(cs => cs.Assists).NotNull();
		RuleFor(cs => cs.Deaths).NotNull();
		RuleFor(cs => cs.Damage).NotNull();
		RuleFor(cs => cs.Playtime).NotNull().GreaterThan(0);
	}
}
