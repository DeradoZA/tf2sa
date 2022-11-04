using System.Text.Json;
using Monad;
using TF2SA.Http.Base.Errors;

namespace TF2SA.Http.Base.Serialization;

public class TF2SAJsonSerializer : IJsonSerializer
{
	private static readonly JsonSerializerOptions jsonOptions =
		new() { PropertyNameCaseInsensitive = true };

	public EitherStrict<SerializationError, TDeserialized> Deserialize<TDeserialized>(string json)
	{
		try
		{
			TDeserialized? serialized = JsonSerializer.Deserialize<TDeserialized>(json, jsonOptions);
			if (serialized is null)
			{
				return new SerializationError("Serialized result is null");
			}
			return serialized;
		}
		catch (Exception e)
		{
			return new SerializationError(e.Message);
		}
	}

	public EitherStrict<SerializationError, TSerialized> Serialize<TSerialized>(string json)
	{
		throw new NotImplementedException();
	}
}
