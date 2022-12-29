using TF2SA.Common.Errors;

namespace TF2SA.StatsETLService.LogsTFIngestion.Errors;

public class IngestionError : Error
{
	public IngestionError(string message)
	{
		Message = message;
	}
}
