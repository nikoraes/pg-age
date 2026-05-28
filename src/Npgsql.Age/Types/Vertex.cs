using Npgsql.Age.Internal.JsonConverters;
using System.Collections.Generic;

namespace Npgsql.Age.Types
{
    public record Vertex<T>(GraphId Id, string Label, T Properties)
    : Entity<T>(Id, Label, Properties)
    {
        public override string ToString()
        {
            return SerializerOptions.Serialize(this);
        }
    }

    public record Vertex(GraphId Id, string Label, Dictionary<string, object> Properties)
    : Vertex<Dictionary<string, object>>(Id, Label, Properties)
    {
        public const string FOOTER = "::vertex";

        public override string ToString()
        {
            return SerializerOptions.Serialize(this);
        }
    }
}
