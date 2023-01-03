using System.Collections.Generic;
using System.Linq;
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

	[Fact]
	public void GivenInvalidDamage_ValidationFails_WithExpectedError()
	{
		PlayerStats playerStats = fixture
			.Build<PlayerStats>()
			.With(p => p.Damage, 18000)
			.With(p => p.Heals, 0)
			.Create();
		string expectedError =
			"'Damage' must be less than or equal to '17500'.";

		ValidationResult result = validator.Validate(playerStats);
		IEnumerable<string> returnedErrors = result.Errors.Select(
			re => re.ErrorMessage
		);

		Assert.False(result.IsValid);
		Assert.Contains(expectedError, returnedErrors);
	}

	[Fact]
	public void GivenValidDamage_ValidationPasses()
	{
		PlayerStats playerStats = fixture
			.Build<PlayerStats>()
			.With(p => p.TeamId, 0)
			.With(p => p.Team, "Blue")
			.With(p => p.Damage, 6000)
			.With(p => p.Heals, 0)
			.With(
				p => p.ClassStats,
				fixture
					.Build<ClassStats>()
					.With(c => c.ClassId, (byte)1)
					.With(c => c.Type, "scout")
					.CreateMany(1)
					.ToList()
			)
			.Create();

		ValidationResult result = validator.Validate(playerStats);

		Assert.True(result.IsValid);
	}

	[Fact]
	public void GivenInvalidHealsPerMinute_ValidationFails_WithExpectedError()
	{
		PlayerStats playerStats = GenerateMedicClassStat(2000, 60);
		string expectedError =
			"Player played medic with abnormally high heals per minute";

		ValidationResult result = validator.Validate(playerStats);
		IEnumerable<string> returnedErrors = result.Errors.Select(
			re => re.ErrorMessage
		);

		Assert.False(result.IsValid);
		Assert.Contains(expectedError, returnedErrors);
	}

	[Fact]
	public void InvalidHealsPerMinute_ValidationPassses()
	{
		PlayerStats playerStats = GenerateMedicClassStat(1000, 60);

		ValidationResult result = validator.Validate(playerStats);

		Assert.True(result.IsValid);
	}

	private PlayerStats GenerateMedicClassStat(int heals, int medicPlayTime) =>
		fixture
			.Build<PlayerStats>()
			.With(p => p.Damage, 600)
			.With(p => p.Heals, heals)
			.With(p => p.Team, "Red")
			.With(
				p => p.ClassStats,
				fixture
					.Build<ClassStats>()
					.With(c => c.Playtime, medicPlayTime)
					.With(c => c.Type, "medic")
					.CreateMany(1)
					.ToList()
			)
			.Create();
}
