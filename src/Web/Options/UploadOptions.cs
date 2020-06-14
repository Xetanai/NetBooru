using System;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Text;

namespace NetBooru.Web.Options
{
    public class UploadOptions
    {
        public uint MaxFileSize { get; set; }

        public MimeType[] MimeTypeDatabase { get; set; } = null!;

        public string UploadLocation { get; set; } = null!;

        public int DirectorySeparationBytes { get; set; }

        public class MimeType
        {
            public string Name { get; set; } = null!;

            public MimeTypePattern[] Patterns { get; set; } = null!;
        }

        public class MimeTypePattern
        {
            public int OffsetIntoFile { get; set; }

            public string Mask { get; set; } = null!;
            public string Value { get; set; } = null!;

            public int WordSize { get; set; }

            public int RangeLength { get; set; }

            public MimeTypePattern[] Children { get; set; } = null!;


            private byte[]? _maskBytes;
            private byte[]? _valueBytes;

            public byte[] GetMaskAsBytes()
            {
                if (_maskBytes != null)
                    return _maskBytes;

                var mask = Encoding.UTF8.GetBytes(Mask);

                var maxLength = Base64.GetMaxDecodedFromUtf8Length(mask.Length);
                var buffer = new byte[maxLength];

                var status = Base64.DecodeFromUtf8(mask, buffer, out int _,
                    out int length);

                Debug.Assert(status == OperationStatus.Done);

                Array.Resize(ref buffer, length);

                return _maskBytes = buffer;
            }

            public byte[] GetValueAsBytes()
            {
                if (_valueBytes != null)
                    return _valueBytes;

                var value = Encoding.UTF8.GetBytes(Value);

                var maxLength = Base64.GetMaxDecodedFromUtf8Length(
                    value.Length);
                var buffer = new byte[maxLength];

                var status = Base64.DecodeFromUtf8(value, buffer, out int _,
                    out int length);

                Debug.Assert(status == OperationStatus.Done);

                Array.Resize(ref buffer, length);

                return _valueBytes = buffer;
            }
        }
    }
}
