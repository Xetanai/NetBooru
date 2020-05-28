namespace NetBooru.Data
{
    /// <summary>
    /// Represents a tag which is aliased to another tag.
    /// </summary>
    public class TagAlias
    {
        /// <summary>
        /// The unique ID for this alias.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The source phrase to resolve from
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The tag which this tag resolves to
        /// </summary>
        public virtual Tag Target { get; set; } = null!;
    }
}
