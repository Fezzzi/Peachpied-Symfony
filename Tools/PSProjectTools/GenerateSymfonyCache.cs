using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Build.Tasks {

    /// <summary>
    /// Simple task to generate Symfony project cache with the help of dedicated PHP script
    /// <summary>
    public sealed class GenerateSymfonyCache : Task {

        public override bool Execute() {
            Tuple<string, string> config = getAppConfig();
            runGeneratorScript(config.Item1, config.Item2);

            return true;
        }

        /// <summary>
        /// Parse application configuration to generate cache for corret environment
        /// </summary>
        private static Tuple<string, string> getAppConfig() {
            IConfigurationRoot appconfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).FullName)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            if (appconfig != null) {
                IConfigurationSection symfonyCfg = appconfig.GetSection("Symfony");

                if (symfonyCfg.GetValue<bool>("OVERRIDE_ENV")) {
                    return new Tuple<string, string>(
                        symfonyCfg.GetValue<string>("APP_ENV"),
                        symfonyCfg.GetValue<string>("APP_DEBUG")
                    );
                }
            }

            // Load configuration from .env file
            var env = getEnvConfig(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
            var envLocal = getEnvConfig(Path.Combine(Directory.GetCurrentDirectory(), ".env.local"));

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
            }

            return new Tuple<string, string>(
                (env != null && env.ContainsKey("APP_ENV")) ? env["APP_ENV"] : "dev",
                (env != null && env.ContainsKey("APP_DEBUG")) ? env["APP_DEBUG"] : "0"
            );
        }

        /// <summary>
        /// Loads values from given env file and returns them as a dictionary
        /// </summary>
        private static Dictionary<string, string> getEnvConfig(string path) {
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

        /// <summary>
        /// Invokes simple php script that generates cache
        /// </summary>
        private static void runGeneratorScript(string env, string debug) {
            // Actual path to .dll inside the nuget package
            string location = new Uri(typeof(GenerateVendorComposer).Assembly.CodeBase).LocalPath;
            location = location.Substring(0, location.LastIndexOf(Path.DirectorySeparatorChar));
            string contentPath = $"{location}\\..\\..\\tools\\any\\netstandard2.0\\";

            var startInfo = new ProcessStartInfo() {
                FileName = "php.exe",
                Arguments = $"\"{contentPath}\\Cache-Generator-Tool.php\" {env} {debug};",
                UseShellExecute = false
            };
            // We need to wait until autoloaders are dumped 
            Process.Start(startInfo).WaitForExit();
        }
    }
}
