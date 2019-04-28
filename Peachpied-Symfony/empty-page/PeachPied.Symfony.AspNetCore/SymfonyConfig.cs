using System;

namespace PeachPied.Symfony.AspNetCore
{
    /// <summary>
    /// Symfony configuration.
    /// The configuration is loaded into Symfony before every request.
    /// </summary>
    public class SymfonyConfig
    {
        // DEFAULTS

        /// <summary>Application environment.</summary>
        public string AppEnv { get; set; } = "dev";

        /// <summary>Application secret.</summary>
        /// <remarks>
        /// Change this to different unique phrases!
        /// You can generate it using Symfony 2 Secret Generator - http://nux.net/secret
        /// You can change these at any point in time. 
        /// After changing this value, you should regenerate the application cache and log out all the application users.
        /// </remarks>
        public string AppSecret { get; set; } = "ThisIsNotASecret";

        /// <summary>Application environment.</summary>
        public string AppDebug { get; set; } = "0";
    }
}
