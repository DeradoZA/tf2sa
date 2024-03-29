using System;
using System.Linq;
using Monad;
using Moq;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.Base.Serialization;
using Xunit;

namespace TF2SA.Tests.Unit.Http.Base.Serialization;

public class SerializationTests
{
	private readonly GameLog? GameLog;
	private readonly PlayerStats? MedicPlayer;
	private readonly MedicStats? MedicStats;
	private readonly ClassStats? ClassStats;
	private readonly WeaponStats? WeaponStats;
	private readonly Round? FirstRound;
	private readonly TF2SAJsonSerializer serializer = new();

	public SerializationTests()
	{
		GameLog = serializer
			.Deserialize<GameLog>(SerializationStubs.NormalGameLogJsonResponse)
			.Right!;

		MedicPlayer = GameLog?.PlayerStats?.FirstOrDefault(
			s =>
				string.Equals(
					s?.Player?.SteamId?.ToString(),
					"[U:1:152151801]",
					StringComparison.InvariantCultureIgnoreCase
				)
		);
		MedicStats = MedicPlayer?.MedicStats;
		ClassStats = MedicPlayer?.ClassStats?.FirstOrDefault(
			c => c.Type == "medic"
		);

		WeaponStats = ClassStats?.WeaponStats?.Single(
			ws =>
				string.Equals(
					"crusaders_crossbow",
					ws.WeaponName,
					StringComparison.CurrentCultureIgnoreCase
				)
		);

		FirstRound = GameLog?.Rounds?[0];
	}

	[Fact]
	public void NonImplementedThrows()
	{
		Assert.Throws<NotImplementedException>(
			() => serializer.Serialize(It.IsAny<GameLog>())
		);
	}

	[Fact]
	public void GivenEmptyString_ReturnsSerializationError()
	{
		EitherStrict<SerializationError, GameLog> nullGameLog =
			serializer.Deserialize<GameLog>("");

		Assert.True(nullGameLog.IsLeft);
	}

	[Fact]
	public void TestRootGameLog()
	{
		Assert.Equal(1787U, GameLog?.Duration);
		Assert.True(GameLog?.Success);
		Assert.Equal(3, GameLog?.Version);
	}

	[Fact]
	public void TestTeamStats()
	{
		TeamStats? red = GameLog
			?.Teams?.Where(
				t =>
					string.Equals(
						"Red",
						t.TeamId,
						StringComparison.InvariantCultureIgnoreCase
					)
			)
			.SingleOrDefault();
		TeamStats? blue = GameLog
			?.Teams?.Where(
				t =>
					string.Equals(
						"Blue",
						t.TeamId,
						StringComparison.InvariantCultureIgnoreCase
					)
			)
			.SingleOrDefault();
		if (red is null || blue is null)
		{
			Assert.NotNull(red);
			Assert.NotNull(blue);
			return;
		}

		Assert.Equal("Red", red.TeamId);
		Assert.Equal("Blue", blue.TeamId);
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
		if (MedicPlayer is null)
		{
			Assert.NotNull(MedicPlayer);
			return;
		}
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
		Assert.Equal(7, MedicPlayer.LongestKillStreak);
		Assert.Equal(0, MedicPlayer.Airshots);
		Assert.Equal(435, MedicPlayer.Dapd);
		Assert.Equal(263, MedicPlayer.Dapm);
		Assert.Equal(5, MedicPlayer.Ubers);
		Assert.Equal(5, MedicPlayer.UberTypes?["medigun"]);
		Assert.Equal(0, MedicPlayer.Drops);
		Assert.Equal(54, MedicPlayer.MedKits);
		Assert.Equal(1602, MedicPlayer.MedkitsHp);
		Assert.Equal(0, MedicPlayer.Backstabs);
		Assert.Equal(0, MedicPlayer.Headshots);
		Assert.Equal(0, MedicPlayer.HeadshotsHit);
		Assert.Equal(0, MedicPlayer.SentriesBuilt);
		Assert.Equal(12219, MedicPlayer.Heals);
		Assert.Equal(3, MedicPlayer.CapturePointsCaptured);
	}

