using Npgsql.Age.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Npgsql.Age.Internal.JsonConverters
{
    internal static class SerializerOptions
    {
        internal static readonly JsonSerializerOptions ReadOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new GraphIdConverter(),
                new FloatConverter(),
                new DoubleConverter(),
                new DecimalConverter(),
                new IntegerConverter(),
                new InferredObjectConverter(),
            },
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    ti =>
                    {
                        if (ti.Type.IsGenericType && ti.Type.GetGenericTypeDefinition() == typeof(Entity<>))
                        {
                            var isDict = ti.Type.GenericTypeArguments[0] == typeof(Dictionary<string, object>);
                            ti.PolymorphismOptions = new JsonPolymorphismOptions
                            {
                                TypeDiscriminatorPropertyName = "$type",
                                IgnoreUnrecognizedTypeDiscriminators = true,
                                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
                                DerivedTypes =
                                {
                                    new JsonDerivedType(isDict ? typeof(Vertex) : typeof(Vertex<>).MakeGenericType(ti.Type.GenericTypeArguments), "vertex"),
                                    new JsonDerivedType(isDict ? typeof(Edge) : typeof(Edge<>).MakeGenericType(ti.Type.GenericTypeArguments), "edge"),
                                }
                            };
                        }
                    }
                }
            }
        };

        internal static readonly JsonSerializerOptions WriteOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new GraphIdConverter(),
                new PathObjectConverter(),
                new EntityConverterFactory(),
                new FloatConverter(),
                new DoubleConverter(),
                new DecimalConverter(),
            },
        };

        internal static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, value.GetType(), WriteOptions);
        }

        internal static bool TryGetNumberFromString<T>(string text, Func<string, T> parser, out T number)
        {
            var type = typeof(T);
            if (type == typeof(object))
                type = typeof(double);
            if (string.Equals(text, "\u0000NaN", StringComparison.OrdinalIgnoreCase))
            {
                number = type switch
                {
                    not null when type == typeof(double) => (T)(object)double.NaN,
                    not null when type == typeof(float) => (T)(object)float.NaN,
                    _ => default!
                };
                return type == typeof(double) || type == typeof(float);
            }
            else if (string.Equals(text, "\u0000Infinity", StringComparison.OrdinalIgnoreCase))
            {
                number = type switch
                {
                    not null when type == typeof(double) => (T)(object)double.PositiveInfinity,
                    not null when type == typeof(float) => (T)(object)float.PositiveInfinity,
                    _ => default!
                };
                return type == typeof(double) || type == typeof(float);
            }
            else if (string.Equals(text, "\u0000-Infinity", StringComparison.OrdinalIgnoreCase))
            {
                number = type switch
                {
                    not null when type == typeof(double) => (T)(object)double.NegativeInfinity,
                    not null when type == typeof(float) => (T)(object)float.NegativeInfinity,
                    _ => default!
                };
                return type == typeof(double) || type == typeof(float);
            }
            else if (text?.EndsWith("\u0000numeric") == true)
            {
                number = parser(text.Substring(0, text.Length - "\u0000numeric".Length));
                return true;
            }
            else
            {
                number = default!;
                return false;
            }
        }
    }
}
