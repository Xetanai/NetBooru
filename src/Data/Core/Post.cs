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
        /// Unique ID for this post.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The user who uploaded this post, or null if in a single-user environment.
        /// </summary>
        public User? Uploader { get; set; }

        /// <summary>
        /// The hash of the post. Used for rudimentary duplicate checking and file location.
        /// </summary>
        public string Hash { get; set; } = null!;

        /// <summary>
        /// The extension of the post. png, jpg, webm, etc.
        /// </summary>
        public string Extension { get; set; } = null!;

        /// <summary>
        /// The parent post of this post. Can be (and usually is) null.
        /// </summary>
        public Post? Parent {get; set;}

        /// <summary>
        /// A possibly (and usually) empty list of child posts to this post.
        /// </summary>
        public IList<Post?> Children { get; set; } = null!;

        /// <summary>
        /// The score of the post. Will always be 0 in single-user environments.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// The tags for this post
        /// </summary>
        public IList<PostTag> PostTags { get; set; } = null!;
    }
}
