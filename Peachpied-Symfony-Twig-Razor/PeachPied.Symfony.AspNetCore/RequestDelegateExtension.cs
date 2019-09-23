using System;
using Pchp.Core;
using PeachPied.Symfony.AspNetCore;
using PeachPied.Symfony.AspNetCore.Internal;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Rewrite;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/> extension for enabling Symfony.
    /// </summary>
    public static class RequestDelegateExtension
    {
        static SymfonyConfig bootstrapConfig;

        /// <summary>
        /// Function overwritting functionality in bootstrap.php
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="locals">PhpArray</param>
        /// <param name="this">object</param>
        /// <param name="self">RuntimeTypeHandle</param>
        /// <returns></returns>
        static PhpValue BootstrapMain(Context ctx, PhpArray locals, object @this, RuntimeTypeHandle self)
        {
            ctx.Include("vendor", "autoload.php", true);
            Apply(ctx, bootstrapConfig);
            return 0;
        }

        /// <summary>
        /// Defines Symfony configuration constants and initializes runtime before proceeding to <c>index.php</c>.
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="config">SymfonyConfig</param>
        static void Apply(Context ctx, SymfonyConfig config)
        {
            // see .env
            ctx.Server["APP_ENV"] = ctx.Env["APP__ENV"] = (PhpValue)config.AppEnv; 
            ctx.Server["APP_SECRET"] = (PhpValue)config.AppSecret;
            ctx.Server["APP_DEBUG"] = ctx.Env["APP_DEBUG"] = (PhpValue)config.AppDebug;
        }

        /// <summary>
        /// Installs Symfony middleware.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="config">Symfony instance configuration.</param>
        /// <param name="path">Physical location of symfony folder. Can be absolute or relative to the current directory.</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseSymfony
            (this IApplicationBuilder app, SymfonyConfig config = null, string path = "Symfony.Skeleton")
        {
            // symony root path:
            var symfonyRoot = System.IO.Path.GetFullPath(path);
            var fproviderB = new PhysicalFileProvider(symfonyRoot);

            if (config == null) {
                bootstrapConfig = SfConfigurationLoader
                    .CreateDefault()
                    .LoadFromSettings(app.ApplicationServices, path);
            } else {
                bootstrapConfig = config;
            }

            // setup URL rewriting as Symfony projects' servers operate from /public folder
            var options = new RewriteOptions()
                .AddRewrite(
                    @"((.*\/)*([^\/\?&]*\.[^\/\?&]*))",
                    bootstrapConfig.PublicDir + "/$1",
                    skipRemainingRules: true
                )
                .AddRewrite(
                    @"^(?!"+ bootstrapConfig.PublicDir + "/)(.*)$", 
                    bootstrapConfig.PublicDir, 
                    skipRemainingRules: true
                );

            app.UseRewriter(options);

            // handling php files:
            app.UsePhp(new PhpRequestOptions(scriptAssemblyName: path) {
                RootPath = symfonyRoot,
            });

            // symfony's static files
            app.UseStaticFiles(new StaticFileOptions() { FileProvider = fproviderB });
            
            // bootstrap.php env loading overriding
            string bootstrapPath = "config\\bootstrap.php";
            Context.MainDelegate md = new Context.MainDelegate(BootstrapMain);
            Context.DeclareScript(bootstrapPath, md);

            return app;
        }
    }
}
