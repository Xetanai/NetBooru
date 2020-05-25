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
        public string Id { get; set; } = null!;

        /// <summary>
        /// The user who uploaded the post
        /// </summary>
        public virtual User Uploader { get; set; } = null!;

        /// <summary>
        /// The md5 hash of the file uploaded
        /// </summary>
        public string Hash { get; set; } = null!;

        /// <summary>
        /// The post's filetype-specific metadata
        /// </summary>
        public virtual PostMetadata Meta { get; set; } = null!;

        /// <summary>
        /// The post's parent
        /// </summary>
        public virtual Post? Parent { get; set; }

        /// <summary>
        /// The post's children
        /// </summary>
        public virtual ICollection<Post?> Children { get; set; } = null!;
    }
}
