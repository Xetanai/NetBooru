using System.Collections.Generic;

namespace NetBooru.Data
{
    /// <summary>
    /// Represents a tag which implies one or more other tags.
    /// </summary>
    public class TagImplication
    {
        /// <summary>
        /// The source tag id which implicates the target tag id
        /// </summary>
        public ulong SourceId { get; set; }

        /// <summary>
        /// Target tag id which is implicated by the source tag id
        /// </summary>
        public ulong TargetId { get; set; }

        /// <summary>
        /// The source tag which implicates the target
        /// </summary>
        public virtual Tag Source { get; set; } = null!;

        /// <summary>
        /// Target tag which is implicated by the source tag
        /// </summary>
        public virtual Tag Target { get; set; } = null!;
    }
}
