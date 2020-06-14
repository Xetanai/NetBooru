using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace NetBooru.MagicTool
{

    public class Reader
    {
        private static readonly byte[] FileHeader
            = Encoding.UTF8.GetBytes("MIME-Magic")
                .Append((byte)0)
                .ToArray();

        public static bool TryRead(
            bool isFileStart,
            ref ReadOnlySequence<byte> buffer,
            out ReadOnlySequence<byte> value)
        {
            var reader = new SequenceReader<byte>(buffer);

            while (reader.TryReadTo(out ReadOnlySequence<byte> read,
                (byte)'\n'))
            {
                if (isFileStart)
                {
                    value = read;
                    buffer = buffer.Slice(reader.Position);
                    return true;
                }
                else
                {
                    if (IsMimeTypeDeclaration(read))
                    {
                        var position = ReadToNextMimeTypeDeclaration(
                            ref reader);
                        value = buffer.Slice(read.Start, position);
                        buffer = buffer.Slice(position);
                        return true;
                    }
                }
            }

            value = ReadOnlySequence<byte>.Empty;
            return false;

            static SequencePosition ReadToNextMimeTypeDeclaration(
                ref SequenceReader<byte> reader)
            {
                while (reader.TryReadTo(out ReadOnlySequence<byte> value,
                    (byte)'\n'))
                {
                    if (IsMimeTypeDeclaration(value))
                        return value.Start;
                }

                return reader.Position;
            }
        }

        public static bool TryProcess(ref bool isFileStart,
            ReadOnlySequence<byte> data,
            out MimeType? mimeType)
        {
            if (isFileStart)
            {
                isFileStart = false;
                Span<byte> tmp = stackalloc byte[FileHeader.Length];
                data.CopyTo(tmp);

                mimeType = default;
                return tmp.SequenceEqual(FileHeader);
            }
            else if (data.IsSingleSegment)
            {
                return TryReadSection(data.FirstSpan, out mimeType);
            }
            else
            {
                using var memory = MemoryPool<byte>.Shared.Rent(
                    (int)data.Length);

                var span = memory.Memory.Span.Slice(0, (int)data.Length);

                data.CopyTo(span);

                return TryReadSection(span, out mimeType);
            }

            static bool TryReadSection(ReadOnlySpan<byte> span,
                [NotNullWhen(true)]
                out MimeType? mimeType)
            {
                if (!TryReadMimeType(ref span, out var priority,
                    out var name))
                {
                    mimeType = default;
                    return false;
                }

                mimeType = new MimeType
                {
                    Priority = priority,
                    Name = name
                };

                do
                {
                    if (!TryReadPattern(ref span, out var indent,
                        out var pattern))
                        return false;

                    var patterns = mimeType.Patterns;
                    for (int x = 0; x < indent; x++)
                        patterns = patterns.Last().Children;

                    patterns.Add(pattern);
                } while (!span.IsEmpty);

                return true;
            }
        }

        private static bool TryReadPattern(ref ReadOnlySpan<byte> span,
            out int indent,
            [NotNullWhen(true)]
            out MimeTypePattern? pattern)
        {
            indent = default;
            pattern = default;

            if (Utf8Parser.TryParse(span, out indent, out int bytesConsumed))
                span = span[bytesConsumed..];

            bool hasStartOffset = false, hasValue = false;
            var startOffset = 0;
            var value = ReadOnlySpan<byte>.Empty;
            var mask = ReadOnlySpan<byte>.Empty;
            var wordSize = 1;
            var rangeLength = 1;

            while (span.Length > 0)
            {
                switch (span[0])
                {
                    case (byte)'>':
                        hasStartOffset = true;
                        if (!Utf8Parser.TryParse(span[1..], out startOffset,
                            out bytesConsumed))
                            return false;

                        span = span.Slice(1 + bytesConsumed);
                        break;
                    case (byte)'=':
                        hasValue = true;
                        if (!BinaryPrimitives.TryReadInt16BigEndian(
                            span[1..], out var length))
                            return false;

                        value = span.Slice(3, length);
                        span = span.Slice(3 + length);
                        break;
                    case (byte)'&':
                        value = span.Slice(1, value.Length);
                        span = span.Slice(1 + value.Length);
                        break;
                    case (byte)'~':
                        if (!Utf8Parser.TryParse(span[1..], out wordSize,
                            out bytesConsumed))
                            return false;

                        span = span.Slice(1 + bytesConsumed);
                        break;
                    case (byte)'+':
                        if (!Utf8Parser.TryParse(span[1..], out rangeLength,
                            out bytesConsumed))
                            return false;

                        span = span.Slice(1 + bytesConsumed);
                        break;
                    case (byte)'\n':
                        span = span.Slice(1);

                        if (!(hasStartOffset && hasValue))
                            return false;

                        if (!value.IsEmpty && mask.IsEmpty)
                        {
                            var tmp = new byte[value.Length].AsSpan();
                            tmp.Fill(byte.MaxValue);

                            mask = tmp;
                        }

                        pattern = new MimeTypePattern
                        {
                            OffsetIntoFile = startOffset,
                            Value = value.ToArray(),
                            Mask = mask.ToArray(),
                            WordSize = wordSize,
                            RangeLength = rangeLength
                        };
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

        private static bool TryReadMimeType(ref ReadOnlySpan<byte> span,
            out int priority,
            [NotNullWhen(true)]
            out string? name)
        {
            priority = default;
            name = default;

            if (span.IsEmpty)
                return false;

            if (span[0] != '[')
                return false;

            if (!Utf8Parser.TryParse(span[1..], out priority,
                out int bytesRead))
                return false;

            span = span.Slice(1 + bytesRead);

            if (span[0] != ':')
                return false;

            var newline = span.IndexOf((byte)'\n');

            if (newline < 0)
                return false;

            if (span[newline - 1] != ']')
                return false;

            name = Encoding.UTF8.GetString(span[1..(newline - 1)]);

            span = span.Slice(newline + 1);
            return true;
        }

        private static bool IsMimeTypeDeclaration(
            ReadOnlySequence<byte> sequence)
        {
            if (sequence.IsEmpty)
                return false;

            if (sequence.IsSingleSegment)
            {
                var span = sequence.FirstSpan.TrimEnd((byte)'\n');

                if (span[0] != (byte)'[' || span[^1] != (byte)']')
                    return false;

                var valueIndex = span.IndexOf((byte)':');

                if (valueIndex < 0)
                    return false;

                if (!Utf8Parser.TryParse(span[1..valueIndex], out int _,
                    out int _))
                    return false;

                return true;
            }
            else
            {
                Span<byte> tmp = stackalloc byte[(int)sequence.Length];
                sequence.CopyTo(tmp);

                tmp = tmp.TrimEnd((byte)'\n');

                if (tmp[0] != (byte)'[' || tmp[^1] == (byte)']')
                    return false;

                var valueIndex = tmp.IndexOf((byte)':');

                if (valueIndex < 0)
                    return false;

                if (!Utf8Parser.TryParse(tmp[1..valueIndex], out int _,
                    out int _))
                    return false;

                return true;
            }
        }
    }
}
