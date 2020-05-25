using Microsoft.EntityFrameworkCore;

namespace NetBooru.Data
{
    /// <summary>
    /// Configuration utility for <see cref="NetBooruContext"/>
    /// </summary>
    public static class DatabaseProvider
    {
        /// <summary>
        /// Configures a <see cref="DbContextOptionsBuilder"/> to use the
        /// correct database provider and migrations assembly.
        /// </summary>
        /// <param name="builder">The builder to configure</param>
        /// <param name="connectionString">The connection string to use</param>
        /// <returns>
        /// The <see cref="DbContextOptionsBuilder"/>, for chaining.
        /// </returns>
        public static DbContextOptionsBuilder ConfigureProvider(
            DbContextOptionsBuilder builder, string connectionString)
        {
            return builder.UseInMemoryDatabase(connectionString);
        }
    }
}
