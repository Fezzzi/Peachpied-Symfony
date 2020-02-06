using System;
using System.IO;
using System.Json;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using prettyPrinter = JsonPrettyPrinterPlus.PrettyPrinterExtensions;

namespace Microsoft.Build.Tasks {
    public sealed class WarmLibsCache : Task {
        [Required]
        public ITaskItem ProjPath {
            get {
                return projPath;
            }

            set {
                projPath = value;
            }
        }

        [Required]
        public ITaskItem TempPath {
            get {
                return tempPath;
            }

            set {
                tempPath = value;
            }
        }

        private ITaskItem projPath;
        private ITaskItem tempPath;

        public override bool Execute() {
            string lockPath = getLockPath(projPath.ItemSpec);
            return !lockPath.Equals(String.Empty)
                && warmCache(lockPath, tempPath.ItemSpec);
        }

        private static bool warmCache(string lockPath, string tempPath) {
            string lockText = File.ReadAllText(lockPath);
            JsonObject result = JsonValue.Parse(lockText) as JsonObject;
            JsonArray lockPackages = result["packages"] as JsonArray;
            JsonArray lockPackagesDev = result["packages-dev"] as JsonArray;
            JsonObject packages = new JsonObject();

            foreach (JsonObject package in lockPackages) {
                packages.Add(package["name"], getPackage(package, false));
            }
            foreach (JsonObject package in lockPackagesDev) {
                packages.Add(package["name"], getPackage(package, true));
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(tempPath, "libsCache.json"))) {
                string jsonString = packages.ToString();
                jsonString = prettyPrinter.PrettyPrintJson(jsonString);
                sw.Write(jsonString);
            }
            return true;
        }

        private static JsonObject getPackage(JsonObject data, bool isDev) {
            JsonObject package = new JsonObject();
            package.Add("version", ((string)data["version"]).Replace("v",""));
            package.Add("dev", isDev);
            package.Add("dependencies", getPackageDependencies(data));

            return package;
        }

        private static JsonObject getPackageDependencies(JsonObject data) {
            JsonObject dependencies = new JsonObject();
            foreach (string package in (data["require"] as JsonObject).Keys) {
                if (!package.Equals("php")) {
                    dependencies.Add(package, false);
                }
            }
            if (data.ContainsKey("require-dev")) {
                foreach (string package in (data["require-dev"] as JsonObject).Keys) {
                    if (!package.Equals("php")) {
                        dependencies.Add(package, true);
                    }
                }
            }

            return dependencies;
        }

        private static string getLockPath(string path) {
            DirectoryInfo di = new DirectoryInfo(path);
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
