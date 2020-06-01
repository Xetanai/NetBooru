namespace NetBooru.Web.Services
{
    public class UploadedFile
    {
        public byte[] Hash { get; }
        public string MediaType { get; }

        public UploadedFile(byte[] hash, string mediaType)
        {
            Hash = hash;
            MediaType = mediaType;
        }
    }
}
