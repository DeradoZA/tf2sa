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
			// empty weapons
			if (reader.TokenType == JsonTokenType.EndObject)
			{
				return result;
			}

			// fetch the key designating the weapon
			// true for both cases
			WeaponStats weapon = new();
			if (reader.TokenType == JsonTokenType.PropertyName)
			{
				weapon.WeaponName = reader.GetString();
				reader.Read();
			}

			// for the older case where only the kills is passed - older logs
			if (reader.TokenType == JsonTokenType.Number)
			{
				weapon.Kills = reader.GetInt32();
				result.Add(weapon);
				continue;
			}

			if (reader.TokenType == JsonTokenType.StartObject)
			{
				Dictionary<string, int> weaponDict = new();
				reader.Read();

				//the usual case - parse object into dict
				while (reader.TokenType != JsonTokenType.EndObject)
				{
					string key = reader.GetString()!;
					reader.Read();

					if (reader.TryGetInt32(out int intValue))
					{
						weaponDict.Add(key, intValue);
					}
					else if (reader.TryGetDouble(out double doubleValue))
					{
						weapon.AverageDamage = doubleValue;
					}
					reader.Read();
				}

				if (weaponDict.TryGetValue("kills", out int kills))
				{
					weapon.Kills = kills;
				}
				if (weaponDict.TryGetValue("dmg", out int dmg))
				{
					weapon.Damage = dmg;
				}
				if (weaponDict.TryGetValue("shots", out int shots))
				{
					weapon.Shots = shots;
				}
				if (weaponDict.TryGetValue("hits", out int hits))
				{
					weapon.Hits = hits;
				}
				result.Add(weapon);
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
