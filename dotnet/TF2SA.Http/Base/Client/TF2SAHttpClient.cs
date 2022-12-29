using Microsoft.Extensions.Logging;
using Monad;
using TF2SA.Http.Base.Errors;
using TF2SA.Http.Base.Serialization;

namespace TF2SA.Http.Base.Client;

public class TF2SAHttpClient : IHttpClient
{
	private readonly IHttpClientFactory httpClientFactory;
	private readonly ILogger<TF2SAHttpClient> logger;
	private readonly IJsonSerializer jsonSerializer;

	public TF2SAHttpClient(
		IHttpClientFactory httpClientFactory,
		ILogger<TF2SAHttpClient> logger,
		IJsonSerializer jsonSerializer
	)
	{
		this.httpClientFactory = httpClientFactory;
		this.logger = logger;
		this.jsonSerializer = jsonSerializer;
	}

	public async Task<EitherStrict<HttpError, TResponse>> Get<TResponse>(
		string url,
		CancellationToken cancellationToken
	)
	{
		HttpClient httpClient = httpClientFactory.CreateClient();

		try
		{
			HttpResponseMessage response = await httpClient.GetAsync(
				url,
				cancellationToken
			);
			if (!response.IsSuccessStatusCode)
			{
				return new HttpError(
					$"Failed HTTP request with code {response.StatusCode}. Headers: {response.Headers}"
				);
			}

			string json = await response.Content.ReadAsStringAsync(
				cancellationToken
			);

			EitherStrict<SerializationError, TResponse> deserialized =
				jsonSerializer.Deserialize<TResponse>(json);
			if (deserialized.IsLeft)
			{
				logger.LogWarning(
					"GET {url}: Failed deserialization: {deserialized.Left.Message}. String: {jsonString}",
					url,
					deserialized.Left.Message,
					json
				);
				return new HttpError(deserialized.Left.Message);
			}

			return deserialized.Right;
		}
		catch (Exception e)
		{
			logger.LogWarning("GET {url}: Failed: {e.Message}", url, e.Message);
			return new HttpError(e.Message);
		}
	}

	public Task<EitherStrict<HttpError, TResponse>> Delete<TResponse>(
		string url,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<HttpError, TResponse>> Patch<TResponse>(
		string url,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<HttpError, TResponse>> Post<TResponse>(
		string url,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}

	public Task<EitherStrict<HttpError, TResponse>> Put<TResponse>(
		string url,
		CancellationToken cancellationToken
	)
	{
		throw new NotImplementedException();
	}
}
