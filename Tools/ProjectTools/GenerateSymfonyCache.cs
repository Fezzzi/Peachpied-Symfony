using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Json;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.Build.Tasks {

    /// <summary>
    /// Simple task to generate Symfony project cache with the help of dedicated PHP script
    /// <summary>
    public sealed class GenerateSymfonyCache : Task {
        [Required]
        public ITaskItem Configuration { get; set; }
        [Output]
        public bool Executed { get; private set; }

        public override bool Execute() {
            Tuple<string, string> config = getAppConfig(Configuration.ItemSpec);
            runGeneratorScript(config.Item1, config.Item2);
            Executed = true;

            return true;
        }

        /// <summary>
        /// Parse application configuration to generate cache for corret environment
        /// </summary>
        private static Tuple<string, string> getAppConfig(string configuration) {
            // Load configuration appSettings files
            JsonObject appSettings = getAppSettingsConfig(configuration);
            if (appSettings != null && appSettings.ContainsKey("APP_ENV") && appSettings.ContainsKey("APP_DEBUG")) {
                return new Tuple<string, string>(
                    appSettings["APP_ENV"],
                    appSettings["APP_DEBUG"]
                );
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
        /// Loads values appsettings based on current Configuration settings
        /// </summary>
        private static JsonObject getAppSettingsConfig(string configuration) {
            JsonObject settings = null;
            JsonObject settingsConf = null;
            JsonObject getSymfonyPart(string file) {
                JsonObject settingsJson = JsonValue.Parse(File.ReadAllText(file)) as JsonObject;
                if (settingsJson.ContainsKey("Symfony")) {
                    return settingsJson["Symfony"] as JsonObject;
                } else if (settingsJson.ContainsKey("symfony")) {
                    return settingsJson["symfony"] as JsonObject;
                }
                return null;
            }

            string dirName = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            string fileName = File.Exists(Path.Combine(dirName, $"appsettings.{configuration}.json")) 
                ? $"appsettings.{configuration}.json" 
                : File.Exists(Path.Combine(dirName, "appsettings.Development.json"))
                    ? "appsettings.Development.json" 
                    : String.Empty;
            if (!configuration.Equals("Release") && !fileName.Equals(String.Empty)) {
                settingsConf = getSymfonyPart(Path.Combine(dirName, fileName));
            }
            if (File.Exists(Path.Combine(dirName, "appsettings.json"))) {
                settings = getSymfonyPart(Path.Combine(dirName, "appsettings.json"));
            }
            if (settingsConf == null && settings == null) {
                return null;
            }

            JsonObject config = new JsonObject();
            if (settingsConf != null && settingsConf.ContainsKey("APP_ENV")) {
                config.Add("APP_ENV", settingsConf["APP_ENV"]);
            } else if (settings != null && settings.ContainsKey("APP_ENV")) {
                config.Add("APP_ENV", settings["APP_ENV"]);
            }
            if (settingsConf != null && settingsConf.ContainsKey("APP_DEBUG")) {
                config.Add("APP_DEBUG", settingsConf["APP_DEBUG"]);
            } else if (settings != null && settings.ContainsKey("APP_DEBUG")) {
                config.Add("APP_DEBUG", settings["APP_DEBUG"]);
            }
            return config;
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
            string location = new Uri(typeof(GenerateSymfonyCache).Assembly.CodeBase).LocalPath;
            location = location.Substring(0, location.LastIndexOf(Path.DirectorySeparatorChar));
            string contentPath = $"{location}\\..\\..\\tools\\any\\netstandard2.0\\";

            var startInfo = new ProcessStartInfo() {
                FileName = "php.exe",
                Arguments = $"\"{contentPath}\\Cache-Generator-Tool.php\" {env} {debug}",
                UseShellExecute = false
            };
            // We need to wait until autoloaders are dumped 
            Process.Start(startInfo).WaitForExit();
        }
    }
}
