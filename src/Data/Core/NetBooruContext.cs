using Microsoft.EntityFrameworkCore;

namespace NetBooru.Data
{
    /// <summary>
    ///
    /// </summary>
    public class NetBooruContext : DbContext
    {
        /// <summary>
        ///
        /// </summary>
        public DbSet<Post> Posts { get; set; } = null!;

        /// <summary>
        ///
        /// </summary>
        public DbSet<TagCategory> TagCategories { get; set; } = null!;

        /// <summary>
        ///
        /// </summary>
        public DbSet<Tag> Tags { get; set; } = null!;

        /// <summary>
        /// TODO: replace this with ASP.Net Auth
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <inheritdoc/>
        public NetBooruContext(DbContextOptions<NetBooruContext> options)
            : base(options)
        { }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            _ = builder.Entity<PostTag>()
                .HasKey(t => new { t.PostId, t.TagId });

            _ = builder.Entity<PostTag>()
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags);

            _ = builder.Entity<PostTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags);
        }
    }
}
