namespace TF2SA.StatsETLService.LogsTFIngestion.Configuration;

public class LogsTFIngestionConfig
{
	public const string LogsTFIngestionConfigSection = "LogsTFIngestion";
	public bool EnableIngestion { get; set; } = false;
	public int IngestionIntervalSeconds { get; set; } = 30 * 60;
}
