using FluentValidation.Results;
using Monad;
using TF2SA.Common.Errors;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.LogsTF.Service;
using TF2SA.StatsETLService.LogsTFIngestion.Errors;
using TF2SA.StatsETLService.LogsTFIngestion.Validation;

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

	public async Task<bool> IngestLog(
		LogListItem logListItem,
		CancellationToken cancellationToken
	)
	{
		List<Error> ingestionErrors = new();

		EitherStrict<HttpError, GameLog> logResult =
			await logsTFService.GetGameLog(logListItem.Id, cancellationToken);
		if (logResult.IsLeft)
		{
			ReportLogFetchFailed(logListItem, ingestionErrors, logResult);
			return false;
		}
		GameLog log = logResult.Right;

		GameLogValidator validator = new();
		ValidationResult validationResult = validator.Validate(log);
		if (!validationResult.IsValid)
		{
			ReportLogValidationFailed(
				logListItem,
				ingestionErrors,
				validationResult
			);
			return false;
		}

		// Insert valid log

		return true;
	}

	private void ReportLogFetchFailed(
		LogListItem logListItem,
		List<Error> ingestionErrors,
		EitherStrict<HttpError, GameLog> logResult
	)
	{
		ingestionErrors.Add(
			new IngestionError($"Failed to fetch Log: {logResult.Left.Message}")
		);
		logger.LogWarning(
			"Failed to fetch {logId} from LogsTF API. "
				+ "No changes to database. Error: {error}",
			logListItem.Id,
			logResult.Left.Message
		);
	}

	private void ReportLogValidationFailed(
		LogListItem logListItem,
		List<Error> ingestionErrors,
		ValidationResult validationResult
	)
	{
		ingestionErrors.AddRange(
			validationResult.Errors.Select(
				error =>
					new IngestionError(
						$"Validation Error on Gamelog: {error.ErrorMessage}"
					)
			)
		);

		// insertInvalidLog

		string errorString = string.Join(
			Environment.NewLine,
			validationResult.Errors.Select(e => e.ErrorMessage)
		);
		logger.LogWarning(
			"Failed to validate GameLog {logId} from LogsTF API. "
				+ "Log marked as invalid written to database. "
				+ "Validation Errors:{newline}{errors}",
			logListItem.Id,
			Environment.NewLine,
			errorString
		);
	}
}
