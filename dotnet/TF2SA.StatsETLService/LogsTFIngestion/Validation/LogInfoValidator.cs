using FluentValidation;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.StatsETLService.LogsTFIngestion.Validation;

public class LogInfoValidator : AbstractValidator<LogInfo>
{
	public LogInfoValidator()
	{
		RuleFor(i => i.HasWeaponDamage).NotNull();
		RuleFor(i => i.HasAccuracy).NotNull();
		RuleFor(i => i.HasHP).NotNull();
		RuleFor(i => i.HasHeadshots).NotNull();
		RuleFor(i => i.HasHeadshotsHit).NotNull();
		RuleFor(i => i.HasBackstabs).NotNull();
		RuleFor(i => i.HasCapturePointsCaptured).NotNull();
		RuleFor(i => i.HasSentriesBuilt).NotNull();
		RuleFor(i => i.HasDamageTaken).NotNull();
		RuleFor(i => i.HasAirshots).NotNull();
		RuleFor(i => i.HasHealsReceived).NotNull();
		RuleFor(i => i.HasIntelCaptures).NotNull();
		RuleFor(i => i.Date).NotNull();
	}
}
