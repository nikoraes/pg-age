using Npgsql.Age.Types;
using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Npgsql.Age.Internal.JsonConverters
{
    internal class EntityConverterFactory : JsonConverterFactory
    {
        private static JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new GraphIdConverter(),
                new DoubleConverter(),
                new FloatConverter(),
                new DecimalConverter(),
            },
        };

        public override bool CanConvert(Type typeToConvert)
        {
            return TryGetVertexEdge(typeToConvert, out var _)
                || (typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Entity<>));
        }

        private static bool TryGetVertexEdge(Type typeToConvert, out bool isVertex)
        {
            var currType = typeToConvert;
            while (currType != null)
            {
                if (currType.IsGenericType)
                {
                    if (currType.GetGenericTypeDefinition() == typeof(Vertex<>))
                    {
                        isVertex = true;
                        return true;
                    }
                    else if (currType.GetGenericTypeDefinition() == typeof(Edge<>))
                    {
                        isVertex = false;
                        return true;
                    }
                }
                currType = currType.BaseType;
            }
            isVertex = false;
            return false;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converter = (JsonConverter)Activator.CreateInstance(
                typeof(EntityConverter<>).MakeGenericType(typeToConvert),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: Array.Empty<object>(),
                culture: null)!;

            return converter;
        }

        private class EntityConverter<T> : JsonConverter<T>
        {
            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotSupportedException();
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                var type = value!.GetType();
                if (!TryGetVertexEdge(type, out var isVertex))
                    throw new InvalidOperationException();

                var code = JsonSerializer.Serialize(value, type, _options);
                if (isVertex)
                    code += Vertex.FOOTER;
                else
                    code += Edge.FOOTER;
                writer.WriteRawValue(code, true);
            }
        }
    }
}
