using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NetBooru.Data.Sqlite
{
    public class SqliteDesignTimeContextFactory
        : IDesignTimeDbContextFactory<NetBooruContext>
    {
        public NetBooruContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<NetBooruContext>();

            _ = options.UseSqlite(b => b.MigrationsAssembly(
                GetType().Assembly.FullName));

            return new NetBooruContext(options.Options);
        }
    }
}
