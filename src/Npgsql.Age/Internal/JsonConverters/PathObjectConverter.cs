using Npgsql.Age.Types;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Npgsql.Age.Internal.JsonConverters
{
    /// <summary>
    /// A custom converter to properly serialize a JSON path.
    /// Deserialization is handled through a combination of <see cref="Agtype.ToJson(System.Buffers.ReadOnlySequence{byte})"/>
    /// and <see cref="SerializerOptions.WriteOptions"/>
    /// </summary>
    internal class PathObjectConverter : JsonConverter<Path>
    {
        public override Path Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, Path value, JsonSerializerOptions options)
        {
            var code = JsonSerializer.Serialize(value.Segments, SerializerOptions.WriteOptions);
            writer.WriteRawValue(code + Path.FOOTER, true);
        }
    }
}
