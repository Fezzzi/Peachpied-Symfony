﻿using Pchp.Core;
using PeachPied.Symfony.AspNetCore.Templating;

namespace Twig {
	
    /// <summary>
    /// Razor Supporting Environment
    /// </summary>
    public class RSEnvironment : Environment {
        public RSEnvironment(Context ctx, Loader.LoaderInterface loader) : base(ctx, loader) {
            base.addFunction(new TwigFunction(ctx, "render_razor", getCallable(ctx)));
        }

        public RSEnvironment(Context ctx, Loader.LoaderInterface loader, PhpValue options) : base(ctx, loader, options) {
            base.addFunction(new TwigFunction(ctx, "render_razor", getCallable(ctx)));
        }

        /// <summary>
        /// Gets callable for Twig's Razor rendering function from bridging class
        /// </summary>
        private static PhpValue getCallable(Context ctx) {
            return PhpValue.Create(new PhpArray(2) {
                PhpValue.FromClass(new RazorRendererBridge(ctx)),
                PhpValue.Create("RenderRazor")
            });
        }
    }
}