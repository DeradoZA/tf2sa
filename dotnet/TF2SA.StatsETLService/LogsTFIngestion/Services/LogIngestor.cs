using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Data.Entities.MariaDb;
using TF2SA.Data.Errors;
using TF2SA.Data.Repositories.Base;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.LogsTF.Service;

namespace TF2SA.StatsETLService.LogsTFIngestion.Services;

public class LogIngestor : ILogIngestor
{
	private readonly ILogger<LogIngestor> logger;
	private readonly ILogsTFService logsTFService;

	public LogIngestor(
		ILogger<LogIngestor> logger,
		ILogsTFService logsTFService
	)
	{
		this.logger = logger;
		this.logsTFService = logsTFService;
	}
}
