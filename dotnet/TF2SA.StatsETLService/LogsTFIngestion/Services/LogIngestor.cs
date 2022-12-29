using FluentValidation.Results;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.LogsTF.Service;
using TF2SA.StatsETLService.LogsTFIngestion.Errors;
using TF2SA.StatsETLService.LogsTFIngestion.Validation;
using System.Linq;

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

	public async Task<OptionStrict<List<Error>>> IngestLog(
		LogListItem logListItem,
		CancellationToken cancellationToken
	)
	{
		List<Error> ingestionErrors = new();

		EitherStrict<HttpError, GameLog> logResult =
			await logsTFService.GetGameLog(logListItem.Id, cancellationToken);
		if (logResult.IsLeft)
		{
			ingestionErrors.Add(
				new IngestionError(
					$"Failed to fetch Log: {logResult.Left.Message}"
				)
			);
			return ingestionErrors;
		}
		GameLog log = logResult.Right;

		GameLogValidator validator = new();
		ValidationResult validationResult = validator.Validate(log);
		if (!validationResult.IsValid)
		{
			ingestionErrors.AddRange(
				validationResult.Errors.Select(
					error =>
						new IngestionError(
							$"Validation Error on Gamelog: {error.ErrorMessage}"
						)
				)
			);
			return ingestionErrors;
		}

		return OptionStrict<List<Error>>.Nothing;
	}
}
