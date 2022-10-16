using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.IO;

namespace SecureClient
{
    public class AuthConfig
    {
        public string Instance { get; set; } = null!;
        public string TenantId { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string Authority => String.Format(CultureInfo.InvariantCulture, Instance, TenantId);
        public string ClientSecret { get; set; } = null!;
        public string BaseAddress { get; set; } = null!;
        public string ResourceId { get; set; } = null!;

        public static AuthConfig ReadJsonFromFile(string path)
        {
            IConfiguration configuration;

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(path);

            configuration = builder.Build();

            return configuration.Get<AuthConfig>();
        }

        public static AuthConfig ReadFromSecrets(string path)
        {      
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(path).AddUserSecrets<AuthConfig>().Build();         
            return config.Get<AuthConfig>();
        }
    }
}
