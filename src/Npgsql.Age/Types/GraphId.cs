using System;
using System.Diagnostics.CodeAnalysis;

namespace Npgsql.Age.Types
{
    /// <summary>
    /// Represents the <c>ag_catalog.graphid</c> PostgreSQL type.
    /// </summary>
    public readonly struct GraphId : IComparable, IComparable<GraphId>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GraphId"/>.
        /// </summary>
        /// <param name="value">The internal identifier value.</param>
        public GraphId(ulong value) => Value = value;

        /// <summary>
        /// Internal value of the graphid.
        /// </summary>
        public ulong Value { get; }

        /// <summary>
        /// Compares this <see cref="GraphId"/> with another.
        /// </summary>
        /// <param name="other">The <see cref="GraphId"/> to compare with.</param>
        /// <returns>
        /// A value indicating the relative order: -1 if less, 0 if equal, 1 if greater.
        /// </returns>
        public int CompareTo(GraphId other)
        {
            if (this < other)
                return -1;

            if (this > other)
                return 1;

            return 0;
        }

        /// <summary>
        /// Compares this <see cref="GraphId"/> with an untyped object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>
        /// A value indicating the relative order: -1 if less, 0 if equal, 1 if greater.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="obj"/> is not a <see cref="GraphId"/>.</exception>
        public int CompareTo(object? obj)
        {
            if (obj is null || obj is not GraphId)
                throw new ArgumentException("obj is not a GraphId", nameof(obj));

            return CompareTo((GraphId)obj);
        }

        /// <inheritdoc />
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is null || obj is not GraphId)
                return false;

            var input = (GraphId)obj;

            return this == input;
        }

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Value.ToString();

        #region Operators
        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.
        /// </summary>
        public static bool operator <(GraphId left, GraphId right)
        {
            return left.Value < right.Value;
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.
        /// </summary>
        public static bool operator >(GraphId left, GraphId right)
        {
            return left.Value > right.Value;
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.
        /// </summary>
        public static bool operator <=(GraphId left, GraphId right)
        {
            return left.Value <= right.Value;
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.
        /// </summary>
        public static bool operator >=(GraphId left, GraphId right)
        {
            return left.Value >= right.Value;
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> equals <paramref name="right"/>.
        /// </summary>
        public static bool operator ==(GraphId left, GraphId right)
        {
            return left.Value == right.Value;
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="left"/> does not equal <paramref name="right"/>.
        /// </summary>
        public static bool operator !=(GraphId left, GraphId right)
        {
            return left.Value != right.Value;
        }
        #endregion
    }
}
