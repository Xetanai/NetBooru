namespace NetBooru.Data
{
    /// <summary>
    /// Extended Metadata which applies only to images
    /// </summary>
    public class ImagePostMetadata : PostMetadata
    {
        /// <summary>
        /// Image's width, in pixels
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Image's height, in pixels
        /// </summary>
        public int Height { get; set; }
    }
}
