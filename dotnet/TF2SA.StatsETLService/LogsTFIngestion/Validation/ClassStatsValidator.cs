using FluentValidation;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Validation;

public class ClassStatsValidator : AbstractValidator<ClassStats>
{
	public ClassStatsValidator() { }
}
