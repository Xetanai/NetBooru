using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NetBooru.Data
{
    /// <summary>
    ///
    /// </summary>
    public class NetBooruContext
        : IdentityDbContext<User, Role, ulong>
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

        /// <inheritdoc/>
        public NetBooruContext(DbContextOptions<NetBooruContext> options)
            : base(options)
        { }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            _ = builder.Entity<User>()
                .Property(u => u.Id)
                .HasConversion<long>();

            _ = builder.Entity<TagCategory>()
                .Property(u => u.Id)
                .HasConversion<long>();

            _ = builder.Entity<Tag>()
                .Property(u => u.Id)
                .HasConversion<long>();
        }
    }
}
