using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// <param name="config">SymfonyConfig</param>
        /// <param name="services">IServiceProvider</param>
        /// <param name="path">string</param>
        /// <returns></returns>
        public static SymfonyConfig LoadFromSettings
            (this SymfonyConfig config, IServiceProvider services, string path)
        {
            if (config == null) {
                config = CreateDefault();
            }

            var appconfig = (IConfiguration)services.GetService(typeof(IConfiguration));
            if (appconfig != null) {
                IConfigurationSection symfonyCfg =  appconfig.GetSection("Symfony");

                if (symfonyCfg.GetValue<bool>("OVERRIDE_ENV")) {
                    symfonyCfg.Bind(config);
                } else {
                    // Load configuration from .env file
                    var env = getEnvConfig(Path.Combine(path, ".env"));
                    var envLocal = getEnvConfig(Path.Combine(path, ".env.local"));

                    // Merge dictionaries into env
                    if (env != null && envLocal != null) {
                        envLocal.ToList().ForEach((x) => {
                            if (env.ContainsKey(x.Key)) {
                                env[x.Key] = x.Value;
                            } else {
                                env.Add(x.Key, x.Value);
                            }
                        });
                    } else if (envLocal != null) {
                        env = envLocal;
                    } else {
                        return config;
                    }

                    // update default config values with those loaded
                    foreach (Tuple<string, string> item in config.SymfonyEnvVarsMap) {
                        if (env.ContainsKey(item.Item2)) {
                            config.GetType().GetProperty(item.Item1).SetValue(config, env[item.Item2]);
                        }
                    }
                }
            }

            return config;
        }

        /// <summary>
        /// Loads values from given env file and returns them as a dictionary
        /// </summary>
        /// <param name="path">Path to a env file</param>
        /// <returns>Dictionary or null</returns>
        private static Dictionary<string, string> getEnvConfig(string path)
        {
            if (File.Exists(path)) {
                Dictionary<string, string> env = new Dictionary<string, string>();

                using (StreamReader file = new StreamReader(path)) {
                    string line;

                    while ((line = file.ReadLine()) != null) {
                        line.Trim();
                        if (line.Length > 0 && line[0] != '#') {
                            string[] keyVal = line.Split('=');
                            if (keyVal.Length == 2) {
                                if (env.ContainsKey(keyVal[0])) {
                                    env[keyVal[0]] = env[keyVal[1]];
                                } else {
                                    env.Add(keyVal[0], keyVal[1]);
                                }
                            }
                        }
                    }
                }

                return env;
            }

            return null;
        }
    }
}
