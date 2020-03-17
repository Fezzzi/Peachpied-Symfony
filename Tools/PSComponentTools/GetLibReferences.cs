using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Microsoft.Build.Tasks {

    /// <summary>
    /// Task that automatizes compiled Symfony component's references retrieval.
    /// </summary>
    public sealed class GetLibReferences : Task {
        [Required]
        public ITaskItem CachePath { get; set; }
        [Required]
        public ITaskItem RepoPath { get; set; }
        [Required]
        public ITaskItem LibName { get; set; }
        [Output]
        public ITaskItem[] References { get; private set; }

        public override bool Execute() {
            string name = LibName.ItemSpec.Replace('.', '/').ToLower();
            JsonObject cache = parseCache(CachePath.ItemSpec);       
            if (cache == null || cache[name] == null) {
                return false;
            }

            References = resolveReferences(name, cache, RepoPath.ItemSpec);
            return true;
        }

        /// <summary>
        /// Resolves references for given package.
        /// </summary>
        private static ITaskItem[] resolveReferences(string packageName, JsonObject cache, string repoPath) {
            JsonObject package = cache[packageName] as JsonObject;
            JsonObject dependencies = package["dependencies"] as JsonObject;
            List<Tuple<string, string>> references = new List<Tuple<string, string>>();

            foreach (string dependency in dependencies.Keys) {
                if (cache.ContainsKey(dependency)) {
                    JsonObject dependencyPart = cache[dependency] as JsonObject;
                    string version = dependencyPart["version"];
                    string name = getReferenceName(dependency);

                    if (isNugetAvailable(name, version, repoPath)) {
                        references.Add(new Tuple<string, string>(name, version));
                    } else if (!dependencies[dependency] && !dependencyPart["dev"]) {
                        Console.WriteLine($"Non-dev dependency {name} {version} nuget missing!");
                    }
                } else if (!dependencies[dependency]) {
                    Console.WriteLine($"Non-dev dependency {dependency} package not installed!");
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

        /// <summary>
        /// Converts Symfony component name to nuget name.
        /// </summary>
        private static string getReferenceName(string name) {
            string[] parts = name.Split('/');

            for (int i = 0; i < parts.Length; ++i) {
                parts[i] = Char.ToUpper(parts[i][0]) + parts[i].Substring(1);
            }
            return String.Join(".", parts);
        }

        /// <summary>
        /// Tests nuget's presence in repository.
        /// </summary>
        private static bool isNugetAvailable(string name, string version, string repoPath) {
            string nuget = Path.Combine(repoPath, name + "." + version + ".nupkg");
            return File.Exists(nuget) || isNugetAvailableRecursive(repoPath, name + "." + version + ".nupkg");
        }

        /// <summary>
        /// Uses DFS to search directories inside provided repository for the nuget package
        /// </summary>
        private static bool isNugetAvailableRecursive(string currentDir, string nuget) {
            string[] directories = Directory.GetDirectories(currentDir);
            foreach (string directory in directories) {
                if (File.Exists(Path.Combine(directory, nuget)) || isNugetAvailableRecursive(directory, nuget)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Finds and parses json cache file.
        /// </summary>
        private static JsonObject parseCache(string cachePath) {
            string cacheFile = Path.Combine(cachePath, "libsCache.json");

            if (!File.Exists(cacheFile)) {
                Console.WriteLine("Cache file not found! Ensure cache are warmed prior running GetLibReference!");
                return null;
            }
            return JsonValue.Parse(File.ReadAllText(cacheFile)) as JsonObject;
        }
    }
}
