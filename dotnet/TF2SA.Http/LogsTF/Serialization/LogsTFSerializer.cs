using System.Text.Json;
using Monad;
using TF2SA.Http.Errors;

namespace TF2SA.Http.LogsTF.Serialization;

public static class LogsTFSerializer
{
	private static readonly JsonSerializerOptions jsonOptions =
		new() { PropertyNameCaseInsensitive = true };

	public static EitherStrict<SerializationError, T> Deserialize<T>(string json)
	{
		try
		{
			T? serialized = JsonSerializer.Deserialize<T>(json, jsonOptions);
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
}
