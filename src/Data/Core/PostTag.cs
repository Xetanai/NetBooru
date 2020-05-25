namespace NetBooru.Data
{
    /// <summary>
    /// A join table for posts and tags
    /// </summary>
    public class PostTag
    {
        /// <summary>
        /// The post for this tag
        /// </summary>
        public Post Post { get; set; } = null!;

        /// <summary>
        /// The post id for this tag
        /// </summary>
        public ulong PostId { get; set; }

        /// <summary>
        /// The tag for this post
        /// </summary>
        public Tag Tag { get; set; } = null!;

        /// <summary>
        /// The tag id for this post
        /// </summary>
        public ulong TagId { get; set; }
    }
}
