using System.Text.Json;
using Monad;
using TF2SA.Http.Errors;
using TF2SA.Http.LogsTF.Models.GameLogModel;

namespace TF2SA.Http.LogsTF.Serialization;

public static class LogsTFSerializer<T>
{
	private static readonly JsonSerializerOptions jsonOptions =
		new() { PropertyNameCaseInsensitive = true };
	public static EitherStrict<SerializationError, T> Deserialize(string json)
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
