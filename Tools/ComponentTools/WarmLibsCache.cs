using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using JsonFormatter;

namespace Microsoft.Build.Tasks {

    /// <summary>
    /// Tasks that processes composer.lock and generates libraries' cache
    /// </summary>
    public sealed class WarmLibsCache : Task {
        [Required]
        public string ProjPath { get; set; }
        [Required]
        public string ConfigPath { get; set; }

        public string CachePath { get; set; } = String.Empty;

        public List<string> OrderedComponents { get; private set; }
        public JsonArray LockPackages { get; private set; }
        public JsonArray LockPackagesDev { get; private set; }

        public override bool Execute() {
            string lockPath = getLockPath(ProjPath);
            string tempPath = CachePath == String.Empty ? Path.Combine(ProjPath, "obj") : CachePath;
            return !lockPath.Equals(String.Empty) && warmCache(lockPath, tempPath, ConfigPath);
        }

        /// <summary>
        /// Generates libsCache.json cache.
        /// </summary>
        private bool warmCache(string lockPath, string tempPath, string configPath) {
            JsonObject config = getConfig(configPath);
            (LockPackages, LockPackagesDev) = getPackageParts(lockPath);
            JsonObject packages = new JsonObject();

            foreach (JsonObject package in LockPackages) {
                packages.Add(package["name"], getPackageRecord(package, config, false));
            }
            foreach (JsonObject package in LockPackagesDev) {
                packages.Add(package["name"], getPackageRecord(package, config, true));
            }
            DependencyGraph graph = DependencyGraph.BuildFromCache(packages);
            graph.FilterCycles();
            List<Tuple<bool, Node>> orderedComponents = graph.SortPackages();
            OrderedComponents = new List<string>(orderedComponents.Select(el => el.Item2.name));
            string jsonString = JsonHelper.FormatJson(toJsonOrdered(orderedComponents));
            Directory.CreateDirectory(tempPath);
            using (StreamWriter sw = new StreamWriter(Path.Combine(tempPath, "libsCache.json"))) {
                sw.Write(jsonString);
            }

            return true;
        }

        /// <summary>
        /// Parses out packages part of composer.lock file.
        /// </summary>
        private static Tuple<JsonArray, JsonArray> getPackageParts(string lockPath) {
            string lockText = File.ReadAllText(lockPath);
            JsonObject result = JsonValue.Parse(lockText) as JsonObject;

            return new Tuple<JsonArray, JsonArray>(
                result["packages"] as JsonArray, 
                result["packages-dev"] as JsonArray
            );
        }

        /// <summary>
        /// Creates package record for the further processing.
        /// </summary>
        private static JsonObject getPackageRecord(JsonObject data, JsonObject config, bool isDev) {
            string version = ((string)data["version"]).Replace("v", "");
            return new JsonObject {
                { "version", version },
                { "dev", isDev },
                { "dependencies", getPackageDependencies(data, config, data["name"], version) }
            };
        }

        /// <summary>
        /// Groups together package dependencies and filters out dependency on PHP version.
        /// </summary>
        private static JsonObject getPackageDependencies(
            JsonObject data, 
            JsonObject config, 
            string name, 
            string version
        ) {
            JsonObject dependencies = new JsonObject();
            JsonObject ignores = config.ContainsKey(name) ? (config[name] as JsonObject) : null;
            ignores = ignores != null && ignores.ContainsKey(version) 
                ? (ignores[version] as JsonObject) 
                : ignores.ContainsKey("default") 
                    ? (ignores["default"] as JsonObject) 
                    : null;
            HashSet<string> ignoresMap = ignores != null && ignores.ContainsKey("ignoredDependencies") 
                ? new HashSet<string>((ignores["ignoredDependencies"] as JsonArray).Select(
                    e => e.ToString().Replace("\"", "")
                )) : null;

            foreach (string package in (data["require"] as JsonObject).Keys) {
                if (!package.Equals("php") && ignoresMap != null && !ignoresMap.Contains(package)) {
                    dependencies.Add(package, false);
                }
            }
            if (data.ContainsKey("require-dev")) {
                foreach (string package in (data["require-dev"] as JsonObject).Keys) {
                    if (!package.Equals("php") && ignoresMap != null && !ignoresMap.Contains(package)) {
                        dependencies.Add(package, true);
                    }
                }
            }
            return dependencies;
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

        /// <summary>
        /// Loads config file
        /// </summary>
        private static JsonObject getConfig(string configPath) {
            string config = Path.Combine(configPath, "libsConfig.json");

            if (!File.Exists(config)) {
                Console.WriteLine("Config file not found! Proceeding without it...");
                return null;
            }
            string configText = File.ReadAllText(config);
            return JsonValue.Parse(configText) as JsonObject;
        }

        /// <summary>
        /// Serialize the list of packages to json.
        /// </summary>
        private static JsonObject toJson(List<Tuple<bool, Node>> packages) {
            JsonObject obj = new JsonObject();

            foreach (var (isPackageDev, packageNode) in packages) {
                JsonObject packageDeps = new JsonObject();

                foreach (var dep in packageNode.children) {
                    packageDeps.Add(dep.Value.Item2.name, dep.Value.Item1);
                }
                obj.Add(packageNode.name, new JsonObject {
                    { "version", packageNode.version },
                    { "dev", isPackageDev },
                    { "dependencies", packageDeps }
                });
            }
            return obj;
        }

        /// <summary>
        /// Manually serialize the list of pakages to json string.
        /// We can't use default ToString of JsonObject as it would result in alphabetical reordering.
        /// </summary>
        private static string toJsonOrdered(List<Tuple<bool, Node>> packages) {
            bool firstPackage = true;
            StringBuilder str = new StringBuilder("{");

            foreach (var (isPackageDev, packageNode) in packages) {
                bool firstDependency = true;
                StringBuilder dependenciesStr = new StringBuilder("\"dependencies\": {");

                foreach (var dep in packageNode.children) {
                    if (!firstDependency) {
                        dependenciesStr.Append(",");
                    } else {
                        firstDependency = false;
                    }
                    dependenciesStr.Append($"\"{dep.Value.Item2.name}\": {dep.Value.Item1.ToString().ToLower()}");
                }
                dependenciesStr.Append("}");

                if (!firstPackage) {
                    str.Append(",");
                } else {
                    firstPackage = false;
                }
                str.Append($"\"{packageNode.name}\": {{");
                str.Append($"\"version\": \"{packageNode.version}\",");
                str.Append($"\"dev\": {isPackageDev.ToString().ToLower()},");
                str.Append(dependenciesStr.ToString());
                str.Append("}");
            }
            str.Append("}");
            return str.ToString();
        }
    }

