using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PeachPied.Symfony.AspNetCore;

namespace empty_page.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // make sure cwd is not app\ but its parent:
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();

            // add symfony into the pipeline
            // using default configuration from appsettings.json (IConfiguration), section Symfony
            // using empty set of .NET plugins
            app.UseSymfony();

            app.UseDefaultFiles();


            /*
            app.UseSession();

            app.UseMvc();
            app.UsePhp(new PhpRequestOptions(scriptAssemblyName: "symfony.skeleton"));
            app.UseDefaultFiles();
            app.UseStaticFiles();
            */
        }
    }
}