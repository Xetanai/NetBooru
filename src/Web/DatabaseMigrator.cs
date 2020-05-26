using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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

            // Unconditionally apply any migrations, just to ensure we have all
            // appropriate migrations.
            await context.Database.MigrateAsync(cancellationToken);

            await SeedDefaultRolesAsync(context, cancellationToken);
            await SeedDefaultPermissionsAsync(context, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return cancellationToken.IsCancellationRequested
                ? Task.FromCanceled(cancellationToken)
                : Task.CompletedTask;
        }

        private static async Task SeedDefaultRolesAsync(
            NetBooruContext context, CancellationToken cancellationToken)
        {
            // If there are no roles, they were either deleted or were simply
            // not scaffolded.
            if (await context.RoleClaims.AnyAsync(cancellationToken))
                return;

            // If not logged in, the Anonymous virtual user they occupy will be
            // in this role.
            _ = context.Roles.Add(new Role()
            {
                Name = PermissionConfiguration.AnonymousRole,
                NormalizedName = PermissionConfiguration.AnonymousRole,
                Deletable = false,
                Locked = false,
                Color = 0xBBBBBB
            });

            // Has no permissions, no matter what.
            _ = context.Roles.Add(new Role()
            {
                Name = PermissionConfiguration.BannedRole,
                NormalizedName = PermissionConfiguration.BannedRole,
                Deletable = false,
                Locked = true,
                Color = 0x996666
            });

            // Has all permissions, no matter what.
            _ = context.Roles.Add(new Role()
            {
                Name = PermissionConfiguration.OwnerRole,
                NormalizedName = PermissionConfiguration.OwnerRole,
                Deletable = false,
                Locked = true,
                Color = 0xFFFFFF
            });

            _ = await context.SaveChangesAsync(cancellationToken);
        }

        private static async Task SeedDefaultPermissionsAsync(
            NetBooruContext context, CancellationToken cancellationToken)
        {
            if (await context.RoleClaims.AnyAsync(cancellationToken))
                return;

            var ownerId = await context.Roles.Where(
                x => x.NormalizedName == PermissionConfiguration.OwnerRole)
                .Select(x => x.Id)
                .SingleAsync(cancellationToken);

            foreach (var permission in PermissionConfiguration.Permissions)
            {
                _ = context.RoleClaims.Add(new IdentityRoleClaim<ulong>
                {
                    ClaimType = permission.Claim,
                    RoleId = ownerId
                });
            }

            _ = await context.SaveChangesAsync(cancellationToken);
        }
    }
}
