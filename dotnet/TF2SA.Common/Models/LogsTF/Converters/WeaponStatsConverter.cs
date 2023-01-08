using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using TF2SA.Common.Models.LogsTF.GameLogModel;

namespace TF2SA.Common.Models.LogsTF.Converters;

public class WeaponStatsConverter : JsonConverter<List<WeaponStats>?>
{
	public override List<WeaponStats>? Read(
		ref Utf8JsonReader reader,
		Type typeToConvert,
		JsonSerializerOptions options
	)
	{
		List<WeaponStats> result = new();

		while (reader.Read())
		{
			WeaponStats weapon = new();
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				string? name = reader.GetString();
				reader.Read();

				weapon.WeaponName = name;
			}
			if (reader.TokenType == JsonTokenType.Number)
			{
				weapon.Kills = reader.GetInt32();

				result.Add(weapon);
				continue;
			}
		}

		return result;
	}

	public override void Write(
		Utf8JsonWriter writer,
		List<WeaponStats>? value,
		JsonSerializerOptions options
	)
	{
		throw new NotImplementedException();
	}
}
