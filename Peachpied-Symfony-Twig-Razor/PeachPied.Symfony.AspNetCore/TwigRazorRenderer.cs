using System.Collections.Generic;
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

namespace App.twig
{
    class RazorRenderer : Twig.Extension.AbstractExtension
    {
        public RazorRenderer(Context ctx) : base(ctx) { }

        public Twig.TwigFunction[] getFunctions(Context ctx)
        {
            return new Twig.TwigFunction[1] {
                new Twig.TwigFunction(ctx, "renderRazor", PhpValue.Null),
            };
        }

        public string renderRazor(string path, string name)
        {
            return "Razor template";
        }
    }
}
