namespace NetBooru.Data
{
    /// <summary>
    /// Represents a tag category.
    /// </summary>
    public class TagCategory
    {
        /// <summary>
        /// Unique internal ID.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The color tags in this category should be.
        /// </summary>
        public uint Color { get; set; }

        /// <summary>
        /// Name of the category.
        /// </summary>
        public string Name { get; set; } = null!;
    }
}
