using Npgsql.Age.Types;
using Npgsql.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Age.Internal
{
#pragma warning disable NPG9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    internal class AgtypeConverter : PgStreamingConverter<Agtype>
    {
        public override bool CanConvert(
            DataFormat format,
            out BufferRequirements bufferRequirements
        )
        {
            bufferRequirements = BufferRequirements.None;
            return format is DataFormat.Text || format is DataFormat.Binary;
        }

        public override Size GetSize(SizeContext context, Agtype value, ref object? writeState)
        {
            return (int)value.Size + 1;
        }

        /// <summary>
        /// Read agtype from its binary representation.
        /// Apache AGE expects: version byte (1) + text content
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public override Agtype Read(PgReader reader)
        {
            // Read the version byte (should be 1)
            byte version = reader.ReadByte();
            if (version != 1)
            {
                throw new NotSupportedException($"Unsupported agtype version number {version}");
            }

            // Read the remaining text content
            return new Agtype(reader.ReadBytes(reader.CurrentRemaining));
        }

        /// <summary>
        /// Asynchronously read agtype from its binary representation.
        /// Apache AGE expects: version byte (1) + text content
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async ValueTask<Agtype> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        {
            byte version = reader.ReadByte();
            if (version != 1)
            {
                throw new NotSupportedException($"Unsupported agtype version number {version}");
            }

            var bytes = await reader.ReadBytesAsync(reader.CurrentRemaining, cancellationToken);
            return new Agtype(bytes);
        }

        /// <summary>
        /// Write agtype to its binary representation.
        /// Apache AGE format: version byte (1) + text content
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public override void Write(PgWriter writer, Agtype value)
        {
            // Write version number as first byte (version 1)
            writer.WriteByte(1);

            using (var stream = writer.GetStream())
            {
                value.WriteTo(stream);
            }
        }

        /// <summary>
        /// Asynchronously write agtype to its binary representation.
        /// Apache AGE format: version byte (1) + text content
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        public override async ValueTask WriteAsync(PgWriter writer, Agtype value, CancellationToken cancellationToken = default)
        {
            writer.WriteByte(1);

            await using (var stream = writer.GetStream(allowMixedIO: true))
            {
                await value.WriteToAsync(stream, cancellationToken);
            }
        }
    }
#pragma warning restore NPG9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
