using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monad;
using Moq;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Common.Models.LogsTF.LogListModel;
using TF2SA.Http.Base.Client;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.LogsTF.Config;
using TF2SA.Http.LogsTF.Service;
using Xunit;

namespace TF2SA.Tests.Unit.Http.LogsTF.Service;

public class LogsTFServiceTests
{
	private readonly Mock<IOptions<LogsTFConfig>> logsTFConfig = new();
	private readonly Mock<ILogger<LogsTFService>> logger = new();
	private readonly Mock<IHttpClient> httpClient = new();
	private readonly LogsTFService logsTFService;

	public LogsTFServiceTests()
	{
		logsTFConfig
			.Setup(x => x.Value)
			.Returns(
				new LogsTFConfig()
				{
					BaseUrl = "http://logs.tf/api/v1",
					Uploaders = new ulong[] { 1, 2 }
				}
			);

		httpClient
			.Setup(
				x =>
					x.Get<GameLog>(
						It.IsAny<string>(),
						It.IsAny<CancellationToken>()
					)
			)
			.ReturnsAsync(() => GetLogSuccess());
		httpClient
			.Setup(
				x =>
					x.Get<LogListResult>(
						It.IsAny<string>(),
						It.IsAny<CancellationToken>()
					)
			)
			.ReturnsAsync(() => GetLogListSuccess());

		logsTFService = new(
			logsTFConfig.Object,
			logger.Object,
			httpClient.Object
		);
	}

	private static EitherStrict<HttpError, GameLog> GetLogSuccess() =>
		new GameLog();

	private static EitherStrict<HttpError, GameLog> GetLogFailure() =>
		new HttpError("Fail");

	private static EitherStrict<HttpError, LogListResult> GetLogListSuccess() =>
		new LogListResult() { Logs = new() };

	private static EitherStrict<
		HttpError,
		LogListResult
	> GetLogListSuccessWithMoreResultsThanLimit() =>
		new LogListResult()
		{
			Logs = new(),
			Results = 10000,
			Total = 15000
		};

	private static EitherStrict<
		HttpError,
		LogListResult
	> GetLogListSuccessWithMoreResultsThanLimitSecondSet() =>
		new LogListResult()
		{
			Logs = new(),
			Results = 5000,
			Total = 15000,
			Parameters = new() { Offset = 10000 }
		};

	private static EitherStrict<HttpError, LogListResult> GetLogListFailure() =>
		new HttpError("Fail");

	[Fact]
	public async Task GetGameLog_Success_ReturnsGameLogAsync()
	{
		EitherStrict<HttpError, GameLog> gameLog =
			await logsTFService.GetGameLog(123U, CancellationToken.None);
		Assert.True(gameLog.IsRight);
		Assert.IsType<GameLog>(gameLog.Right);
	}

	[Fact]
	public async Task GetGameLog_Failure_ReturnsError()
	{
		httpClient
			.Setup(
				x => x.Get<GameLog>(It.IsAny<string>(), CancellationToken.None)
			)
			.ReturnsAsync(() => GetLogFailure());

		EitherStrict<HttpError, GameLog> gameLog =
			await logsTFService.GetGameLog(123U, CancellationToken.None);
		Assert.True(gameLog.IsLeft);
		Assert.IsType<HttpError>(gameLog.Left);
	}

	[Fact]
	public async Task GetLogList_Success_ReturnsLogListAsync()
	{
		LogListQueryParams queryParams = new();

		EitherStrict<HttpError, LogListResult> logList =
			await logsTFService.GetLogList(queryParams, CancellationToken.None);

		Assert.True(logList.IsRight);
		Assert.IsType<LogListResult>(logList.Right);
	}

	[Fact]
	public async Task GetLogList_Failure_ReturnsError()
	{
		httpClient
			.Setup(
				x =>
					x.Get<LogListResult>(
						It.IsAny<string>(),
						It.IsAny<CancellationToken>()
					)
			)
			.ReturnsAsync(() => GetLogListFailure());

		LogListQueryParams queryParams = new();

		EitherStrict<HttpError, LogListResult> logList =
			await logsTFService.GetLogList(queryParams, CancellationToken.None);

		Assert.True(logList.IsLeft);
		Assert.IsType<HttpError>(logList.Left);
	}

