using System;
using System.Diagnostics;
using System.IO;
using System.Json;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Formater = JsonFormatterPlus.JsonFormatter;

namespace PSProjectTools {

    /// <summary>
    /// Task that generates vendor/composer folder and autoload script
    /// </summary>
    public sealed class GenerateVendorComposer : Task {
        [Required]
        public ITaskItem ProjPath { get; set; }

        public override bool Execute() {
            generateInstalledJson(ProjPath.ItemSpec);
            runRestoreScript(ProjPath.ItemSpec);
            return true;
        }

        /// <summary>
        /// Invokes simple powerShell script that calls included composer to dump autoloaders
        /// </summary>
        private static void runRestoreScript(string projectPath) {
            // Actual path to .dll inside the nuget package
            string location = new Uri(typeof(GenerateVendorComposer).Assembly.CodeBase).LocalPath;
            location = location.Substring(0, location.LastIndexOf(Path.DirectorySeparatorChar));
            string contentPath = $"{location}\\..\\..\\tools\\any\\netstandard2.0\\";

            var startInfo = new ProcessStartInfo() {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy unrestricted /c php \"{contentPath}\\composerDumpAutoload.phar\";",
                UseShellExecute = false
            };
            // We need to wait until autoloaders are dumped 
            Process.Start(startInfo).WaitForExit();
        }

        /// <summary>
        /// Produces installed.json file that is further processed by composer
        /// </summary>
        private static void generateInstalledJson(string projPath) {
            JsonArray packages = getPackages(projPath);
            string composerDir = Path.Combine(projPath, "vendor", "composer");
            // Formater seems not to work properly
            // string jsonString = Formater.Format(packages.ToString());
            string jsonString = packages.ToString();
            Directory.CreateDirectory(composerDir);
            using (StreamWriter sw = new StreamWriter(Path.Combine(composerDir, "installed.json"))) {
                sw.Write(jsonString);
            }
        }

        /// <summary>
        /// Parses out and unifies packages part of composer.lock file.
        /// </summary>
        private static JsonArray getPackages(string projPath) {
            JsonArray packages = new JsonArray();
            string lockPath = getLockPath(projPath);

            if (lockPath != null) {
                string lockText = File.ReadAllText(lockPath);
                JsonObject json = JsonValue.Parse(lockText) as JsonObject;

                foreach (JsonObject package in json["packages"] as JsonArray) {
                    package.Add("version-normalized", package["version"] + ".0");
                    package.Add("installation-source", "dist");
                    packages.Add(package);
                }
                foreach (JsonObject package in json["packages-dev"] as JsonArray) {
                    package.Add("version-normalized", package["version"] + ".0");
                    package.Add("installation-source", "dist");
                    packages.Add(package);
                }
            }
            return packages;
        }

        /// <summary>
        /// Searches provided path and all parent directories for composer.lock.
        /// </summary>
        private static string getLockPath(string path) {
            DirectoryInfo di = path != String.Empty ? new DirectoryInfo(path) : null;

            while (di != null && !File.Exists(Path.Combine(di.FullName, "composer.lock"))) {
                di = di.Parent;
            }
            if (di == null || !File.Exists(Path.Combine(di.FullName, "composer.lock"))) {
                Console.WriteLine("composer.lock file was not found!");
                return String.Empty;
            }

            return Path.Combine(di.FullName, "composer.lock");
        }
    }
}
