using Npgsql.Age.Internal.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Npgsql.Age.Types
{
    public record Path
    {
        internal const string FOOTER = "::path";

        public IReadOnlyList<Entity<Dictionary<string, object>>> Segments { get; init; }

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


        public Path(IReadOnlyList<Entity<Dictionary<string, object>>> segments)
        {
            CheckPath(segments);
            Segments = segments;
        }

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
