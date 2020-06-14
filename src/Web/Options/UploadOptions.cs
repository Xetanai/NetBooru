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

            public byte[] Mask { get; set; } = null!;

            public byte[] Value { get; set; } = null!;

            public int WordSize { get; set; }

            public int RangeLength { get; set; }

            public MimeTypePattern[] Children { get; set; } = null!;
        }
    }
}
