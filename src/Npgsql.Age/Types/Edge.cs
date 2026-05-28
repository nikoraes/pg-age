using Npgsql.Age.Internal.JsonConverters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Npgsql.Age.Types
{
    public record Edge<T>(GraphId Id, [property: JsonPropertyName("start_id")] GraphId StartId, [property: JsonPropertyName("end_id")] GraphId EndId, string Label, T Properties)
    : Entity<T>(Id, Label, Properties)
    {
        public override string ToString()
        {
            return SerializerOptions.Serialize(this);
        }
    }

    public record Edge(GraphId Id, GraphId StartId, GraphId EndId, string Label, Dictionary<string, object> Properties)
    : Edge<Dictionary<string, object>>(Id, StartId, EndId, Label, Properties)
    {
        public const string FOOTER = "::edge";

        public override string ToString()
        {
            return SerializerOptions.Serialize(this);
        }
    }
}
