﻿using System;
using System.Diagnostics;
using System.IO;
using System.Json;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using JsonFormatter;

namespace Microsoft.Build.Tasks {

    /// <summary>
    /// Task that generates vendor/composer folder and autoload script
    /// </summary>
    public sealed class GenerateSymfonyAutoload : Task {
        [Required]
        public string ProjPath { get; set; }
        [Output]
        public bool Executed { get; private set; } = false;

        public override bool Execute() {
            if (generateInstalledJson(ProjPath)) {
                runRestoreScript(ProjPath);
                Executed = true;
            }
            return true;
        }

        /// <summary>
        /// Invokes composer to dump autoloaders
        /// </summary>
        private static void runRestoreScript(string projectPath) {
            // Actual path to .dll inside the nuget package
            string location = new Uri(typeof(GenerateSymfonyAutoload).Assembly.CodeBase).LocalPath;
            location = location.Substring(0, location.LastIndexOf(Path.DirectorySeparatorChar));
            string contentPath = $"{location}\\..\\..\\tools\\any\\netstandard2.0\\";

            var startInfo = new ProcessStartInfo() {
                FileName = "php.exe",
                Arguments = $"\"{contentPath}\\composer.phar\" dump-autoload",
                UseShellExecute = false
            };
            // We need to wait until autoloaders are dumped 
            Process.Start(startInfo).WaitForExit();
        }

        /// <summary>
        /// Produces installed.json file that is further processed by composer
        /// </summary>
        private static bool generateInstalledJson(string projPath) {
            JsonArray packages = getPackages(projPath);
            if (packages == null) {
                return false;
            }
            string composerDir = Path.Combine(projPath, "vendor", "composer");
            string jsonString = JsonHelper.FormatJson(packages.ToString());
            Directory.CreateDirectory(composerDir);
            using (StreamWriter sw = new StreamWriter(Path.Combine(composerDir, "installed.json"))) {
                sw.Write(jsonString);
            }
            return true;
        }

        /// <summary>
        /// Parses out and unifies packages part of composer.lock file.
        /// </summary>
        private static JsonArray getPackages(string projPath) {
            JsonArray packages = new JsonArray();
            string lockPath = getLockPath(projPath);

            if (lockPath == null) {
                return null;
            }
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
