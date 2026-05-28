using Npgsql.Age.Internal.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Npgsql.Age.Types
{
    /// <summary>
    /// Represents a path returned by Apache AGE, consisting of alternating vertices and edges.
    /// </summary>
    /// <remarks>
    /// A path is a sequence of segments where vertices and edges alternate:
    /// <c>vertex - edge - vertex - edge - ... - vertex</c>.
    /// The first and last segments are always vertices.
    /// </remarks>
    public record Path
    {
        /// <summary>
        /// The suffix appended to the JSON representation by Apache AGE to identify this as a path.
        /// </summary>
        public const string FOOTER = "::path";

        /// <summary>
        /// The segments of the path, alternating between vertices and edges.
        /// </summary>
        /// <remarks>
        /// Use <see cref="Vertices"/> and <see cref="Edges"/> for typed access.
        /// </remarks>
        public IReadOnlyList<Entity<Dictionary<string, object>>> Segments { get; private init; }

        /// <summary>
        /// The length of the path.
        /// </summary>
        /// <remarks>
        /// Equal to the number of edges.
        /// </remarks>
        public int Length => Segments.Count / 2;

        /// <summary>
        /// Vertices in the path (in order).
        /// </summary>
        public IEnumerable<Vertex> Vertices => Segments.OfType<Vertex>();

        /// <summary>
        /// Edges in the path.
        /// </summary>
        /// <remarks>
        /// Edge with index 0 is the edge between vertices 0 and 1. Edge 1
        /// connects vertices 1 and 2, and so on.
        /// </remarks>
        public IEnumerable<Edge> Edges => Segments.OfType<Edge>();


        /// <summary>
        /// Initialises a new instance of <see cref="Path"/>.
        /// </summary>
        /// <param name="segments">The segments of the path, alternating vertex, edge, vertex, etc.</param>
        /// <exception cref="FormatException">Thrown when the path format is invalid.</exception>
        public Path(IReadOnlyList<Entity<Dictionary<string, object>>> segments)
        {
            CheckPath(segments);
            Segments = segments;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return SerializerOptions.Serialize(this);
        }

        private static void CheckPath(IEnumerable<Entity<Dictionary<string, object>>> path)
        {
            var i = 0;
            foreach (var segment in path)
            {
                var shouldBeVertex = i % 2 == 0;
                if (shouldBeVertex && !(segment is Vertex<Dictionary<string, object>>))
                {
                    throw new FormatException("Invalid path");
                }
                else if (!shouldBeVertex && !(segment is Edge<Dictionary<string, object>>))
                {
                    throw new FormatException("Invalid path");
                }
                i++;
            }
        }
    }
}
