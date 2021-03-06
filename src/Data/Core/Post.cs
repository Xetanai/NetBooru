using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetBooru.Data
{
    /// <summary>
    /// Represents a posted media.
    /// </summary>
    public class Post
    {
        /// <summary>
        /// The unique ID of the post
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The user who uploaded the post
        /// </summary>
        public virtual User Uploader { get; set; } = null!;

        /// <summary>
        /// The hash of the file uploaded
        /// </summary>
        public byte[] Hash { get; set; } = null!;

        /// <summary>
        /// The post's filetype-specific metadata
        /// </summary>
        public virtual PostMetadata Metadata { get; set; } = null!;

        /// <summary>
        /// The post's parent
        /// </summary>
        public virtual Post? Parent { get; set; }

        /// <summary>
        /// The post's children
        /// </summary>
        public virtual ICollection<Post> Children { get; set; } = null!;

        /// <summary>
        /// The post's tags
        /// </summary>
        public virtual ICollection<PostTag> PostTags { get; set; } = null!;
    }
}
