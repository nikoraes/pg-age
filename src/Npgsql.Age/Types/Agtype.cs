using Npgsql.Age.Internal.JsonConverters;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Npgsql.Age.Types
{
    /// <summary>
    /// Represent the <c>ag_catalog.agtype</c> PostgreSQL type.
    /// </summary>
    public readonly struct Agtype
    {
        private readonly ReadOnlySequence<byte> _value;

        /// <summary>
        /// The size of the structure in bytes
        /// </summary>
        public long Size => _value.Length;

        /// <summary>
        /// Initialises a new instance of <see cref="Agtype"/>.
        /// </summary>
        /// <param name="value"></param>
        internal Agtype(ReadOnlySequence<byte> value)
        {
            _value = value;
        }

        internal Agtype(string? utf8String) : this(new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(utf8String ?? throw new ArgumentNullException())))
        {
        }

        #region Public methods
        /// <summary>
        /// Return the agtype value as a string.
        /// </summary>
        /// <returns>
        /// String value.
        /// </returns>
        public string GetString() => Get<string>();

        /// <summary>
        /// Return the agtype value as a boolean.
        /// </summary>
        /// <returns>
        /// Boolean value.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the value of the agtype cannot be correctly parsed.
        /// </exception>
        public bool GetBoolean() => Get<bool>();

        /// <summary>
        /// Return the agtype value as a float.
        /// </summary>
        /// <returns>
        /// Float value.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the value of the agtype cannot be correctly parsed.
        /// </exception>
        public float GetFloat() => Get<float>();

        /// <summary>
        /// Return the agtype value as a double.
        /// </summary>
        /// <returns>
        /// Double value.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the value of the agtype cannot be correctly parsed.
        /// </exception>
        public double GetDouble() => Get<double>();

        /// <summary>
        /// Return the agtype value as a byte.
        /// </summary>
        /// <returns>
        /// Byte value.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the value of the agtype cannot be correctly parsed.
        /// </exception>
        public byte GetByte() => Get<byte>();

        /// <summary>
        /// Return the agtype value as an sbyte.
        /// </summary>
        /// <returns>
        /// SByte value.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the value of the agtype cannot be correctly parsed.
        /// </exception>
        public sbyte GetSByte() => Get<sbyte>();

        /// <summary>
        /// Return the agtype value as a short.
        /// </summary>
        /// <returns>
        /// Short value.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the value of the agtype cannot be correctly parsed.
        /// </exception>
        public short GetInt16() => Get<short>();

        /// <summary>
        /// Return the agtype value as a ushort.
        /// </summary>
        /// <returns>
        /// UShort value.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the value of the agtype cannot be correctly parsed.
        /// </exception>
        public ushort GetUInt16() => Get<ushort>();

        /// <summary>
        /// Return the agtype value as an integer.
        /// </summary>
        /// <returns>
        /// Integer value.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the value of the agtype cannot be correctly parsed.
        /// </exception>
        public int GetInt32() => Get<int>();

        /// <summary>
        /// Return the agtype value as a uint.
        /// </summary>
        /// <returns>
        /// UInt value.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the value of the agtype cannot be correctly parsed.
        /// </exception>
        public uint GetUInt32() => Get<uint>();

        /// <summary>
        /// Return the agtype value as a long.
        /// </summary>
        /// <returns>
        /// Long value.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the value of the agtype cannot be correctly parsed.
        /// </exception>
        public long GetInt64() => Get<long>();

        /// <summary>
        /// Return the agtype value as a ulong.
        /// </summary>
        /// <returns>
        /// ULong value.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the value of the agtype cannot be correctly parsed.
        /// </exception>
        public ulong GetUInt64() => Get<ulong>();

        /// <summary>
        /// Return the agtype value as a decimal.
        /// </summary>
        /// <returns>
        /// Decimal value.
        /// </returns>
        public decimal GetDecimal() => Get<decimal>();

        /// <summary>
        /// Return the agtype value as a list.
        /// </summary>
        ///
        /// <remarks>
        /// The list may contain mixed data types.
        /// Example: [1, 2, "string", null].
        /// </remarks>
        /// <returns>
        /// List of objects.
        /// </returns>
        public List<object?> GetList() => Get<List<object?>>();

        /// <summary>
        /// Return true if the agtype is a vertex.
        /// </summary>
        /// <returns>
        /// Boolean value.
        /// </returns>
        public bool IsVertex => EndsWith(Vertex.FOOTER);

        /// <summary>
        /// Return the agtype value as a <see cref="Vertex"/>.
        /// </summary>
        /// <returns>
        /// Vertex.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the agtype cannot be converted to a vertex.
        /// </exception>
        public Vertex<T> GetVertex<T>()
        {
            if (!IsVertex)
                throw new FormatException(
                    "Cannot convert agtype to vertex. Agtype is not a valid vertex."
                );

            return Get<Vertex<T>>();
        }

        /// <summary>
        /// Return the agtype value as a <see cref="Vertex"/>.
        /// </summary>
        /// <returns>
        /// Vertex.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the agtype cannot be converted to a vertex.
        /// </exception>
        public Vertex GetVertex()
        {
            if (!IsVertex)
                throw new FormatException(
                    "Cannot convert agtype to vertex. Agtype is not a valid vertex."
                );

            return Get<Vertex>();
        }

        /// <summary>
        /// Return true if the agtype is an edge.
        /// </summary>
        /// <returns>
        /// Boolean value.
        /// </returns>
        public bool IsEdge => EndsWith(Edge.FOOTER);

        /// <summary>
        /// Return the agtype value as a <see cref="Edge"/>.
        /// </summary>
        /// <returns>
        /// Edge.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the agtype cannot be converted to an edge.
        /// </exception>
        public Edge<T> GetEdge<T>()
        {
            if (!IsEdge)
                throw new FormatException(
                    "Cannot convert agtype to edge. Agtype is not a valid edge."
                );

            return Get<Edge<T>>();
        }

        /// <summary>
        /// Return the agtype value as a <see cref="Edge"/>.
        /// </summary>
        /// <returns>
        /// Edge.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the agtype cannot be converted to an edge.
        /// </exception>
        public Edge GetEdge()
        {
            if (!IsEdge)
                throw new FormatException(
                    "Cannot convert agtype to edge. Agtype is not a valid edge."
                );

            return Get<Edge>();
        }

        /// <summary>
        /// Return true if the agtype is an edge.
        /// </summary>
        /// <returns>
        /// Boolean value.
        /// </returns>
        public bool IsPath => EndsWith(Path.FOOTER);

        /// <summary>
        /// Return the agtype value as a path containing vertices and edges.
        /// </summary>
        /// <returns>
        /// A <see cref="System.IO.Path"/>.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the agtype cannot be converted to a path.
        /// </exception>
        public Path GetPath()
        {
            if (!IsPath)
                throw new FormatException(
                    "Cannot convert agtype to path. Agtype is not a valid path."
                );

            return Get<Path>();
        }

        public T Get<T>()
        {
            using var jsonStream = ToJson(_value);
            return JsonSerializer.Deserialize<T>(jsonStream, SerializerOptions.ReadOptions)!;
        }

        public override string ToString()
        {
            return Encoding.UTF8.GetString(_value);
        }

        public void WriteTo(Stream stream)
        {
            foreach (var memory in _value)
            {
                stream.Write(memory.Span);
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if the agtype represents a null value.
        /// </summary>
        public bool IsNull => _value.Length == 4 && _value.FirstSpan[0] == (byte)'n' && GetLastByte() == (byte)'l';

        /// <summary>
        /// Returns <see langword="true"/> if the agtype is an array.
        /// </summary>
        /// <remarks>
        /// Paths (which also start with <c>[</c>) are not considered arrays because their
        /// string representation ends with the <c>::path</c> footer rather than <c>]</c>.
        /// </remarks>
        public bool IsArray => _value.FirstSpan[0] == (byte)'[' && GetLastByte() == (byte)']';

        /// <summary>
        /// Returns <see langword="true"/> if the agtype is a plain JSON object (map).
        /// </summary>
        public bool IsMap => _value.FirstSpan[0] == (byte)'{' && GetLastByte() == (byte)'}' && !IsVertex && !IsEdge;

        /// <summary>
        /// Returns <see langword="true"/> if the raw agtype value is a JSON string
        /// (enclosed in double quotes).
        /// </summary>
        internal bool IsJsonString => _value.FirstSpan[0] == (byte)'"' && GetLastByte() == (byte)'"';

        /// <summary>
        /// Returns the agtype map as a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <exception cref="FormatException">
        /// Thrown when the agtype is not a map.
        /// </exception>
        public Dictionary<string, object?> GetMap()
        {
            if (!IsMap)
                throw new FormatException(
                    "Cannot convert agtype to map. Agtype is not a valid map."
                );

            return Get<Dictionary<string, object?>>();
        }
        #endregion

        #region Explicit operators
        public static explicit operator byte(Agtype agtype) => agtype.GetByte();

        public static explicit operator sbyte(Agtype agtype) => agtype.GetSByte();

        public static explicit operator short(Agtype agtype) => agtype.GetInt16();

        public static explicit operator ushort(Agtype agtype) => agtype.GetUInt16();

        public static explicit operator int(Agtype agtype) => agtype.GetInt32();

        public static explicit operator uint(Agtype agtype) => agtype.GetUInt32();

        public static explicit operator long(Agtype agtype) => agtype.GetInt64();

        public static explicit operator ulong(Agtype agtype) => agtype.GetUInt64();

        public static explicit operator decimal(Agtype agtype) => agtype.GetDecimal();

        public static explicit operator float(Agtype agtype) => agtype.GetFloat();

        public static explicit operator double(Agtype agtype) => agtype.GetDouble();

        public static explicit operator string(Agtype agtype) => agtype.GetString();

        public static explicit operator List<object?>(Agtype agtype) => agtype.GetList();

        public static explicit operator Vertex(Agtype agtype) => agtype.GetVertex();

        public static explicit operator Edge(Agtype agtype) => agtype.GetEdge();

        public static explicit operator Dictionary<string, object?>(Agtype agtype) =>
            agtype.GetMap();
        #endregion

        private byte GetLastByte()
        {
            if (_value.IsSingleSegment)
                return _value.FirstSpan[^1];
            return _value.Slice(_value.Length - 1, 1).FirstSpan[0];
        }

        private bool EndsWith(string suffix)
        {
            var byteLength = Encoding.UTF8.GetByteCount(suffix);
            if (_value.Length < byteLength)
                return false;
            var actualSuffix = Encoding.UTF8.GetString(_value.Slice(_value.Length - byteLength));
            return actualSuffix == suffix;
        }

        /// <summary>
        /// Create a new object by serializing the data to properly handle
        /// number literals, strings, etc.
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <returns>A new instance of <see cref="Agtype"/></returns>
        public static Agtype Create(object value)
        {
            var ms = new MemoryStream();
            JsonSerializer.Serialize(ms, value, value.GetType(), SerializerOptions.WriteOptions);
            return new Agtype(new ReadOnlySequence<byte>(ms.ToArray()));
        }

        /// <summary>
        /// Converts the Agtype format adhering to the <a href="https://github.com/apache/age/blob/master/drivers/Agtype.g4#L64">grammar</a>
        /// to valid JSON. Special numbers are converted to strings that contain the null character
        /// for easier identification. Special data types (edges, vertices, and paths) are converted
        /// to objects with a <c>$type</c> property.
        /// </summary>
        /// <param name="value">Binary data</param>
        /// <returns>JSON stream</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static Stream ToJson(ReadOnlySequence<byte> value)
        {
            // To avoid unnecessary allocations, byte slices are used wherever feasible.
            var reader = new SequenceReader<byte>(value);
            var depthChanges = new List<int>();
            var depth = 0;
            var segments = new List<ReadOnlySequence<byte>>();

            while (reader.TryPeek(out var next))
            {
                if (char.IsWhiteSpace((char)next))
                {
                    reader.Advance(1);
                    continue;
                }

                var start = reader.Position;
                if (next == '"')
                {
                    reader.Advance(1);
                    if (!reader.TryReadTo(out ReadOnlySequence<byte> _, (byte)'"', (byte)'\\', true))
                        throw new InvalidOperationException();
                    segments.Add(reader.Sequence.Slice(start, reader.Position));
                }
                else if (next == ':' || next == ',')
                {
                    reader.Advance(1);
                    segments.Add(reader.Sequence.Slice(start, reader.Position));
                }
                else if (next == '{' || next == '[')
                {
                    depth++;
                    depthChanges.Add(depth);
                    reader.Advance(1);
                    segments.Add(reader.Sequence.Slice(start, reader.Position));
                }
                else if (next == '}' || next == ']')
                {
                    if (reader.TryPeek(1, out var colon1) && colon1 == ':'
                      && reader.TryPeek(2, out var colon2) && colon2 == ':')
                    {
                        reader.Advance(3);
                        start = reader.Position;
                        var i = 0;
                        while (reader.TryPeek(i, out var c) && char.IsLetter((char)c))
                            i++;
                        reader.Advance(i);
                        var kind = Encoding.UTF8.GetString(reader.Sequence.Slice(start, reader.Position));
                        var index = depthChanges.LastIndexOf(depth);
                        if (index == -1)
                            throw new InvalidOperationException($"Unexpected depth change for closing bracket at position {reader.Position}");

                        var prefix = $@"{{""$type"":""{kind}"",";
                        if (next == ']')
                            prefix += $@"""segments"":[";
                        segments[index] = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(prefix));

                        var suffix = next == ']' ? new[] { (byte)']', (byte)'}' } : new[] { (byte)'}' };
                        segments.Add(new ReadOnlySequence<byte>(suffix));
                    }
                    else
                    {
                        reader.Advance(1);
                        segments.Add(reader.Sequence.Slice(start, reader.Position));
                    }
                    depthChanges.Add(depth);
                    depth--;
                }
                else
                {
                    var i = 0;
                    var hasDigits = false;
                    var hasCapital = false;
                    var isColon = false;
                    while (reader.TryPeek(i, out var c))
                    {
                        isColon = c == ':' && reader.TryPeek(i + 1, out var c1) && c1 == ':';
                        if (char.IsWhiteSpace((char)c) || c == '}' || c == ']' || c == ',' || isColon)
                            break;

                        hasCapital = hasCapital || char.IsUpper((char)c);
                        hasDigits = hasDigits || char.IsDigit((char)c);
                        i++;
                    }

                    if (isColon)
                    {
                        reader.Advance(i);
                        var number = Encoding.UTF8.GetString(reader.Sequence.Slice(start, reader.Position));
                        reader.Advance(2);
                        start = reader.Position;
                        i = 0;
                        while (reader.TryPeek(i, out var c) && char.IsLetter((char)c))
                            i++;
                        reader.Advance(i);
                        var kind = Encoding.UTF8.GetString(reader.Sequence.Slice(start, reader.Position));
                        segments.Add(new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes($@"""{number}\u0000{kind}""")));
                    }
                    else if (hasCapital && !hasDigits)
                    {
                        reader.Advance(i);
                        var text = Encoding.UTF8.GetString(reader.Sequence.Slice(start, reader.Position));
                        if (string.Equals(text, "NaN", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(text, "Infinity", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(text, "-Infinity", StringComparison.OrdinalIgnoreCase))
                        {
                            segments.Add(new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes($@"""\u0000{text}""")));
                        }
                        else if (string.Equals(text, "true", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(text, "false", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(text, "null", StringComparison.OrdinalIgnoreCase))
                        {
                            segments.Add(new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(text.ToLowerInvariant())));
                        }
                        else
                        {
                            segments.Add(reader.Sequence.Slice(start, reader.Position));
                        }
                    }
                    else
                    {
                        reader.Advance(i);
                        segments.Add(reader.Sequence.Slice(start, reader.Position));
                    }
                }

                if (depthChanges.Count < segments.Count)
                    depthChanges.Add(-1);
            }

            var ms = new MemoryStream(segments.Sum(s => (int)s.Length));
            foreach (var segment in segments)
            {
                foreach (var memory in segment)
                {
                    ms.Write(memory.Span);
                }
            }
            ms.Position = 0;
            return ms;
        }
    }
}
