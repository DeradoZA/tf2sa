using Monad;
using TF2SA.Http.Base.Errors;

namespace TF2SA.Http.Base.Client;

public interface IHttpClient
{
	public Task<EitherStrict<HttpError, TResponse>> Get<TResponse>(
		string url,
		CancellationToken cancellationToken
	);
	public Task<EitherStrict<HttpError, TResponse>> Post<TResponse>(
		string url,
		CancellationToken cancellationToken
	);
	public Task<EitherStrict<HttpError, TResponse>> Put<TResponse>(
		string url,
		CancellationToken cancellationToken
	);
	public Task<EitherStrict<HttpError, TResponse>> Patch<TResponse>(
		string url,
		CancellationToken cancellationToken
	);
	public Task<EitherStrict<HttpError, TResponse>> Delete<TResponse>(
		string url,
		CancellationToken cancellationToken
	);
}
