namespace NetBooru.Data
{
    /// <summary>
    /// Represents a tag which is aliased to another tag.
    /// </summary>
    public class AliasTag
    {
        /// <summary>
        /// The source phrase to resolve from
        /// </summary>
        public string Source { get; set; } = null!;

        /// <summary>
        /// The tag which this tag resolves to
        /// </summary>
        public virtual Tag Target { get; set; } = null!;
    }
}
