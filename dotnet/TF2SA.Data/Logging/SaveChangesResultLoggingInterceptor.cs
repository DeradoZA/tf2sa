using Microsoft.EntityFrameworkCore.Diagnostics;

namespace TF2SA.Data.Logging;

/// <summary>
/// Debug-only: logs how many rows <c>SaveChanges</c> wrote, so
/// <c>Debug:LogDatabaseQueries</c> shows a result alongside the SQL emitted by
/// <c>.LogTo</c>. Only registered when that flag is on (local dev).
/// </summary>
public class SaveChangesResultLoggingInterceptor : SaveChangesInterceptor
{
	public override int SavedChanges(
		SaveChangesCompletedEventData eventData,
		int result
	)
	{
		Console.WriteLine($"-- SaveChanges wrote {result} row(s) --");
		return base.SavedChanges(eventData, result);
	}

	public override ValueTask<int> SavedChangesAsync(
		SaveChangesCompletedEventData eventData,
		int result,
		CancellationToken cancellationToken = default
	)
	{
		Console.WriteLine($"-- SaveChanges wrote {result} row(s) --");
		return base.SavedChangesAsync(eventData, result, cancellationToken);
	}
}
