namespace NetBooru.Web.Options
{
    public class UploadOptions
    {
        public uint MaxFileSize { get; set; }

        public string UploadLocation { get; set; } = null!;

        public int DirectorySeparationBytes { get; set; }
    }
}
