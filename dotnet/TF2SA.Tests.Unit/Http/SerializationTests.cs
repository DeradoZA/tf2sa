using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Monad;
using TF2SA.Http.Errors;
using TF2SA.Http.LogsTF.Models.GameLogModel;
using TF2SA.Http.LogsTF.Serialization;
using Xunit;

namespace TF2SA.Tests.Unit.Http;

public class SerializationTests
{
	private readonly GameLog GameLog;
	private readonly PlayerStats MedicPlayer;
	private readonly MedicStats MedicStats;
	private readonly ClassStats ClassStats;
	private readonly WeaponStats WeaponStats;

	public SerializationTests()
	{
		GameLog = LogsTFSerializer<GameLog>
			.Deserialize(SerializationStubs.NormalGameLogJsonResponse)
			.Right;

		MedicPlayer = GameLog.Players["[U:1:152151801]"];
		MedicStats = MedicPlayer.MedicStats;
		ClassStats = MedicPlayer.ClassStats?.FirstOrDefault(c => c.Type == "medic");
		WeaponStats = ClassStats.Weapons["crusaders_crossbow"];
	}

	[Fact]
	public void TestRootGameLog()
	{
		Assert.Equal(1787U, GameLog.Length);
		Assert.True(GameLog.Success);
		Assert.Equal(3, GameLog.Version);
	}

	[Fact]
	public void TestTeamStats()
	{
		TeamStats red = GameLog.Teams["Red"];
		TeamStats blue = GameLog.Teams["Blue"];
		Assert.NotNull(red);
		Assert.NotNull(blue);

		Assert.Equal(4, red.Score);
		Assert.Equal(158, red.Kills);
		Assert.Equal(0, red.Deaths);
		Assert.Equal(47204, red.Damage);
		Assert.Equal(6, red.UberCharges);
		Assert.Equal(1, blue.Drops);
		Assert.Equal(19, red.Captures);
	}

	[Fact]
	public void TestPlayerStats()
	{
		Assert.Equal("Red", MedicPlayer.Team);
		Assert.Equal(26, MedicPlayer.Kills);
		Assert.Equal(18, MedicPlayer.Deaths);
		Assert.Equal(0, MedicPlayer.Suicides);
		Assert.Equal("2.4", MedicPlayer.Kapd);
		Assert.Equal("1.4", MedicPlayer.Kpd);
		Assert.Equal(7843, MedicPlayer.Damage);
		Assert.Equal(563, MedicPlayer.DamageReal);
		Assert.Equal(6192, MedicPlayer.DamageTaken);
		Assert.Equal(662, MedicPlayer.DamageTakenReal);
		Assert.Equal(822, MedicPlayer.HealsReceived);
		Assert.Equal(7, MedicPlayer.Lks);
		Assert.Equal(0, MedicPlayer.Airshots);
		Assert.Equal(435, MedicPlayer.Dapd);
		Assert.Equal(263, MedicPlayer.Dapm);
		Assert.Equal(5, MedicPlayer.Ubers);
		Assert.Equal(5, MedicPlayer.UberTypes?["medigun"]);
		Assert.Equal(0, MedicPlayer.Drops);
		Assert.Equal(54, MedicPlayer.MedKits);
		Assert.Equal(1602, MedicPlayer.MedKitsHealth);
		Assert.Equal(0, MedicPlayer.BackStabs);
		Assert.Equal(0, MedicPlayer.Headshots);
		Assert.Equal(0, MedicPlayer.HeadshotsHit);
		Assert.Equal(0, MedicPlayer.Sentries);
		Assert.Equal(0, MedicPlayer.Heals);
		Assert.Equal(3, MedicPlayer.Cpc);
	}

	[Fact]
	public void TestMedicStats()
	{
		Assert.Equal(0, MedicStats.AdvantagesLost);
		Assert.Equal(0, MedicStats.BiggestAdvantageLost);
		Assert.Equal(0, MedicStats.DeathsWith95To99Uber);
		Assert.Equal(1, MedicStats.DeathsWitin20sAfterUber);
		Assert.Equal(3.45, MedicStats.AverageTimeBeforeHealing);
		Assert.Equal(13.2, MedicStats.AverageTimeBeforeUsing);
		Assert.Equal(7.3999999999999995, MedicStats.AverageUberLength);
	}

	[Fact]
	public void TestClassStats()
	{
		Assert.Equal(4, ClassStats.Kills);
		Assert.Equal(10, ClassStats.Assists);
		Assert.Equal(5, ClassStats.Deaths);
		Assert.Equal(1652, ClassStats.Damage);
		Assert.Equal(767, ClassStats.TotalTime);
	}

	[Fact]
	public void TestWeaponStatus()
	{
		Assert.Equal(2, WeaponStats.Kills);
		Assert.Equal(1132, WeaponStats.Damage);
		Assert.Equal(53.904761904761905, WeaponStats.AverageDamage);
		Assert.Equal(143, WeaponStats.Shots);
		Assert.Equal(77, WeaponStats.Hits);
	}

}