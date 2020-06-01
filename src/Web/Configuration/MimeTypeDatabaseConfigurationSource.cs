using Microsoft.Extensions.Configuration;

namespace NetBooru.Web.Configuration
{
    public class MimeTypeDatabaseConfigurationSource : FileConfigurationSource
    {
        /// <summary>
        /// The base path of the mime type database in the built configuration
        /// </summary>
        public string[] ConfigurationPath { get; set; } = null!;

        public override IConfigurationProvider Build(
            IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);

            return new MimeTypeDatabaseConfigurationProvider(this,
                ConfigurationPath);
        }
    }
}
