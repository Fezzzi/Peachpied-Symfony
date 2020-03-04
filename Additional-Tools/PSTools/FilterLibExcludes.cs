using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.Build.Tasks {

    /// <summary>
    /// Task that automatizes Symfony component's compilation blockig files and dependencies exclusion.
    /// </summary>
    public sealed class FilterLibExcludes : Task {
        [Required]
        public ITaskItem ConfigPath { get; set; }
        [Required]
        public ITaskItem LibPath { get; set; }
        [Required]
        public ITaskItem LibName { get; set; }
        [Required]
        public ITaskItem LibVersion { get; set; }
        [Required]
        public ITaskItem[] Compile { get; set; }
        [Output]
        public ITaskItem[] NewCompile { get; private set; }

        public override bool Execute() {
            string name = LibName.ItemSpec.Replace('.', '/').ToLower();
            JsonObject package = getPackage(name, LibVersion.ItemSpec, ConfigPath.ItemSpec);

            if (package != null) {
                JsonArray excludes = package["excludes"] as JsonArray;
                JsonArray includes = package["includes"] as JsonArray;
                NewCompile = updateCompileItem(Compile, excludes, includes, LibPath.ItemSpec);
            }
            return true;
        }

        /// <summary>
        /// Filters out compilation blocking files and adds back needed files removed by default filters.
        /// </summary>
        private ITaskItem[] updateCompileItem (
            ITaskItem[] compile, 
            JsonArray excludes, 
            JsonArray includes, 
            string libPath
        ) {
            HashSet<string> excludesMap = new HashSet<string>(excludes.Select(
                e => $"{libPath}{e.ToString().Replace("\"", "").Replace('/', Path.DirectorySeparatorChar)}"
            ));
            List<ITaskItem> newCompile = new List<ITaskItem>();

            foreach (ITaskItem item in compile) {
                if (!excludesMap.Contains(item.ItemSpec)) {
                    newCompile.Add(item);
                }
            }
            foreach (JsonValue include in includes) {
                ITaskItem item = new TaskItem(include);
                item.SetMetadata("Includes", include);
                newCompile.Add(item);
            }
            return newCompile.ToArray();
        }

        /// <summary>
        /// Gets the package part from the config json.
        /// </summary>
        private static JsonObject getPackage(string libName, string libVersion, string configPath) {
            string config = Path.Combine(configPath, "libsConfig.json");

            if (!File.Exists(config)) {
                Console.WriteLine("Config file not found!");
                return null;
            }
            string configText = File.ReadAllText(config);
            JsonObject result = JsonValue.Parse(configText) as JsonObject;

            result = result[libName] as JsonObject;
            return result == null ? null : (result[libVersion] as JsonObject);
        }
    }
}
