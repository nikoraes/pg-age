using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Npgsql.Age.Internal.JsonConverters
{
    internal class DecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetDecimal();
            else if (reader.TokenType == JsonTokenType.String
              && SerializerOptions.TryGetNumberFromString<decimal>(reader.GetString()!, decimal.Parse, out var num))
            {
                return num;
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteRawValue(value.ToString() + "::numeric", true);
        }
    }
}
