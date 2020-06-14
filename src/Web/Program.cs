using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetBooru.Web.Configuration;

namespace NetBooru.Web
{
    public class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    _ = builder.Add<MimeTypeDatabaseConfigurationSource>(
                        source =>
                        {
                            source.ConfigurationPath = new[]
                            {
                                "Upload",
                                "MimeTypeDatabase"
                            };
                            source.Path = "mimetypes.json";
                            source.Optional = false;
                            source.ReloadOnChange = false;
                            source.ResolveFileProvider();
                        });
                })
                .ConfigureWebHostDefaults(
                    webBuilder => webBuilder.UseStartup<Startup>());
    }
}
