using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text;
using Pchp.Core;

namespace PeachPied.Symfony.AspNetCore
{
    public class TwigRenderer
    {
        public static string RenderTwig(string path, string name)
        {
            return RenderTwig(path, name, new PhpArray());
        }

        public static string RenderTwig(string path, string name, PhpArray data)
        {
            using (var ctx = Context.CreateEmpty())
            {
                ctx.RootPath = System.IO.Path.GetFullPath("twig-razor-page");
                var loader = new Twig.Loader.FilesystemLoader(ctx, path);
                var twig = new Twig.Environment(ctx, loader);

                return twig.render(name, data).ToString();
            }
        }

        public static PhpArray DataToPhp<T>(KeyValuePair<string, T> value) where T: struct
        {
            PhpArray res = new PhpArray(1);
            res.Add(value.Key, value.Value);
            return res;
        }

        public static PhpArray DataToPhp<T>(KeyValuePair<int, T> value) where T : struct
        {
            PhpArray res = new PhpArray(1);
            res.Add(value.Key, value.Value);
            return res;
        }

        public static PhpArray DataToPhp<T>(IntStringKey key, KeyValuePair<string, T>[] value) where T: struct
        {
            PhpArray val = new PhpArray(value.Length);
            foreach (var v in value) {
                val.Add(v.Key, v.Value);
            }

            PhpArray res = new PhpArray(1);
            res.Add(key, val);
            return res;
        }

        public static PhpArray DataToPhp<T>(IntStringKey key, KeyValuePair<int, T>[] value) where T : struct
        {
            PhpArray val = new PhpArray(value.Length);
            foreach (var v in value) {
                val.Add(v.Key, v.Value);
            }

            PhpArray res = new PhpArray(1);
            res.Add(key, val);
            return res;
        }

        public static PhpArray DataToPhp<T>(IntStringKey key, T? value) where T : struct
        {
            PhpArray res = new PhpArray(1);
            res.Add(key, PhpValue.Create(value));
            return res;
        }

        public static PhpArray DataToPhp<T>(IntStringKey key, T[] value)
        {
            PhpArray res = new PhpArray(1);
            res.Add(key, new PhpArray(value));
            return res;
        }

        public static PhpArray MergePhpArrays(params PhpArray[] arrs)
        {
            if (arrs.Length > 0) {
                PhpArray res = PhpArray.Empty;

                foreach (PhpArray r in arrs) {
                    res = PhpArray.Union(res, r);
                }
                return res;
            }

            return null;
        }
    }
}

namespace Twig
{
    /// <summary>
    /// Razor Supporting Environment
    /// </summary>
    public class RSEnvironment : Environment
    {
        public RSEnvironment(Context ctx, Loader.LoaderInterface loader) : base(ctx, loader) {
            base.addExtension(new RazorRenderer(ctx));
        }

        public RSEnvironment(Context ctx, Loader.LoaderInterface loader, PhpValue options) : base(ctx, loader, options) {
            base.addExtension(new RazorRenderer(ctx));
        }
    }

    public class RazorRenderer : Extension.AbstractExtension
    {
        Context ctx;

        public RazorRenderer(Context ctx) : base(ctx) {
            this.ctx = ctx;
        }

        public override PhpValue getFunctions()
        {
            PhpValue[] obj = new PhpValue[2] {PhpValue.Create("render_razor"), default(PhpValue)};
            PhpArray val2 = new PhpArray(2) { PhpValue.FromClass((object)this), PhpValue.Create("renderRazor")};
            obj[1] = PhpValue.Create(val2);

            PhpArray funcs = new PhpArray(1);
            funcs.Add(PhpValue.FromClass((object)ctx.Create<TwigFunction>(typeof(RazorRenderer).TypeHandle, obj)));
            return PhpValue.Create(funcs);
        }

        public PhpValue renderRazor(PhpValue name, PhpArray data)
        {
            /*ViewPage viewPage = new ViewPage() { ViewContext = new ViewContext() };

            viewPage.ViewData = new ViewDataDictionary(viewData);
            viewPage.Controls.Add(viewPage.LoadControl(name));

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (HtmlTextWriter tw = new HtmlTextWriter(sw))
                {
                    viewPage.RenderControl(tw);
                }
            }

            return sb.ToString();*/
            return "test is working";
        }
    }
}
