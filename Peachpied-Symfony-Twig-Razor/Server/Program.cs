using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using PeachPied.Symfony.AspNetCore.Templating;

namespace Peachpied.Symfony.Twig.Razor.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // make sure cwd is not server\ but its parent:
            if (Path.GetFileName(Directory.GetCurrentDirectory()) == "Server")
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Directory.GetCurrentDirectory()));
            }

            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:5004/")
                .Build();

            host.Run();
        }
    }
	
	class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add the ViewRenderService
            services.AddHttpContextAccessor();
            services.AddTransient<IRazorRenderService, RazorRenderService>();

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
			// Includes support for Razor Pages and controllers.
			services.AddMvc();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSession();
            app.UseMvc();

            // c-sharp root path and static files
            var root = Path.GetFullPath("Server");
            var fprovider = new PhysicalFileProvider(root);
            app.UseStaticFiles(new StaticFileOptions() { FileProvider = fprovider });

            app.UseSymfony(null, "twig-razor-page");
            app.UseDefaultFiles();
        }
    }
}