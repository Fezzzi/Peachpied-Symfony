using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;

namespace PeachPied.Symfony.AspNetCore.Templating
{
    /// <summary>
    /// Class providing service for Razor into string rendering
    /// </summary>
    public class RazorRenderService : IRazorRenderService
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RazorRenderService(IRazorViewEngine viewEngine, IHttpContextAccessor httpContextAccessor,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _httpContextAccessor = httpContextAccessor;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Outputs Razor view identified with provided path rendered into string
        /// </summary>
        /// <param name="viewPath">string</param>
        /// <param name="data">ViewDataDictionary</param>
        /// <returns>string</returns>
        public string RenderToString(string viewPath, ViewDataDictionary data)
        {
            try {
                var viewEngineResult = _viewEngine.GetView("~/", viewPath, false);

                if (!viewEngineResult.Success) {
                    throw new InvalidOperationException($"Couldn't find view {viewPath}");
                }

                var view = viewEngineResult.View;
                using (var sw = new StringWriter()) {
                    var viewContext = new ViewContext() {
                        HttpContext = _httpContextAccessor.HttpContext ?? new DefaultHttpContext { RequestServices = _serviceProvider },
                        ViewData = data,
                        Writer = sw
                    };

                    view.RenderAsync(viewContext).GetAwaiter().GetResult();
                    return sw.ToString();
                }

            } catch (Exception) {
                return "";
            }
        }
    }
}
