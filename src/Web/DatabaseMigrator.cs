using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetBooru.Data;

namespace NetBooru.Web
{
    internal class DatabaseMigrator : IHostedService
    {
        private enum MigrationBehavior
        {
            Migrate, // NOTE: listed first as this is the default value
            DropAndMigrate,
            DropAndCreate
        }

        private readonly MigrationBehavior _behavior;
        private readonly IHostEnvironment _environment;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public DatabaseMigrator(IConfiguration configuration,
            IHostEnvironment environment,
            ILogger<DatabaseMigrator> logger,
            IServiceScopeFactory scopeFactory)
        {
            _behavior = configuration.GetValue<MigrationBehavior>(
                nameof(MigrationBehavior));
            _environment = environment;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting database migration");

            using var scope = _scopeFactory.CreateScope();

            await using var context = scope.ServiceProvider
                .GetRequiredService<NetBooruContext>();

            switch (_behavior)
            {
                case MigrationBehavior.DropAndCreate:
                case MigrationBehavior.DropAndMigrate:
                    if (_environment.IsDevelopment())
                    {
                        _logger.LogWarning(
                            "Dropping database as " +
                            $"{nameof(MigrationBehavior)} is {{behavior}} " +
                            "and environment is {environment}",
                            _behavior,
                            _environment.EnvironmentName);

                        _ = await context.Database.EnsureDeletedAsync(
                            cancellationToken);
                    }
                    else
                    {
                        _logger.LogError(
                            "Attempt to drop database ignored - " +
                            $"{nameof(MigrationBehavior)} is {{behavior}} " +
                            "and environment is {environment}",
                            _behavior,
                            _environment.EnvironmentName);
                    }
                    break;
            }

            if (_behavior == MigrationBehavior.DropAndCreate
                && _environment.IsDevelopment())
            {
                _logger.LogWarning(
                    "Creating database WITH NO MIGRATION SUPPORT");
                _ = await context.Database.EnsureCreatedAsync(
                    cancellationToken);
            }
            else if (_behavior == MigrationBehavior.Migrate
                || _behavior == MigrationBehavior.DropAndMigrate)
            {
                _logger.LogInformation(
                    "Database is being migrated to latest version...");
                await context.Database.MigrateAsync(cancellationToken);
            }

            await SeedDefaultUsersAsync(context, cancellationToken);
            await SeedDefaultRolesAsync(context, cancellationToken);
            await SeedDefaultPermissionsAsync(context, cancellationToken);

            _logger.LogInformation("Finished database migration");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return cancellationToken.IsCancellationRequested
                ? Task.FromCanceled(cancellationToken)
                : Task.CompletedTask;
        }

        private async Task SeedDefaultUsersAsync(
            NetBooruContext context, CancellationToken cancellationToken)
        {
            if (await context.Users.AnyAsync(cancellationToken))
                return;

            _logger.LogDebug(
                "Seeding initial users as none could be found");

            _ = context.Users.Add(new User
            {
                UserName = PermissionConfiguration.OwnerRole,
                NormalizedUserName = PermissionConfiguration.OwnerRole,
            });

            _ = await context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Done seeding initial users");
        }

        private async Task SeedDefaultRolesAsync(
            NetBooruContext context, CancellationToken cancellationToken)
        {
            if (await context.RoleClaims.AnyAsync(cancellationToken))
                return;

            _logger.LogDebug(
                "Seeding initial roles as none could be found");

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

            _logger.LogDebug("Done seeding initial roles");
        }

        private async Task SeedDefaultPermissionsAsync(
            NetBooruContext context, CancellationToken cancellationToken)
        {
            if (await context.RoleClaims.AnyAsync(cancellationToken))
                return;

            _logger.LogDebug(
                "Seeding initial role claims as none could be found");

            var ownerRoleId = await context.Roles.Where(
                x => x.NormalizedName == PermissionConfiguration.OwnerRole)
                .Select(x => x.Id)
                .SingleAsync(cancellationToken);

            var ownerUserId = await context.Users.Where(
                x => x.NormalizedUserName == PermissionConfiguration.OwnerRole)
                .Select(x => x.Id)
                .SingleAsync(cancellationToken);

            _ = context.UserRoles.Add(new IdentityUserRole<ulong>
            {
                RoleId = ownerRoleId,
                UserId = ownerUserId
            });

            foreach (var permission in PermissionConfiguration.Permissions)
            {
                _ = context.RoleClaims.Add(new IdentityRoleClaim<ulong>
                {
                    ClaimType = permission.Claim,
                    RoleId = ownerRoleId
                });
            }

            _ = await context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Done seeding initial role claims");
        }
    }
}
