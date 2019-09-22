using System.Collections.Generic;
using Pchp.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PeachPied.Symfony.AspNetCore.Templating
{
    /// <summary>
    /// Class providing bridge between Razor supporting Environment and Razor rendering service
    /// </summary>
    class RazorRendererBridge
    {
        Context ctx;

        public RazorRendererBridge(Context ctx)
        {
            this.ctx = ctx;
        }

        /// <summary>
        /// Returns Razor view rendered into a string
        /// </summary>
        /// <param name="name">PhpValue</param>
        /// <param name="data">PhpArray</param>
        /// <returns>PhpValue</returns>
        public PhpValue renderRazor(PhpValue name, PhpArray data)
        {
            object httpCtx = ctx.GetType().GetProperty("HttpContext").GetValue(ctx, null);

            if (httpCtx is HttpContext) {
                HttpContext httpContext = (HttpContext)httpCtx;
                object viewRenderSvc = httpContext.RequestServices.GetService(typeof(IRazorRenderService));
                ViewDataDictionary viewDataDictionary = getData(data);
                string partialName;

                if (name.GetValue().IsString(out partialName)) {
                    var partial = ((IRazorRenderService)viewRenderSvc).RenderToString(partialName, viewDataDictionary);

                    if (partial != null) {
                        return partial;
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Provides utilities for converting twig data to razor comprehensable format
        /// </summary>
        /// <param name="data">PhpArray</param>
        /// <returns>ViewDataDictionary</returns>
        private static ViewDataDictionary getData(PhpArray data)
        {
            var viewDataDictionary = new ViewDataDictionary(
                new EmptyModelMetadataProvider(),
                new ModelStateDictionary()
            );
            viewDataDictionary.Model = null;
            foreach (var el in data) {
                string key = el.Key.IsInteger ? el.Key.Integer.ToString() : el.Key.String;

                if (!el.Value.IsArray) {
                    viewDataDictionary.Add(key, el.Value);
                } else {
                    viewDataDictionary.Add(key, resolveDataReccursive(el.Value.AsArray()));
                }
            }

            return viewDataDictionary;
        }

        /// <summary>
        /// Recursive part of Twig to Razor data conversion
        /// </summary>
        /// <param name="data">PhpArray</param>
        /// <returns>Dictionary</returns>
        private static Dictionary<string, object> resolveDataReccursive(PhpArray data)
        {
            Dictionary<string, object> output = new Dictionary<string, object>();
            foreach (var el in data) {
                string key = el.Key.IsInteger ? el.Key.Integer.ToString() : el.Key.String;

                if (!el.Value.IsArray) {
                    output.Add(key, el.Value);
                } else {
                    output.Add(key, resolveDataReccursive(el.Value.AsArray()));
                }
            }

            return output;
        }
    }
}
