using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Npgsql.Age.Internal.JsonConverters
{
    internal class FloatConverter : JsonConverter<float>
    {
        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetSingle();
            else if (reader.TokenType == JsonTokenType.String
              && SerializerOptions.TryGetNumberFromString<float>(reader.GetString()!, float.Parse, out var num))
            {
                return num;
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
        {
#pragma warning disable JSON001 // Invalid JSON pattern
            if (value == float.PositiveInfinity)
                writer.WriteRawValue("Infinity", true);
            else if (value == float.NegativeInfinity)
                writer.WriteRawValue("-Infinity", true);
            else if (float.IsNaN(value))
                writer.WriteRawValue("NaN", true);
#pragma warning restore JSON001 // Invalid JSON pattern
            else
                writer.WriteNumberValue(value);
        }
    }
}
