using System.Collections.Generic;

namespace NetBooru.Data
{
    /// <summary>
    /// Represents a tag which implies one or more other tags.
    /// </summary>
    public class ImplicitTag
    {
        /// <summary>
        /// The tag which implicates the target
        /// </summary>
        public virtual Tag Source { get; set; } = null!;

        /// <summary>
        /// Tag which is implicated by the source tag
        /// </summary>
        public virtual Tag Target { get; set; } = null!;
    }
}
