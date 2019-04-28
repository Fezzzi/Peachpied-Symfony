﻿using System;
using Pchp.Core;
using Microsoft.AspNetCore.Builder;
using PeachPied.Symfony.AspNetCore.Internal;
using Microsoft.Extensions.FileProviders;
using Peachpie.AspNetCore.Web;

namespace PeachPied.Symfony.AspNetCore
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/> extension for enabling Symfony.
    /// </summary>
    public static class RequestDelegateExtension
    {
        static SymfonyConfig bootstrapConfig;

        static PhpValue BootstrapMain(Context ctx, PhpArray locals, object @this, RuntimeTypeHandle self)
        {
            Apply(ctx, bootstrapConfig);
            return 0;
        }

        /// <summary>
        /// Defines Symfony configuration constants and initializes runtime before proceeding to <c>index.php</c>.
        /// </summary>
        static void Apply(Context ctx, SymfonyConfig config)
        {
            // see .env
            ctx.Server["APP_ENV"] = (PhpValue)config.AppEnv; // APP_ENV=dev;
            ctx.Server["APP_SECRET"] = (PhpValue)config.AppSecret; // APP_SECRET=ThisIsNotASecret;
            ctx.Server["APP_DEBUG"] = (PhpValue)config.AppDebug; //APP_DEBUG=0;
        }

        /// <summary>
        /// Installs Symfony middleware.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="config">Symfony instance configuration.</param>
        /// <param name="plugins">Container describing what plugins will be loaded.</param>
        /// <param name="path">Physical location of symfony folder. Can be absolute or relative to the current directory.</param>
        public static IApplicationBuilder UseSymfony(this IApplicationBuilder app, SymfonyConfig config = null, string path = "Website")
        {
            // symony root path:
            var root = System.IO.Path.GetFullPath(path);
            var fprovider = new PhysicalFileProvider(root);

            if (config == null) {
                bootstrapConfig = SfConfigurationLoader
                    .CreateDefault()
                    .LoadFromSettings(app.ApplicationServices);
            } else {
                bootstrapConfig = config;
            }

            // bootstrap.php env loading overriding
            string bootstrapPath = "/config/bootstrap.php";

            var infos = System.Linq.Enumerable.Empty<Pchp.Core.Context.ScriptInfo>();
            Context.TryGetScriptsInDirectory(root, bootstrapPath, out infos);
            Pchp.Core.Context.ScriptInfo info = Context.TryGetDeclaredScript(bootstrapPath);
            Context.MainDelegate md = new Context.MainDelegate(BootstrapMain);
            Context.DeclareScript(bootstrapPath, md);
            Pchp.Core.Context.ScriptInfo info2 = Context.TryGetDeclaredScript(bootstrapPath);

            // handling php files:
            app.UsePhp(new PhpRequestOptions(scriptAssemblyName: "Symfony.Skeleton")
            {
                RootPath = root,
            });

            // static files
            app.UseStaticFiles(new StaticFileOptions() { FileProvider = fprovider });

            return app;
        }
    }
}