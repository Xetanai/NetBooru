using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetBooru.Data;
using NetBooru.Data.Npgsql;
using NetBooru.Data.Sqlite;
using NetBooru.Web.Options;
using NetBooru.Web.Services;

namespace NetBooru.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.Configure<LandingOptions>(
                Configuration.GetSection("Landing"));
            _ = services.Configure<UploadOptions>(
                Configuration.GetSection("Upload"));

            var provider = Configuration.GetValue<DatabaseProvider?>(
                "DatabaseProvider");

            _ = services.AddDbContext<NetBooruContext>(builder =>
            {
                _ = builder.UseLazyLoadingProxies();

                if (Environment.IsDevelopment())
                    _ = builder.EnableSensitiveDataLogging();

                _ = provider switch
                {
                    DatabaseProvider.Npgsql => builder.UseNpgsql(
                        Configuration.GetConnectionString("npgsql"),
                        o => o.MigrationsAssembly(
                            typeof(NpgsqlDesignTimeContextFactory)
                            .Assembly.FullName)),
                    DatabaseProvider.Sqlite => builder.UseSqlite(
                        Configuration.GetConnectionString("sqlite"),
                        o => o.MigrationsAssembly(
                            typeof(SqliteDesignTimeContextFactory)
                            .Assembly.FullName)),
                    DatabaseProvider.Memory => builder.UseInMemoryDatabase(
                        Configuration.GetConnectionString("memory")),
                    _ => throw new NotSupportedException(
                        $"Provider {provider} is not supported.")
                };
            });

            _ = services.AddHostedService<DatabaseMigrator>();

            _ = services.AddTransient<FileUploadService>();

            _ = services
                .AddIdentity<User, Role>()
                .AddEntityFrameworkStores<NetBooruContext>()
                .AddDefaultTokenProviders();

            _ = services
                .AddAuthorization(options =>
                {
                    foreach (var p in PermissionConfiguration.Permissions)
                    {
                        options.AddPolicy(p.Claim,
                            policy => policy.RequireClaim(p.Claim));
                    }
                });

            var mvc = services.AddControllersWithViews();

            if (Environment.IsDevelopment())
                _ = mvc.AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
            }
            else
            {
                _ = app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                _ = app.UseHsts();
            }

            _ = app
                .UseHttpsRedirection()
                .UseStaticFiles()

                .UseRouting()

                .UseAuthentication()
                .UseAuthorization()

                .UseEndpoints(endpoints =>
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}"));
        }
    }
}
