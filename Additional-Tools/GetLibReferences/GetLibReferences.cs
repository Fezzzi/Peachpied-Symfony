using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.Build.Tasks {
    public sealed class GetLibReferences : Task {
        [Required]
        public ITaskItem TempPath { get; set; }
        [Required]
        public ITaskItem RepoPath { get; set; }
        [Required]
        public ITaskItem LibName { get; set; }
        [Output]
        public ITaskItem[] References { get; private set; }

        public override bool Execute() {
            string name = LibName.ItemSpec.Replace('.', '/').ToLower();
            Tuple<JsonObject, JsonObject> jsonData = getPackage(name, TempPath.ItemSpec);
            if (jsonData.Item1 == null || jsonData.Item2 == null) {
                return false;
            }

            References = resolveReferences(jsonData.Item1, jsonData.Item2, RepoPath.ItemSpec);
            return true;
        }

        private static ITaskItem[] resolveReferences(JsonObject package, JsonObject packages, string repoPath) {
            JsonObject dependencies = package["dependencies"] as JsonObject;
            List<Tuple<string, string>> references = new List<Tuple<string, string>>();
            foreach (string dependency in dependencies.Keys) {
                if (packages.ContainsKey(dependency)) {
                    string version = (packages[dependency] as JsonObject)["version"];
                    string name = getReferenceName(dependency);
                    // We don't want to include dev dependencies as those can result in cycles
                    if (isNugetAvailable(name, version, repoPath) && !dependencies[dependency]) {
                        references.Add(new Tuple<string, string>(name, version));
                    // If not dev dependency display warning
                    } else if (!dependencies[dependency] && !(packages[dependency] as JsonObject)["dev"]) {
                        Console.WriteLine("Non-dev dependency " + name + " " + version + " nuget missing!");
                    }
                } else if (!dependencies[dependency]) {
                    Console.WriteLine("Non-dev dependency " + dependency + " package not installed!");
                }
            }
            TaskItem[] tasks = new TaskItem[references.Count];
            for (int i = 0; i < references.Count; ++i) {
                tasks[i] = new TaskItem(references[i].Item1);
                tasks[i].SetMetadata("Include", references[i].Item1);
                tasks[i].SetMetadata("Version", references[i].Item2);
            }
            return tasks;
        }

        private static string getReferenceName(string name) {
            string[] parts = name.Split('/');
            for (int i = 0; i < parts.Length; ++i) {
                parts[i] = Char.ToUpper(parts[i][0]) + parts[i].Substring(1);
            }
            return String.Join(".", parts);
        }

        private static bool isNugetAvailable(string name, string version, string repoPath) {
            string nuget = Path.Combine(repoPath, name + "." + version + ".nupkg");
            return File.Exists(nuget);
        }

        private static Tuple<JsonObject, JsonObject> getPackage(string libName, string tempPath) {
            string cache = Path.Combine(tempPath, "libsCache.json");
            if (!File.Exists(cache)) {
                Console.WriteLine("Cache file not found! Ensure cache are warmed prior running GetLibReference!");
                return null;
            }

            string cacheText = File.ReadAllText(cache);
            JsonObject result = JsonValue.Parse(cacheText) as JsonObject;
            return new Tuple<JsonObject, JsonObject> (result[libName] as JsonObject, result);
        }
    }
}
