using System;
using System.IO;
using System.Json;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.Build.Tasks {
    public sealed class GetLibVersion : Task {
        [Required]
        public ITaskItem TempPath {
            get {
                return tempPath;
            }

            set {
                tempPath = value;
            }
        }

        [Required]
        public ITaskItem LibName {
            get {
                return libName;
            }

            set {
                libName = value;
            }
        }

        [Output]
        public ITaskItem Version {
            get {
                return version;
            }
        }

        private ITaskItem tempPath;
        private ITaskItem libName;
        private ITaskItem version;

        public override bool Execute() {
            string name = libName.ItemSpec.Replace('.', '/').ToLower();
            JsonObject package = getPackage(name, tempPath.ItemSpec);
            if (package == null) {
                return false;
            }

            this.version = new TaskItem(package["version"].ToString().Trim('"'));
            Console.WriteLine("Building " + name + " with version " + this.version);
            return true;
        }

        private static JsonObject getPackage(string libName, string tempPath) {
            string cache = Path.Combine(tempPath, "libsCache.json");
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
