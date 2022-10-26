using System.Text.Json;
using TF2SA.Http.LogsTF.Models.GameLogModel;

namespace TF2SA.Http.LogsTF.Serialization;

public static class LogsTFSerializer
{
    public static GameLog? DeserializeGameLog(string json) // use monads here.
    {
        JsonSerializerOptions jsonOptions =
            new() { PropertyNameCaseInsensitive = true };
        var serialized = JsonSerializer.Deserialize<GameLog>(json, jsonOptions);
        return serialized;
    }
}
