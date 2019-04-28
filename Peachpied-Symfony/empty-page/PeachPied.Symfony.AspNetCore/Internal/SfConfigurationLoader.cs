using System;
using Microsoft.Extensions.Configuration;

namespace PeachPied.Symfony.AspNetCore.Internal
{
    /// <summary>
    /// Provides methods of loading <see cref="SymfonyConfig"/>.
    /// </summary>
    static class SfConfigurationLoader
    {
        /// <summary>
        /// Crates default configuration with default values.
        /// </summary>
        public static SymfonyConfig CreateDefault()
        {
            return new SymfonyConfig();
        }

        /// <summary>
        /// Loads configuration from appsettings file (<see cref="IConfiguration"/>).
        /// </summary>
        public static SymfonyConfig LoadFromSettings(this SymfonyConfig config, IServiceProvider services)
        {
            if (config == null)
            {
                config = CreateDefault();
            }

            var appconfig = (IConfiguration)services.GetService(typeof(IConfiguration));
            if (appconfig != null)
            {
                appconfig.GetSection("Symfony").Bind(config);
            }

            return config;
        }

        static bool HandleEnvironmentVar(string value, Func<string, string, bool> keyValueFunc)
        {
            // parses the environment variable separated with semicolon
            // Template: NAME=VALUE;NAME=VALUE;...
            // not expecting ';' or quotes in the value

            bool gotvalue = false;

            if (!string.IsNullOrEmpty(value))
            {
                foreach (var pair in value.Split(';'))
                {
                    var eq = pair.IndexOf('=');
                    if (eq < 0 || eq == pair.Length - 1) continue;

                    gotvalue |= keyValueFunc(pair.Remove(eq).Trim(), pair.Substring(eq + 1).Trim());
                }
            }

            return gotvalue;
        }
    }
}
