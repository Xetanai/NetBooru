namespace NetBooru.Data
{
    /// <summary>
    /// Represents a tag which is aliased to another tag.
    /// </summary>
    public class AliasTag : Tag
    {
        /// <summary>
        /// The tag which this tag resolves to.
        /// </summary>
        public Tag Target { get; set; } = null!;
    }
}
