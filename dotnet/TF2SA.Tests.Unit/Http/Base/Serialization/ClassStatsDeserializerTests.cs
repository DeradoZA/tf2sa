using System.Collections.Generic;
using System;
using Monad;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.Base.Serialization;
using Xunit;
using System.Linq;

namespace TF2SA.Tests.Unit.Http.Base.Serialization;

public class ClassStatsDeserializerTests
{
	private readonly TF2SAJsonSerializer serializer = new();

	[Fact]
	public void GivenClassStatsWithEmptyWeapons_DeserializationSucceeeds_ReturnsEmpty()
	{
		EitherStrict<SerializationError, ClassStats> classStatResult =
			serializer.Deserialize<ClassStats>(
				SerializationStubs.ClassStatsWithEmptyWeapons
			);

		Assert.True(classStatResult.IsRight);

		ClassStats classStats = classStatResult.Right;
		Assert.Equal("scout", classStats.Type);
		Assert.Equal(9, classStats.Kills);
		Assert.Equal(4052, classStats.Damage);

		List<WeaponStats> weaponStats = classStats.WeaponStats!;
		Assert.NotNull(weaponStats);
		Assert.Empty(weaponStats);
	}

	[Fact]
	public void GivenClassStatsWithWeaponsObject_DeserializationSucceeeds_WithCorrectProperties()
	{
		EitherStrict<SerializationError, ClassStats> classStatResult =
			serializer.Deserialize<ClassStats>(
				SerializationStubs.ClassStatsWithObjectWeapons
			);

		Assert.True(classStatResult.IsRight);

		ClassStats classStats = classStatResult.Right;
		Assert.Equal("scout", classStats.Type);
		Assert.Equal(9, classStats.Kills);
		Assert.Equal(4052, classStats.Damage);

		List<WeaponStats> weaponStats = classStats.WeaponStats!;
		Assert.NotNull(weaponStats);

		WeaponStats scatterGunStats = weaponStats.Single(
			w =>
				string.Equals(
					w.WeaponName,
					"scattergun",
					StringComparison.InvariantCultureIgnoreCase
				)
		);
		Assert.NotNull(scatterGunStats);

		Assert.Equal(9, scatterGunStats.Kills);
		Assert.Equal(3953, scatterGunStats.Damage);
		Assert.Equal(0, scatterGunStats.Shots);
		Assert.Equal(145, scatterGunStats.Hits);
		Assert.Contains("scattergun", weaponStats.Select(w => w.WeaponName));
		Assert.Contains("the_winger", weaponStats.Select(w => w.WeaponName));
	}

	[Fact]
	public void GivenClassStatsWithWeaponsInt_DeserializationSucceeeds_WithCorrectProperties()
	{
		EitherStrict<SerializationError, ClassStats> classStatResult =
			serializer.Deserialize<ClassStats>(
				SerializationStubs.ClassStatsWithDictIntWeapons
			);

		Assert.True(classStatResult.IsRight);

		ClassStats classStats = classStatResult.Right;
		Assert.Equal("scout", classStats.Type);
		Assert.Equal(9, classStats.Kills);
		Assert.Equal(4052, classStats.Damage);

		List<WeaponStats> weaponStats = classStats.WeaponStats!;
		Assert.NotNull(weaponStats);

		WeaponStats scatterGunStats = weaponStats.Single(
			w =>
				string.Equals(
					w.WeaponName,
					"scattergun",
					StringComparison.InvariantCultureIgnoreCase
				)
		);
		Assert.NotNull(scatterGunStats);

		Assert.Equal(3, scatterGunStats.Kills);
		Assert.Contains("scattergun", weaponStats.Select(w => w.WeaponName));
		Assert.Contains("the_winger", weaponStats.Select(w => w.WeaponName));
	}
}
