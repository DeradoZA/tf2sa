namespace TF2SA.StatsETLService
{
	internal interface IStatsETLService
	{
		Task ProcessLogs(CancellationToken cancellationToken);
	}
}
