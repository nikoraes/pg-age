using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Npgsql.Age.Internal.JsonConverters
{
    internal class DoubleConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetDouble();
            else if (reader.TokenType == JsonTokenType.String
              && SerializerOptions.TryGetNumberFromString<double>(reader.GetString()!, double.Parse, out var num))
            {
                return num;
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
#pragma warning disable JSON001 // Invalid JSON pattern
            if (value == double.PositiveInfinity)
                writer.WriteRawValue("Infinity", true);
            else if (value == double.NegativeInfinity)
                writer.WriteRawValue("-Infinity", true);
            else if (double.IsNaN(value))
                writer.WriteRawValue("NaN", true);
#pragma warning restore JSON001 // Invalid JSON pattern
            else
                writer.WriteNumberValue(value);
        }
    }
}
