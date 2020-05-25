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


            // TODO: Enfore that these be ID 0
            var tagme = new Tag()
            {
                Name = "tagme",
                //Id = 0
            };

            var anon = new User()
            {
                Username = "Anonymous",
                //Id = 0,
                UseDarkMode = false
            };

            context.Update(tagme);
            context.Update(anon);

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
