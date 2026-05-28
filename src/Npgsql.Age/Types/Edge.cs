using Npgsql.Age.Internal.JsonConverters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Npgsql.Age.Types
{
    /// <summary>
    /// Represents a typed edge (relationship) in the graph.
    /// </summary>
    /// <typeparam name="T">The type of the edge's properties.</typeparam>
    /// <param name="Id">The unique identifier of the edge.</param>
    /// <param name="StartId">The identifier of the source vertex.</param>
    /// <param name="EndId">The identifier of the target vertex.</param>
    /// <param name="Label">The label of the edge.</param>
    /// <param name="Properties">The properties of the edge.</param>
    public record Edge<T>(GraphId Id, [property: JsonPropertyName("start_id")] GraphId StartId, [property: JsonPropertyName("end_id")] GraphId EndId, string Label, T Properties)
    : Entity<T>(Id, Label, Properties)
    {
        /// <inheritdoc />
        public override string ToString()
        {
            return SerializerOptions.Serialize(this);
        }
    }

    /// <summary>
    /// Represents an edge (relationship) in the graph with dictionary-based properties.
    /// </summary>
    /// <param name="Id">The unique identifier of the edge.</param>
    /// <param name="StartId">The identifier of the source vertex.</param>
    /// <param name="EndId">The identifier of the target vertex.</param>
    /// <param name="Label">The label of the edge.</param>
    /// <param name="Properties">The properties of the edge.</param>
    public record Edge(GraphId Id, GraphId StartId, GraphId EndId, string Label, Dictionary<string, object> Properties)
    : Edge<Dictionary<string, object>>(Id, StartId, EndId, Label, Properties)
    {
        /// <summary>
        /// The suffix appended to the JSON representation by Apache AGE to identify this as an edge.
        /// </summary>
        public const string FOOTER = "::edge";

        /// <inheritdoc />
        public override string ToString()
        {
            return SerializerOptions.Serialize(this);
        }
    }
}
