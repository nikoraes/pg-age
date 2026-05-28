using Npgsql.Age.Types;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Npgsql.Age.Internal.JsonConverters
{
    /// <summary>
    /// A custom converter to infer object types from their JSON token type.
    /// <para>
    /// For example, numbers in the json will be returned as a valid number type
    /// in C# (<see langword="int"/>, <see langword="decimal"/>, or
    /// <see langword="double"/>).
    /// </para>
    /// </summary>
    internal class InferredObjectConverter : JsonConverter<object>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
                return true;
            else if (reader.TokenType == JsonTokenType.False)
                return false;
            else if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var integer))
                return integer;
            else if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var @long))
                return @long;
            else if (reader.TokenType == JsonTokenType.Number && reader.TryGetDecimal(out var @decimal))
                return @decimal;
            else if (reader.TokenType == JsonTokenType.Number)
                return reader.GetDouble();
            else if (reader.TokenType == JsonTokenType.String)
            {
                var text = reader.GetString()!;
                return SerializerOptions.TryGetNumberFromString<object>(text, v => decimal.Parse(v), out var num) ? num : text;
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
                return JsonSerializer.Deserialize<List<object>>(ref reader, options);
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                var readerClone = reader;
                readerClone.Read();
                if (readerClone.TokenType == JsonTokenType.PropertyName && readerClone.GetString() == "$type")
                {
                    readerClone.Read();
                    if (readerClone.TokenType == JsonTokenType.String)
                    {
                        var type = readerClone.GetString();
                        if (type == "edge")
                            return JsonSerializer.Deserialize<Edge<Dictionary<string, object>>>(ref reader, options);
                        else if (type == "vertex")
                            return JsonSerializer.Deserialize<Vertex<Dictionary<string, object>>>(ref reader, options);
                        else if (type == "path")
                            return JsonSerializer.Deserialize<Path>(ref reader, options);
                    }
                }
                return JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options);
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
