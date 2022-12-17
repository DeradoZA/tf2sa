namespace TF2SA.StatsETLService.LogsTFIngestion.Handlers;

internal interface ILogsTFIngestionHandler
{
	Task ExecuteAsync(CancellationToken cancellationToken);
}
