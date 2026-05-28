using Npgsql.Age.Types;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Npgsql.Age.Internal.JsonConverters
{
    /// <summary>
    /// A custom converter to properly serialize a JSON path.
    /// </summary>
    internal class PathObjectConverter : JsonConverter<Path>
    {
        public override Path Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            var segments = new List<Entity<Dictionary<string, object>>>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException();

                var propertyName = reader.GetString();
                if (propertyName == "$type")
                {
                    reader.Skip();
                }
                else if (propertyName == "segments")
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.StartArray)
                        throw new JsonException();

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                            break;

                        var segment = JsonSerializer.Deserialize<Entity<Dictionary<string, object>>>(ref reader, options);
                        if (segment != null)
                            segments.Add(segment);
                    }
                }
                else
                {
                    reader.Skip();
                }
            }

            return new Path(segments);
        }

        public override void Write(Utf8JsonWriter writer, Path value, JsonSerializerOptions options)
        {
            var code = JsonSerializer.Serialize(value.Segments, SerializerOptions.WriteOptions);
            writer.WriteRawValue(code + Path.FOOTER, true);
        }
    }
}
