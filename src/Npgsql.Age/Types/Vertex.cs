using Npgsql.Age.Internal.JsonConverters;
using System.Collections.Generic;

namespace Npgsql.Age.Types
{
    /// <summary>
    /// Represents a typed vertex in the graph.
    /// </summary>
    /// <typeparam name="T">The type of the vertex's properties.</typeparam>
    /// <param name="Id">The unique identifier of the vertex.</param>
    /// <param name="Label">The label of the vertex.</param>
    /// <param name="Properties">The properties of the vertex.</param>
    public record Vertex<T>(GraphId Id, string Label, T Properties)
    : Entity<T>(Id, Label, Properties)
    {
        /// <inheritdoc />
        public override string ToString()
        {
            return SerializerOptions.Serialize(this);
        }
    }

    /// <summary>
    /// Represents a vertex in the graph with dictionary-based properties.
    /// </summary>
    /// <param name="Id">The unique identifier of the vertex.</param>
    /// <param name="Label">The label of the vertex.</param>
    /// <param name="Properties">The properties of the vertex.</param>
    public record Vertex(GraphId Id, string Label, Dictionary<string, object> Properties)
    : Vertex<Dictionary<string, object>>(Id, Label, Properties)
    {
        /// <summary>
        /// The suffix appended to the JSON representation by Apache AGE to identify this as a vertex.
        /// </summary>
        public const string FOOTER = "::vertex";

        /// <inheritdoc />
        public override string ToString()
        {
            return SerializerOptions.Serialize(this);
        }
    }
}
