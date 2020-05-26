using System;
using System.Collections.Generic;
using System.Linq;
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
using NetBooru.Web.Options;

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

            var connectionString =
                Configuration.GetConnectionString("Database");

            _ = services.AddDbContext<NetBooruContext>(builder =>
            {
                _ = builder.UseLazyLoadingProxies();

                _ = DatabaseProvider.ConfigureProvider(
                    builder, connectionString);
            });

            _ = services
                .AddIdentity<User, Role>()
                .AddDefaultTokenProviders();

            if (Environment.IsDevelopment())
                _ = services.AddHostedService<DatabaseMigrator>();

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
