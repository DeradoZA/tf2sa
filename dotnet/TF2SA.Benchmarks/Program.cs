using BenchmarkDotNet.Running;
using TF2SA.Benchmarks.Common.Models.LogsTF.GameLogModel;

public class Program
{
	public static void Main(string[] args)
	{
		var summary = BenchmarkRunner.Run<PlayerBenchmarks>();
	}
}
