using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentValidation.Results;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.StatsETLService.LogsTFIngestion.Validation;
using Xunit;

namespace TF2SA.Tests.Unit.StatsETLService.LogsTFIngestion.Validation;

public class PlayerStatsValidatorTests
{
	private readonly PlayerStatsValidator validator = new();
	private readonly Fixture fixture = new();

	[Fact]
	public void GivenNullRootFields_ValidationFails_WithExpectedErrors()
	{
		PlayerStats playerStats = new();
		List<string> expectedErrors =
			new()
			{
				"'Class Stats' must not be empty.",
				"'Damage' must not be empty.",
				"'Heals' must not be empty."
			};

		ValidationResult result = validator.Validate(playerStats);
		IEnumerable<string> returnedErrors = result.Errors.Select(
			re => re.ErrorMessage
		);

		Assert.False(result.IsValid);
		Assert.All(expectedErrors, e => Assert.Contains(e, returnedErrors));
	}
}
