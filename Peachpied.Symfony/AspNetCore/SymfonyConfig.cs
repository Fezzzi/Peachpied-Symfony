using System;

namespace PeachPied.Symfony.AspNetCore {
    /// <summary>
    /// Configuration is loaded into Symfony before every request
    /// </summary>
    public class SymfonyConfig {
        // DEFAULTS

        /// <summary>Decides whether to override Symfony's .env configuration with appsettings.</summary>
        public bool OverrideEnv { get; set; } = false;

        /// <summary>Application environment.</summary>
        public string AppEnv { get; set; } = "dev";

        /// <summary>Application secret.</summary>
        /// <remarks>
        /// Change this to different unique phrase in appsettings.json!
        /// You can generate it using Symfony 2 Secret Generator - http://nux.net/secret
        /// You can change these at any point in time. 
        /// After changing this value, you should regenerate the application cache and log out all the application users.
        /// </remarks>
        public string AppSecret { get; set; } = "ThisIsNotASecret";

        /// <summary>Application environment.</summary>
        public string AppDebug { get; set; } = "0";

        /// <summary>Path to pubic directory</summary>
        public string PublicDir { get; set; } = "public";

        public Tuple<string, string>[] SymfonyEnvVarsMap { get; } = {
            new Tuple<string, string>("AppEnv","APP_ENV"),
            new Tuple<string, string>("AppSecret","APP_SECRET"),
            new Tuple<string, string>("AppDebug","APP_DEBUG")
        };
    }
}