	[Fact]
	public async Task GetLogList_CallsWithCorrectUrl()
	{
		LogListQueryParams queryParams = new() { Uploader = 999 };

		EitherStrict<HttpError, LogListResult> logList =
			await logsTFService.GetLogList(queryParams, CancellationToken.None);

		httpClient.Verify(
			x =>
				x.Get<LogListResult>(
					It.Is<string>(
						s => s.Equals("http://logs.tf/api/v1/log?uploader=999")
					),
					It.IsAny<CancellationToken>()
				),
			Times.Once
		);
	}

	[Fact]
	public async Task GetAllLogsFromUploader_GivenHttpError_ReturnsError()
	{
		ulong uploader = 123;

		httpClient
			.Setup(
				x =>
					x.Get<LogListResult>(
						It.IsAny<string>(),
						It.IsAny<CancellationToken>()
					)
			)
			.ReturnsAsync(() => GetLogListFailure());

		EitherStrict<HttpError, List<LogListItem>> logList =
			await logsTFService.GetAllLogs(uploader, CancellationToken.None);

		Assert.True(logList.IsLeft);
		Assert.IsType<HttpError>(logList.Left);
	}

	[Fact]
	public async Task GetAllLogsFromUploader_CallsGetWithQueryParams()
	{
		ulong uploader = 123;

		EitherStrict<HttpError, List<LogListItem>> logList =
			await logsTFService.GetAllLogs(uploader, CancellationToken.None);

		httpClient.Verify(
			x =>
				x.Get<LogListResult>(
					It.Is<string>(
						s =>
							s.Equals(
								"http://logs.tf/api/v1/log?uploader=123&limit=10000&offset=0"
							)
					),
					It.IsAny<CancellationToken>()
				),
			Times.Once
		);
	}

	[Fact]
	public async Task GetAllLogsFromUploader_MoreResultsThanLimit_MakesMultipleGetCalls_WithUpdatedOffset()
	{
		ulong uploader = 123;
		httpClient
			.Setup(
				x =>
					x.Get<LogListResult>(
						It.Is<string>(
							s =>
								s.Equals(
									"http://logs.tf/api/v1/log?uploader=123&limit=10000&offset=0"
								)
						),
						It.IsAny<CancellationToken>()
					)
			)
			.ReturnsAsync(() => GetLogListSuccessWithMoreResultsThanLimit());
		httpClient
			.Setup(
				x =>
					x.Get<LogListResult>(
						It.Is<string>(
							s =>
								s.Equals(
									"http://logs.tf/api/v1/log?uploader=123&limit=10000&offset=10000"
								)
						),
						It.IsAny<CancellationToken>()
					)
			)
			.ReturnsAsync(
				() => GetLogListSuccessWithMoreResultsThanLimitSecondSet()
			);

		EitherStrict<HttpError, List<LogListItem>> logList =
			await logsTFService.GetAllLogs(uploader, CancellationToken.None);

		httpClient.Verify(
			x =>
				x.Get<LogListResult>(
					It.Is<string>(
						s =>
							s.Equals(
								"http://logs.tf/api/v1/log?uploader=123&limit=10000&offset=0"
							)
					),
					It.IsAny<CancellationToken>()
				),
			Times.Once
		);
		httpClient.Verify(
			x =>
				x.Get<LogListResult>(
					It.Is<string>(
						s =>
							s.Equals(
								"http://logs.tf/api/v1/log?uploader=123&limit=10000&offset=10000"
							)
					),
					It.IsAny<CancellationToken>()
				),
			Times.Once
		);
	}

	[Fact]
	public async Task GetAllLogsFromAllUploader_GivenHttpError_ReturnsError()
	{
		httpClient
			.Setup(
				x =>
					x.Get<LogListResult>(
						It.IsAny<string>(),
						It.IsAny<CancellationToken>()
					)
			)
			.ReturnsAsync(() => GetLogListFailure());

		EitherStrict<HttpError, List<LogListItem>> logList =
			await logsTFService.GetAllLogs(CancellationToken.None);

		Assert.True(logList.IsLeft);
		Assert.IsType<HttpError>(logList.Left);
	}

	[Fact]
	public async Task GetAllLogsFromAllUploader_CallsMultipleGetsForEachUploader()
	{
		EitherStrict<HttpError, List<LogListItem>> logList =
			await logsTFService.GetAllLogs(CancellationToken.None);

		Assert.True(logList.IsRight);
		httpClient.Verify(
			x =>
				x.Get<LogListResult>(
					It.IsAny<string>(),
					It.IsAny<CancellationToken>()
				),
			Times.AtLeast(2)
		);
	}
}
