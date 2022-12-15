using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Monad;
using Moq;
using Moq.Protected;
using TF2SA.Common.Models.LogsTF.GameLogModel;
using TF2SA.Http.Base.Client;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.Base.Serialization;
using TF2SA.Tests.Unit.Http.Base.Serialization;
using Xunit;

namespace TF2SA.Tests.Unit.Http.Base.Client;

public class TF2SAHttpClientTests
{
	private readonly Mock<IHttpClientFactory> httpClientFactory = new();
	private readonly Mock<ILogger<TF2SAHttpClient>> logger = new();
	private readonly Mock<IJsonSerializer> jsonSerializer = new();
	private readonly HttpClient httpClient = new();

	private readonly HttpResponseMessage httpResponseMessage;
	private readonly TF2SAHttpClient tF2SAHttpClient;

	public TF2SAHttpClientTests()
	{
		httpResponseMessage = new()
		{
			StatusCode = HttpStatusCode.OK,
			Content = new StringContent(
				SerializationStubs.NormalGameLogJsonResponse
			)
		};

		Mock<HttpMessageHandler> handlerMock = new(MockBehavior.Strict);
		handlerMock
			.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>()
			)
			.ReturnsAsync(httpResponseMessage)
			.Verifiable();
		httpClient = new HttpClient(handlerMock.Object);

		httpClientFactory
			.Setup(x => x.CreateClient(string.Empty))
			.Returns(httpClient);

		jsonSerializer
			.Setup(x => x.Deserialize<GameLog>(It.IsAny<string>()))
			.Returns(ErrorSerializationResponse());

		tF2SAHttpClient = new(
			httpClientFactory.Object,
			logger.Object,
			jsonSerializer.Object
		);
	}

	private static EitherStrict<
		SerializationError,
		GameLog
	> ErrorSerializationResponse() => new SerializationError("Fail");

	private static EitherStrict<
		SerializationError,
		GameLog
	> SuccessSerializationResponse() => new GameLog();

	[Fact]
	public async Task Get_GivenSerializationFailure_ReturnsHttpError()
	{
		var result = await tF2SAHttpClient.Get<GameLog>(
			"http://logs.tf/api/v1"
		);
		Assert.True(result.IsLeft);
		Assert.IsType<HttpError>(result.Left);
		Assert.Equal("Fail", result.Left.Message);
	}

	[Fact]
	public async Task GetGivenGamelog_GivenSerializationSuccess_ReturnsGameLog()
	{
		jsonSerializer
			.Setup(x => x.Deserialize<GameLog>(It.IsAny<string>()))
			.Returns(SuccessSerializationResponse());

		var result = await tF2SAHttpClient.Get<GameLog>(
			"http://logs.tf/api/v1"
		);
		Assert.True(result.IsRight);
		Assert.IsType<GameLog>(result.Right);
	}

	[Fact]
	public async Task GetGivenGamelog_GivenException_ReturnsHttpError()
	{
		jsonSerializer
			.Setup(x => x.Deserialize<GameLog>(It.IsAny<string>()))
			.Throws<JsonException>();

		var result = await tF2SAHttpClient.Get<GameLog>(
			"http://logs.tf/api/v1"
		);
		Assert.True(result.IsLeft);
		Assert.IsType<HttpError>(result.Left);
	}

	[Fact]
	public async Task NonImplementedThrows()
	{
		await Assert.ThrowsAsync<NotImplementedException>(
			() => tF2SAHttpClient.Delete<GameLog>("http://logs.tf/api/v1")
		);

		await Assert.ThrowsAsync<NotImplementedException>(
			() => tF2SAHttpClient.Patch<GameLog>("http://logs.tf/api/v1")
		);

		await Assert.ThrowsAsync<NotImplementedException>(
			() => tF2SAHttpClient.Post<GameLog>("http://logs.tf/api/v1")
		);

		await Assert.ThrowsAsync<NotImplementedException>(
			() => tF2SAHttpClient.Put<GameLog>("http://logs.tf/api/v1")
		);
	}
}
