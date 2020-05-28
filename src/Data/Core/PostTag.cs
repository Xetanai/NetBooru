namespace NetBooru.Data
{
    /// <summary>
    /// A join table for posts and tags
    /// </summary>
    public class PostTag
    {
        /// <summary>
        /// The post id for this tag
        /// </summary>
        public ulong PostId { get; set; }

        /// <summary>
        /// The tag id for this post
        /// </summary>
        public ulong TagId { get; set; }

        /// <summary>
        /// The post for this tag
        /// </summary>
        public virtual Post Post { get; set; } = null!;

        /// <summary>
        /// The tag for this post
        /// </summary>
        public virtual Tag Tag { get; set; } = null!;
    }
}
