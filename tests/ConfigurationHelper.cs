using Microsoft.Extensions.Configuration;

namespace Kros.Utils.UnitTests
{
    internal class ConfigurationHelper
    {
        private static IConfigurationRoot? _config;

        public static IConfigurationRoot GetConfiguration()
        {
            if (_config is null)
            {
                _config = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json")
                    .AddJsonFile($"appsettings.local.json", true)
                    .Build();
            }

            return _config;
        }
    }
}
