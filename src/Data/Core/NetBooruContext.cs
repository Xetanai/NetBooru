using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NetBooru.Data
{
    /// <summary>
    /// The database
    /// </summary>
    public class NetBooruContext
        : IdentityDbContext<User, Role, ulong>
    {
        /// <summary>
        /// The database set of all posts
        /// </summary>
        public DbSet<Post> Posts { get; set; } = null!;

        /// <summary>
        /// The database set of all post metadata
        /// </summary>
        public DbSet<PostMetadata> PostMetadata { get; set; } = null!;

        /// <summary>
        /// The database set of all post tags
        /// </summary>
        public DbSet<PostTag> PostTags { get; set; } = null!;

        /// <summary>
        /// The database set of all tags
        /// </summary>
        public DbSet<Tag> Tags { get; set; } = null!;

        /// <summary>
        /// The database set of all tag aliases
        /// </summary>
        public DbSet<TagAlias> TagAliases { get; set; } = null!;

        /// <summary>
        /// The database set of all tag categories
        /// </summary>
        public DbSet<TagCategory> TagCategories { get; set; } = null!;

        /// <summary>
        /// The database set of all tag implications
        /// </summary>
        public DbSet<TagImplication> TagImplications { get; set; } = null!;

        /// <inheritdoc/>
        public NetBooruContext(DbContextOptions<NetBooruContext> options)
            : base(options)
        { }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            _ = builder.Entity<IdentityRoleClaim<ulong>>()
                .Property(u => u.RoleId)
                .HasConversion<long>();

            _ = builder.Entity<IdentityUserClaim<ulong>>()
                .Property(u => u.UserId)
                .HasConversion<long>();

            _ = builder.Entity<IdentityUserLogin<ulong>>()
                .Property(u => u.UserId)
                .HasConversion<long>();

            _ = builder.Entity<IdentityUserRole<ulong>>()
                .Property(u => u.UserId)
                .HasConversion<long>();
            _ = builder.Entity<IdentityUserRole<ulong>>()
                .Property(u => u.RoleId)
                .HasConversion<long>();

            _ = builder.Entity<IdentityUserToken<ulong>>()
                .Property(u => u.UserId)
                .HasConversion<long>();

            _ = builder.Entity<Post>()
                .Property(u => u.Id)
                .HasConversion<long>()
                .HasValueGenerator<SnowflakeGenerator>();

            _ = builder.Entity<PostMetadata>()
                .Property(u => u.Id)
                .HasConversion<long>();
            _ = builder.Entity<ImagePostMetadata>()
                .HasBaseType<PostMetadata>();
            _ = builder.Entity<AudioPostMetadata>()
                .HasBaseType<PostMetadata>();
            _ = builder.Entity<VideoPostMetadata>()
                .HasBaseType<PostMetadata>();

            _ = builder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId, pt.TagId });
            _ = builder.Entity<PostTag>()
                .Property(u => u.PostId)
                .HasConversion<long>();
            _ = builder.Entity<PostTag>()
                .Property(u => u.TagId)
                .HasConversion<long>();

            _ = builder.Entity<Role>()
                .Property(u => u.Id)
                .HasConversion<long>()
                .HasValueGenerator<SnowflakeGenerator>();

            _ = builder.Entity<Tag>()
                .Property(u => u.Id)
                .HasConversion<long>()
                .ValueGeneratedNever()
                .HasValueGenerator<SnowflakeGenerator>();

            _ = builder.Entity<TagAlias>()
                .Property(u => u.Id)
                .HasConversion<long>()
                .HasValueGenerator<SnowflakeGenerator>();
            _ = builder.Entity<TagAlias>()
                .HasIndex(ta => ta.Name)
                .IsUnique();

            _ = builder.Entity<TagCategory>()
                .Property(u => u.Id)
                .HasConversion<long>()
                .HasValueGenerator<SnowflakeGenerator>();
            _ = builder.Entity<TagCategory>()
                .HasIndex(tc => tc.Name)
                .IsUnique();

            _ = builder.Entity<TagImplication>()
                .HasKey(ti => new { ti.SourceId, ti.TargetId });
            _ = builder.Entity<TagImplication>()
                .Property(u => u.SourceId)
                .HasConversion<long>();
            _ = builder.Entity<TagImplication>()
                .Property(u => u.TargetId)
                .HasConversion<long>();

            _ = builder.Entity<User>()
                .Property(u => u.Id)
                .HasConversion<long>()
                .HasValueGenerator<SnowflakeGenerator>();

            base.OnModelCreating(builder);
        }
    }
}
