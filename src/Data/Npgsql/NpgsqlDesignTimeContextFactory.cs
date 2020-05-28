using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NetBooru.Data.Npgsql
{
    public class NpgsqlDesignTimeContextFactory
        : IDesignTimeDbContextFactory<NetBooruContext>
    {
        public NetBooruContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<NetBooruContext>();

            _ = builder.UseNpgsql(b => b.MigrationsAssembly(
                GetType().Assembly.FullName));

            return new NetBooruContext(builder.Options);
        }
    }
}
