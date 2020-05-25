using System.Collections.Generic;

namespace NetBooru.Data
{
    /// <summary>
    /// Represents a tag which implies one or more other tags.
    /// </summary>
    public class ImplicitTag : Tag
    {
        /// <summary>
        /// List of tags which are implied by this tag.
        /// </summary>
        public IList<Tag?> Targets { get; set; } = null!;
    }
}
