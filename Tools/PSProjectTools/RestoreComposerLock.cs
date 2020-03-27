using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Text;
using System.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.Build.Tasks {

    /// <summary>
    /// Simple task to restore Symfony project's composer.lock file from lockFragments inside nuget pakcages
    /// <summary>
    public sealed class RestoreComposerLock : Task {
        [Required]
        public ITaskItem ProjPath { get; set; }
        [Required]
        public ITaskItem[] LockFragments { get; set; }

        public override bool Execute() {
            JsonObject composer = getComposerJson(ProjPath.ItemSpec);
            if (composer == null) {
                return false;
            }
            string headerPart = getHeaderPart(composer);
            string packagesPart = getPackagesParts(LockFragments);
            string footerPart = getFooterPart(composer);
            using (StreamWriter sw = new StreamWriter(Path.Combine(ProjPath.ItemSpec, "composer.lock"))) {
                sw.Write(headerPart + packagesPart + footerPart);
            }
            return true;
        }

        private static string getHeaderPart(JsonObject composer) {
            string contentHash = getContentHash(composer);
            return $@"{{
    ""_readme"": [
        ""This file locks the dependencies of your project to a known state"",
        ""Read more about it at https://getcomposer.org/doc/01-basic-usage.md#installing-dependencies"",
        ""This file is @generated automatically""
    ],
    ""content-hash"": ""{contentHash}"",
";
        }

        /// <summary>
        /// Gets footer part of composer.lock file
        /// </summary>
        private static string getFooterPart(JsonObject composer) {
            JsonObject require = composer["require"] as JsonObject;
            IEnumerable<string> keys = require.Keys.Where(key => key == "php" || key.StartsWith("ext-"));
            StringBuilder platform = new StringBuilder();
            bool first = true;
            foreach (string key in keys) {
                platform.Append($"{(!first ? "," : "")}\"{key}\": {require[key].ToString()}");
                first = false;
            }
            return $@"
	""aliases"": [],
    ""minimum-stability"": ""stable"",
    ""stability-flags"": [],
    ""prefer-stable"": false,
    ""prefer-lowest"": false,
    ""platform"": {{{platform}}},
    ""platform-dev"": []
    }}
";
        }

        /// <summary>
        /// Processes lockFragments and output packages and packages-dev parts of compsoer.lock
        /// </summary>
        private static string getPackagesParts(ITaskItem[] lockFragments) {
            StringBuilder packages = new StringBuilder("\"packages\": [");
            StringBuilder packagesDev = new StringBuilder("\"packages-dev\": [");
            bool skipComma = true, skipCommaDev = true;
            bool addComma(bool first, StringBuilder sb) {
                if (!first) {
                    sb.Append(',');
                }
                return false;
            };

            foreach (ITaskItem lockFragment in lockFragments) {
                if (File.Exists(lockFragment.ItemSpec)) {
                    if (lockFragment.ItemSpec.EndsWith("-dev.json")) {
                        skipCommaDev = addComma(skipCommaDev, packagesDev);
                        packagesDev.Append(File.ReadAllText(lockFragment.ItemSpec));
                    } else {
                        skipComma = addComma(skipComma, packages);
                        packages.Append(File.ReadAllText(lockFragment.ItemSpec));
                    }
                }
            }
            packages.Append("],");
            packagesDev.Append("],");

            return packages.Append(packagesDev).ToString();
        }

        /// <summary>
        /// Searches provided path and all parent directories for composer.json and loads it
        /// </summary>
        private static JsonObject getComposerJson(string path) {
            DirectoryInfo di = path != String.Empty ? new DirectoryInfo(path) : null;

            while (di != null && !File.Exists(Path.Combine(di.FullName, "composer.json"))) {
                di = di.Parent;
            }
            if (di == null || !File.Exists(Path.Combine(di.FullName, "composer.json"))) {
                Console.WriteLine("composer.json file was not found!");
                return null;
            }

            string jsonText = File.ReadAllText(Path.Combine(di.FullName, "composer.json"));
            return JsonValue.Parse(jsonText) as JsonObject;
        }

        /// <summary>
        /// C-sharp version of Konstantin Kudryashiv's and Jordi Boggiano's Locker::GetContentHash
        /// </summary>
        private static string getContentHash(JsonObject composer) {
            StringBuilder relevantContent = new StringBuilder();
            string[] relevantKeys = new string[] {
                "conflict",
                "extra",
                "minimum-stability",
                "name",
                "prefer-stable",
                "provide",
                "replace",
                "repositories",
                "require",
                "require-dev",
                "version",
            };
            bool first = true;
            foreach (string key in relevantKeys) {
                if (composer.ContainsKey(key)) {
                    relevantContent.Append($"{(first ? "" : ",")}\"{key}\":{getSortedValue(composer[key])}");
                    first = false;
                }
            }
            string config = "{";
            if (composer.ContainsKey("config") && (composer["config"] as JsonObject).ContainsKey("platform")) {
                config += $"\"config\":{{\"platform\":{(composer["config"] as JsonObject)["platform"]}}},";
            }
            string content = config + relevantContent.ToString() + "}";
            using (MD5 md5Hash = MD5.Create()) {
                return GetMd5Hash(md5Hash, content.Replace("/", "\\/"));
            }
        }

        /// <summary>
        /// Returns stringyfied JsonValue recursively sorted by keys in case of JsonObject or JsonArray
        /// </summary>
        private static string getSortedValue(JsonValue val) {
            if (val.JsonType.Equals(JsonType.Boolean) || val.JsonType.Equals(JsonType.Number) || val.JsonType.Equals(JsonType.String)) {
                return val.ToString();
            }
            JsonObject obj = val as JsonObject;
            StringBuilder sorted = new StringBuilder(val.JsonType.Equals(JsonType.Object) ? "{" : "[");
            bool first = true;
            foreach (string key in obj.Keys.OrderBy(s => s)) {
                if (!first) {
                    sorted.Append(',');
                }
                sorted.Append($"\"{key}\":{getSortedValue(obj[key])}");
                first = false;
            }
            sorted.Append(val.JsonType.Equals(JsonType.Object) ? "}" : "]");
            return sorted.ToString();
        }

        /// <summary>
        /// Gets md5 hash of given input string
        /// </summary>
        private static string GetMd5Hash(MD5 md5Hash, string input) {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
