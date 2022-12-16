using System.Text.Json;
using Monad;
using TF2SA.Http.Base.Errors;

namespace TF2SA.Http.Base.Serialization;

public class TF2SAJsonSerializer : IJsonSerializer
{
	private static readonly JsonSerializerOptions jsonOptions =
		new() { PropertyNameCaseInsensitive = true };

	public EitherStrict<
		SerializationError,
		TDeserialized
	> Deserialize<TDeserialized>(string json)
	{
		try
		{
			TDeserialized? dserialized =
				JsonSerializer.Deserialize<TDeserialized>(json, jsonOptions);
			if (dserialized is null)
			{
				return new SerializationError("Derialized result is null");
			}
			return dserialized;
		}
		catch (Exception e)
		{
			return new SerializationError(e.Message);
		}
	}

	public EitherStrict<SerializationError, string> Serialize<TSerialized>(
		TSerialized obj
	)
	{
		throw new NotImplementedException();
	}
}
