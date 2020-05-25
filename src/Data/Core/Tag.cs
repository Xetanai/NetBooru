using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetBooru.Data
{
    /// <summary>
    /// Represents a tag.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Unique internal ID of the tag.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The name of the tag, sans any prefix
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The category, if any, this tag falls under.
        /// </summary>
        public TagCategory? Category { get; set; }

        /// <summary>
        /// The posts for this tag.
        /// </summary>
        public IList<PostTag> PostTags { get; set; } = null!;
    }
}