	[Fact]
	public void TestMedicStats()
	{
		if (MedicStats is null)
		{
			Assert.NotNull(MedicStats);
			return;
		}
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
		if (ClassStats is null)
		{
			Assert.NotNull(ClassStats);
			return;
		}
		Assert.Equal(4, ClassStats.Kills);
		Assert.Equal(10, ClassStats.Assists);
		Assert.Equal(5, ClassStats.Deaths);
		Assert.Equal(1652, ClassStats.Damage);
		Assert.Equal(767, ClassStats.Playtime);
	}

	[Fact]
	public void TestWeaponStats()
	{
		if (WeaponStats is null)
		{
			Assert.NotNull(WeaponStats);
			return;
		}
		Assert.Equal(2, WeaponStats.Kills);
		Assert.Equal(1132, WeaponStats.Damage);
		Assert.Equal(53.904761904761905, WeaponStats.AverageDamage);
		Assert.Equal(143, WeaponStats.Shots);
		Assert.Equal(77, WeaponStats.Hits);
	}

	[Fact]
	public void TestNames()
	{
		Player expectedPlayer = GameLog?.Names?.SingleOrDefault(
			n =>
				string.Equals(
					n?.SteamId?.ToString(),
					"[U:1:28353669]",
					StringComparison.InvariantCultureIgnoreCase
				)
		)!;
		Assert.NotNull(expectedPlayer);
		Assert.Equal("Skye", expectedPlayer.PlayerName);
	}

	[Fact]
	public void TestRounds()
	{
		if (FirstRound is null)
		{
			Assert.NotNull(FirstRound);
			return;
		}
		Assert.Equal(1656195371, FirstRound.StartTime);
		Assert.Equal("Red", FirstRound.WinnerTeam);
		Assert.Equal("Red", FirstRound.FirstCapTeam);
		Assert.Equal(87, FirstRound.Length);
	}

	[Fact]
	public void TestRoundTeam()
	{
		TeamRound? redTeamRound = FirstRound?.TeamRound?["Red"];
		if (redTeamRound is null)
		{
			Assert.NotNull(redTeamRound);
			return;
		}
		Assert.Equal(1, redTeamRound.Score);
		Assert.Equal(11, redTeamRound.Kills);
		Assert.Equal(3334, redTeamRound.Damage);
		Assert.Equal(1, redTeamRound.Ubers);
	}

	[Fact]
	public void TestRoundEvents()
	{
		RoundEvent? firstRoundEvent = FirstRound?.RoundEvents?[0];
		if (firstRoundEvent is null)
		{
			Assert.NotNull(firstRoundEvent);
			return;
		}
		Assert.Equal("medic_death", firstRoundEvent.Type);
		Assert.Equal(41, firstRoundEvent.Time);
		Assert.Equal("Blue", firstRoundEvent.Team);
		Assert.Equal("[U:1:120175766]", firstRoundEvent.SteamId);
		Assert.Equal("[U:1:28353669]", firstRoundEvent.KillerSteamId);
	}

	[Fact]
	public void TestRoundPlayers()
	{
		var firstPlayer = FirstRound?.PlayerRounds?["[U:1:51337520]"];
		if (firstPlayer is null)
		{
			Assert.NotNull(firstPlayer);
			return;
		}
		Assert.Equal("Red", firstPlayer.Team);
		Assert.Equal(1, firstPlayer.Kills);
		Assert.Equal(613, firstPlayer.Damage);
	}

	[Fact]
	public void TestHealSpread()
	{
		Assert.Equal(
			2201,
			GameLog?.HealSpread?["[U:1:152151801]"]["[U:1:51337520]"]
		);
	}

	[Fact]
	public void TestClassKills()
	{
		Assert.Equal(5, GameLog?.ClassKills?["[U:1:159058380]"].Soldier);
		Assert.Equal(2, GameLog?.ClassKills?["[U:1:159058380]"].Heavy);
		Assert.Equal(6, GameLog?.ClassKills?["[U:1:159058380]"].Scout);
		Assert.Equal(6, GameLog?.ClassKills?["[U:1:159058380]"].Medic);
		Assert.Equal(2, GameLog?.ClassKills?["[U:1:159058380]"].Sniper);
		Assert.Equal(4, GameLog?.ClassKills?["[U:1:159058380]"].Engineer);
		Assert.Equal(2, GameLog?.ClassKills?["[U:1:159058380]"].Demoman);
		Assert.Equal(4, GameLog?.ClassKills?["[U:1:159058380]"].Pyro);
	}

