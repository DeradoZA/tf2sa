using Xunit;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using SteamKit2;

namespace TF2SA.Tests.Unit.Common.Models.LogsTF.GameLogModel;

public class PlayerTests
{
	[Fact]
	public void GivenSteamID3Format_ReturnsValid_SteamID64()
	{
		// Arrange
		string steamId3 = "[U:1:96137874]";
		ulong expectedSteamId64 = 76561198056403602;

		// Act
		SteamID steamId = Player.MakeSteamIdFromString(steamId3);
		ulong idReturned = steamId.ConvertToUInt64();

		// Assert
		Assert.Equal(expectedSteamId64, idReturned);
	}

	[Fact]
	public void GivenSteamIDFormat_ReturnsValid_SteamID64()
	{
		// Arrange
		string steamId3 = "STEAM_0:0:48068937";
		ulong expectedSteamId64 = 76561198056403602;

		// Act
		SteamID steamId = Player.MakeSteamIdFromString(steamId3);
		ulong idReturned = steamId.ConvertToUInt64();

		// Assert
		Assert.Equal(expectedSteamId64, idReturned);
	}
}