    /// <summary>
    /// Class representing single dependency graph.
    /// </summary>
    class DependencyGraph {
        Node root;
        private List<Tuple<string, bool>[]> cycles = new List<Tuple<string, bool>[]>();

        /// <summary>
        /// Dependency graph factory.
        /// </summary>
        public static DependencyGraph BuildFromCache(JsonObject packages) {
            DependencyGraph graph = new DependencyGraph {
                root = new Node("Symfony", String.Empty)
            };

            foreach (var package in packages.Keys) {
                graph.addPackage(package, packages[package] as JsonObject, packages);
            }

            return graph;
        }

        /// <summary>
        /// Resolve circular dependencies by "minimal dev edges removal" strategy. 
        /// </summary>
        public void FilterCycles() {
            findCycles();
            resolveDevCycles();
        }

        /// <summary>
        /// Sorts packages into list by their dependencies.
        /// </summary>
        public List<Tuple<bool, Node>> SortPackages() {
            List<Tuple<bool, Node>> sortedPackages = new List<Tuple<bool, Node>>(root.children.Values);

            if (sortedPackages.Count > 1) {
                for (int index = 0; index < sortedPackages.Count; ++index) {
                    int replacement = findFirstCompilable(sortedPackages, index);
                    if (index != replacement) {
                        var temp = sortedPackages[replacement];
                        sortedPackages[replacement] = sortedPackages[index];
                        sortedPackages[index] = temp;
                    }
                }
            }
            return sortedPackages;
        }

        /// <summary>
        /// Finds first package starting at index whose all dependencies are in the packages list before him.
        /// </summary>
        private int findFirstCompilable(List<Tuple<bool, Node>> packages, int index) {
            for (int i = index; i < packages.Count; ++i) {
                bool isCompilable = true;
                foreach (string dependencyName in packages[i].Item2.children.Keys) {
                    bool isDependencyCompilable = false;
                    for (int j = 0; j < index; ++j) {
                        isDependencyCompilable |= dependencyName == packages[j].Item2.name;
                    }
                    isCompilable &= isDependencyCompilable;
                }
                if (isCompilable) {
                    return i;
                }
            }
            Console.WriteLine($"Package sorting error!");

            return packages.Count - 1;
        }

        /// <summary>
        /// For each cycle selects all their dev edges and removes the one leading to alphabetically last package.
        /// </summary>
        private void resolveDevCycles() {
            var newCycles = new List<Tuple<string, bool>[]>();
            foreach (var cycle in cycles) {
                var devNodes = new List<Tuple<int, string>>();
                for (int i = 0; i < cycle.Length - 1; ++i) {
                    if (cycle[i].Item2) {
                        devNodes.Add(new Tuple<int, string>(i + 1, cycle[i + 1].Item1));
                    }
                }
                if (devNodes.Count > 0) {
                    devNodes.Sort((node1, node2) => { return -String.Compare(node1.Item2, node2.Item2); });
                    root.children[cycle[devNodes[0].Item1 - 1].Item1].Item2.children.Remove(devNodes[0].Item2);
                } else {
                    newCycles.Add(cycle);
                }
            }
            cycles = newCycles;
        }

