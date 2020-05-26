using System.Security.Cryptography.Xml;
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

            // Has all permissions, no matter what.
            context.Roles.Add(new Role()
            {
                NormalizedName = "Owner",
                Deletable = false,
                Locked = true,
                Color = 0xFFFFFF
            });

            // If not logged in, the Anonymous virtual user they occupy will be in this role.
            context.Roles.Add(new Role()
            {
                NormalizedName = "Anonymous",
                Deletable = false,
                Locked = false,
                Color = 0xBBBBBB
            });

            // Has no permissions, no matter what.
            context.Roles.Add(new Role()
            {
                NormalizedName = "Banned",
                Deletable = false,
                Locked = true,
                Color = 0x996666
            });

            await context.SaveChangesAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return cancellationToken.IsCancellationRequested
                ? Task.FromCanceled(cancellationToken)
                : Task.CompletedTask;
        }
    }
}
