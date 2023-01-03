using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentValidation.Results;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.StatsETLService.LogsTFIngestion.Validation;
using Xunit;

namespace TF2SA.Tests.Unit.StatsETLService.Validation;

public class GameLogValidatorTests
{
	private readonly GameLogValidator validator = new();
	private readonly Fixture fixture = new();

	[Fact]
	public void GivenNullRootFields_ValidationFails_WithExpectedErrors()
	{
		GameLog gamelog = new();
		List<string> expectedErrors =
			new()
			{
				"'Duration' must not be empty.",
				"'Player Stats' must not be empty."
			};

		ValidationResult result = validator.Validate(gamelog);
		IEnumerable<string> returnedErrors = result.Errors.Select(
			re => re.ErrorMessage
		);

		Assert.False(result.IsValid);
		Assert.All(expectedErrors, e => Assert.Contains(e, returnedErrors));
	}

	[Theory]
	[InlineData(8)]
	[InlineData(9)]
	[InlineData(17)]
	[InlineData(18)]
	public void GivenNon6v6AmountOfPlayers_ValidationFails_WithExpectedErrors(
		int playerCount
	)
	{
		GameLog gamelog = CreateValidGameLog(playerCount);
		string expectedError =
			"Game is likely not a 6v6 game. Must contain 10-16 players.";

		ValidationResult result = validator.Validate(gamelog);

		Assert.False(result.IsValid);
		Assert.Contains(
			result.Errors,
			e =>
				string.Equals(
					expectedError,
					e.ErrorMessage,
					StringComparison.InvariantCultureIgnoreCase
				)
		);
	}

	[Theory]
	[InlineData(10)]
	[InlineData(12)]
	[InlineData(16)]
	public void Given6v6AmountOfPlayers_ValidationPasses(int playerCount)
	{
		GameLog gamelog = CreateValidGameLog(playerCount);

		ValidationResult result = validator.Validate(gamelog);

		Assert.True(result.IsValid);
	}

	private GameLog CreateValidGameLog(int playerCount) =>
		fixture
			.Build<GameLog>()
			.With(x => x.Duration, 1000U)
			.With(
				x => x.PlayerStats,
				fixture
					.Build<PlayerStats>()
					.With(p => p.Team, "Red")
					.With(p => p.TeamId, 0)
					.With(p => p.Heals, 0)
					.With(p => p.Damage, 5000)
					.With(
						p => p.ClassStats,
						fixture
							.Build<ClassStats>()
							.With(cs => cs.ClassId, (byte)1)
							.With(cs => cs.Type, "Scout")
							.CreateMany()
							.ToList()
					)
					.CreateMany(playerCount)
					.ToList()
			)
			.Create();
}