        /// <summary>
        /// Performs recursive search for cycles for each package.
        /// </summary>
        private void findCycles() {
            foreach (var package in root.children) {
                foreach (var dependency in package.Value.Item2.children) {
                    traverseDependencies(
                        new Dictionary<string, Tuple<int, bool>> {
                            { package.Key, new Tuple<int, bool>(0, dependency.Value.Item1) }
                        }, 
                        dependency.Value.Item2, 
                        1
                    );
                }
            }
        }

        /// <summary>
        /// Performs recursive step in dependencies traversal during cycles detencion.
        /// </summary>
        private void traverseDependencies(Dictionary<string, Tuple<int, bool>> traversed, Node node, int count) {
            if (traversed.ContainsKey(node.name)) {
                var cycle = traversedToCycle(traversed, node.name, false);
                cycle = cropCycle(cycle);
                if (isCycleUnique(cycle)) {
                    cycles.Add(cycle);
                }
            } else {
                foreach (var dependency in node.children) {
                    var newTraversed = new Dictionary<string, Tuple<int, bool>>(traversed);
                    newTraversed.Add(node.name, new Tuple<int, bool>(count, dependency.Value.Item1));
                    traverseDependencies(newTraversed, dependency.Value.Item2, count + 1);
                }
            }
        }
        
        /// <summary>
        /// Tests whether is the found cycle unique between already found cycles.
        /// </summary>
        private bool isCycleUnique(Tuple<string, bool>[] newCycle) {
            foreach (var cycle in cycles) {
                if (cycle.Length == newCycle.Length) {
                    int j = 0;
                    // Find the first package shared by both cycles
                    while (j < newCycle.Length && cycle[0].Item1 != newCycle[j].Item1) {
                        ++j;
                    }
                    if (j != newCycle.Length) {
                        bool duplicate = true;
                        // Continue to compare packages in both cycles from the found shared package.
                        for (int i = 0; i < cycle.Length; ++i) {
                            duplicate &= cycle[i].Item1 == newCycle[j].Item1;
                            j = j + 1 == newCycle.Length ? 1 : j + 1;
                        }
                        if (duplicate) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Removes packages that are not part of the cycle found.
        /// </summary>
        private Tuple<string, bool>[] cropCycle(Tuple<string, bool>[] cycle) {
            string end = cycle[cycle.Length - 1].Item1;
            int index = 0;
            while (cycle[index].Item1 != end) {
                ++index;
            }
            if (index > 0) {
                var newCycle = new Tuple<string, bool>[cycle.Length - index];
                Array.Copy(cycle, index, newCycle, 0, cycle.Length - index);
                return newCycle;
            }
            return cycle;
        }

        /// <summary>
        /// Sort list of traversed nodes by order of their traversal.
        /// </summary>
        private Tuple<string, bool>[] traversedToCycle(
            Dictionary<string, Tuple<int, bool>> traversed, 
            string lastName, 
            bool lastDev
        ) {
            var cycle = new Tuple<string, bool>[traversed.Count + 1];
            foreach (string key in traversed.Keys) {
                int index = traversed[key].Item1;
                cycle[index] = new Tuple<string, bool>(key, traversed[key].Item2);
            }
            cycle[traversed.Count] = new Tuple<string, bool>(lastName, lastDev);
            return cycle;
        }

        /// <summary>
        /// Creates package node with set children and adds it to the children list of the root.
        /// Creates nodes for all dependencies not present under the root.
        /// </summary>
        private void addPackage(string name, JsonObject package, JsonObject packages) {
            if (!root.children.ContainsKey(name)) {
                root.children.Add(name, new Tuple<bool, Node>(
                    package["dev"],
                    new Node(name, package["version"])
                ));
            }
            Node packageNode = root.children[name].Item2;
            JsonObject packageDeps = package["dependencies"] as JsonObject;

            foreach (string dependencyName in packageDeps.Keys) {
                // Remove not installed package dependencies
                if (packages.ContainsKey(dependencyName)) {
                    // Adds dependency as a new child of the root
                    if (!root.children.ContainsKey(dependencyName)) {
                        JsonObject dependency = packages[dependencyName] as JsonObject;
                        root.children.Add(dependencyName, new Tuple<bool, Node>(
                            dependency["dev"],                              // Edge [Root -> Dependency]
                            new Node(dependencyName, dependency["version"]) // Dependency node
                        ));
                    }
                    Node dependencyNode = root.children[dependencyName].Item2;
                    packageNode.children.Add(dependencyName, new Tuple<bool, Node>(
                        packageDeps[dependencyName],                        // Edge [Package -> Dependency]
                        dependencyNode                                      // Dependency node refference
                    ));
                }
            }
        }
    }

    /// <summary>
    /// Class representing a single node in the dependency graph.
    /// </summary>
    class Node {
        public readonly Dictionary<string, Tuple<bool, Node>> children;
        public readonly string name;
        public readonly string version;

        public Node(string name, string version) {
            children = new Dictionary<string, Tuple<bool, Node>>();
            this.name = name;
            this.version = version;
        }
    }
}