	[Fact]
	public void TestClassDeaths()
	{
		Assert.Equal(4, GameLog?.ClassDeaths?["[U:1:96137874]"].Scout);
		Assert.Equal(1, GameLog?.ClassDeaths?["[U:1:96137874]"].Demoman);
		Assert.Equal(2, GameLog?.ClassDeaths?["[U:1:96137874]"].Pyro);
		Assert.Equal(2, GameLog?.ClassDeaths?["[U:1:96137874]"].Soldier);
		Assert.Equal(2, GameLog?.ClassDeaths?["[U:1:96137874]"].Heavy);
		Assert.Equal(2, GameLog?.ClassDeaths?["[U:1:96137874]"].Medic);
		Assert.Equal(1, GameLog?.ClassDeaths?["[U:1:96137874]"].Engineer);
		Assert.Equal(1, GameLog?.ClassDeaths?["[U:1:96137874]"].Sniper);
	}

	[Fact]
	public void TestClassKillAssists()
	{
		Assert.Equal(7, GameLog?.ClassKillAssists?["[U:1:159058380]"].Soldier);
		Assert.Equal(3, GameLog?.ClassKillAssists?["[U:1:159058380]"].Heavy);
		Assert.Equal(7, GameLog?.ClassKillAssists?["[U:1:159058380]"].Scout);
		Assert.Equal(3, GameLog?.ClassKillAssists?["[U:1:159058380]"].Demoman);
		Assert.Equal(7, GameLog?.ClassKillAssists?["[U:1:159058380]"].Medic);
		Assert.Equal(2, GameLog?.ClassKillAssists?["[U:1:159058380]"].Sniper);
		Assert.Equal(4, GameLog?.ClassKillAssists?["[U:1:159058380]"].Engineer);
		Assert.Equal(4, GameLog?.ClassKillAssists?["[U:1:159058380]"].Pyro);
		Assert.Equal(2, GameLog?.ClassKillAssists?["[U:1:159058380]"].Spy);
	}

	[Fact]
	public void TestChat()
	{
		Chat? firstMessage = GameLog?.Chats?[0];
		if (firstMessage is null)
		{
			Assert.NotNull(firstMessage);
			return;
		}
		Assert.Equal("[U:1:159058380]", firstMessage.SteamId);
		Assert.Equal("hondjo", firstMessage.Name);
		Assert.Equal("stop emailing", firstMessage.Message);
	}

	[Fact]
	public void TestInfo()
	{
		LogInfo? info = GameLog?.Info;
		if (info is null)
		{
			Assert.NotNull(info);
			return;
		}
		Assert.Equal("cp_process_f9a", info.Map);
		Assert.True(info.IsSupplemental);
		Assert.Equal(1787, info.TotalLength);
		Assert.True(info.HasRealDamage);
		Assert.True(info.HasWeaponDamage);
		Assert.True(info.HasAccuracy);
		Assert.True(info.HasHp);
		Assert.True(info.HasHPReal);
		Assert.True(info.HasHeadshots);
		Assert.True(info.HasHeadshotsHit);
		Assert.True(info.HasBackstabs);
		Assert.True(info.HasCapturePointsCaptured);
		Assert.False(info.HasSentriesBuilt);
		Assert.True(info.HasDamageTaken);
		Assert.True(info.HasAirshots);
		Assert.True(info.HasHealsReceived);
		Assert.False(info.HasIntelCaptures);
		Assert.False(info.HasAdscoring);
		Assert.Equal("TF2SA Pug: RED vs fwian", info.Title);
		Assert.Equal(1656197193, info.Date);

		var uploader = info?.Uploader;
		if (uploader is null)
		{
			Assert.NotNull(uploader);
			return;
		}
		Assert.Equal("76561199085369255", uploader.Id);
		Assert.Equal("TF2SA", uploader.Name);
		Assert.Equal("LogsTF 2.3.0", uploader.Info);
	}

	[Fact]
	public void TestKillStreaks()
	{
		KillStreak? firstKillStreak = GameLog?.KillStreaks?[0];
		if (firstKillStreak is null)
		{
			Assert.NotNull(firstKillStreak);
			return;
		}
		Assert.Equal("[U:1:28353669]", firstKillStreak.SteamId);
		Assert.Equal(3, firstKillStreak.Streak);
		Assert.Equal(36, firstKillStreak.Time);
	}
}
