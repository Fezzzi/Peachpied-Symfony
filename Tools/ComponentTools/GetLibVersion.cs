using System;
using System.IO;
using System.Json;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.Build.Tasks {

    /// <summary>
    /// Task that automatizes compiled Symfony component's version retrieval.
    /// </summary>
    public sealed class GetLibVersion : Task {
        [Required]
        public ITaskItem CachePath { get; set; }
        [Required]
        public ITaskItem LibName { get; set; }
        [Output]
        public string Version { get; private set; }

        public override bool Execute() {
            string name = LibName.ItemSpec.Replace('.', '/').ToLower();
            JsonObject package = getPackageFromConfig(name, CachePath.ItemSpec);

            if (package == null) {
                return false;
            }
            Version = package["version"].ToString().Trim('"');
            Console.WriteLine($"Building {name} with version {Version}");

            return true;
        }

        /// <summary>
        /// Gets the package part from cache json.
        /// </summary>
        private static JsonObject getPackageFromConfig(string libName, string cachePath) {
            string cache = Path.Combine(cachePath, "libsCache.json");

            if (!File.Exists(cache)) {
                Console.WriteLine("Cache file not found! Ensure cache are warmed prior running GetLibVersion!");
                return null;
            }
            string cacheText = File.ReadAllText(cache);
            JsonObject result = JsonValue.Parse(cacheText) as JsonObject;

            return result[libName] as JsonObject;
        }
    }
}
