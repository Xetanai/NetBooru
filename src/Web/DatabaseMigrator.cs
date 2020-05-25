using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetBooru.Data;

namespace NetBooru.Web
{
    internal class DatabaseMigrator : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DatabaseMigrator(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            await using var context = scope.ServiceProvider
                .GetRequiredService<NetBooruContext>();

            await context.Database.MigrateAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.FromCanceled(cancellationToken);
    }
}
