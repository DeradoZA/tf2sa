namespace TF2SA.StatsETLService
{
	internal interface IStatsETLService
	{
		Task Execute(CancellationToken cancellationToken);
	}
}
