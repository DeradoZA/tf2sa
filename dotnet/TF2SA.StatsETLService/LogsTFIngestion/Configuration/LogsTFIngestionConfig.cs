namespace TF2SA.StatsETLService.LogsTFIngestion.Configuration;

public class LogsTFIngestionConfig
{
    public bool EnableIngestion { get; init; } = false;
    public int IngestionIntervalSeconds { get; init; } = 30;
    public string ClickhouseConnectionString { get; init; } = "Host=localhost;Port=9000;Database=tf2sa;User=default;Password=";
    public int ConcurrentThreads { get; init; }
}