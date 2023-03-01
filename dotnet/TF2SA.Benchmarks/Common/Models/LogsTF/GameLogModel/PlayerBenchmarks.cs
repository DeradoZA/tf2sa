using BenchmarkDotNet.Attributes;
using SteamKit2;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.Benchmarks.Common.Models.LogsTF.GameLogModel;

[MemoryDiagnoser]
public class PlayerBenchmarks
{
	[Benchmark]
	public void GivenSteamID3Format_ReturnsValid_SteamID64()
	{
		string steamId3 = "[U:1:96137874]";

		SteamID steamId = Player.MakeSteamIdFromString(steamId3);
		ulong idReturned = steamId.ConvertToUInt64();
	}

	[Benchmark]
	public void GivenSteamIDFormat_ReturnsValid_SteamID64()
	{
		string steamId3 = "STEAM_0:0:48068937";

		SteamID steamId = Player.MakeSteamIdFromString(steamId3);
		ulong idReturned = steamId.ConvertToUInt64();
	}
}
