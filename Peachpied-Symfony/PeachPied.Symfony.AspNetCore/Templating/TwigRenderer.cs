using Pchp.Core;
using System.Collections.Generic;

namespace PeachPied.Symfony.AspNetCore.Templating
{
    /// <summary>
    /// Class providing Twig templates within Razor rendering
    /// </summary>
    public class TwigRenderer
    {
        /// <summary>
        /// Base Twig rendering function
        /// </summary>
        /// <param name="path">string</param>
        /// <param name="name">string</param>
        /// <returns>string</returns>
        public static string RenderTwig(string path, string name)
        {
            return RenderTwig(path, name, new PhpArray());
        }

        /// <summary>
        /// Base Twig rendering function
        /// </summary>
        /// <param name="path">string</param>
        /// <param name="name">string</param>
        /// <param name="data">PhpArray</param>
        /// <returns>string</returns>
        public static string RenderTwig(string path, string name, PhpArray data)
        {
            using (var ctx = Context.CreateEmpty()) {
                ctx.RootPath = System.IO.Path.GetFullPath("twig-razor-page");
                var loader = new Twig.Loader.FilesystemLoader(ctx, path);
                var twig = new Twig.Environment(ctx, loader);

                return twig.render(name, data).ToString();
            }
        }

        /// <summary>
        /// Provides utilities for converting razor data to twig comprehensable format
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">KeyValuePair</param>
        /// <returns>PhpArray</returns>
        public static PhpArray DataToPhp<T>(KeyValuePair<string, T> value) where T : struct
        {
            PhpArray res = new PhpArray(1);
            res.Add(value.Key, value.Value);
            return res;
        }

        /// <summary>
        /// Provides utilities for converting razor data to twig comprehensable format
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="value">KeyValuePair</param>
        /// <returns>PhpArray</returns>
        public static PhpArray DataToPhp<T>(KeyValuePair<int, T> value) where T : struct
        {
            PhpArray res = new PhpArray(1);
            res.Add(value.Key, value.Value);
            return res;
        }

        /// <summary>
        /// Provides utilities for converting razor data to twig comprehensable format
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="key">IntStringKey</param>
        /// <param name="value">KeyValuePair</param>
        /// <returns>PhpArray</returns>
        public static PhpArray DataToPhp<T>(IntStringKey key, KeyValuePair<string, T>[] value) where T : struct
        {
            PhpArray val = new PhpArray(value.Length);
            foreach (var v in value) {
                val.Add(v.Key, v.Value);
            }

            PhpArray res = new PhpArray(1);
            res.Add(key, val);
            return res;
        }

        /// <summary>
        /// Provides utilities for converting razor data to twig comprehensable format
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="key">IntStringKey</param>
        /// <param name="value">KeyValuePair</param>
        /// <returns>PhpArray</returns>
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

        /// <summary>
        /// Provides utilities for converting razor data to twig comprehensable format
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="key">IntStringKey</param>
        /// <param name="value">T?</param>
        /// <returns>PhpArray</returns>
        public static PhpArray DataToPhp<T>(IntStringKey key, T? value) where T : struct
        {
            PhpArray res = new PhpArray(1);
            res.Add(key, PhpValue.Create(value));
            return res;
        }

        /// <summary>
        /// Provides utilities for converting razor data to twig comprehensable format
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="key">IntStringKey</param>
        /// <param name="value">T[]</param>
        /// <returns>PhpArray</returns>
        public static PhpArray DataToPhp<T>(IntStringKey key, T[] value)
        {
            PhpArray res = new PhpArray(1);
            res.Add(key, new PhpArray(value));
            return res;
        }

        /// <summary>
        /// Provides utility for combining twig comprehensable data into single unit
        /// </summary>
        /// <param name="arrs">PhpArray[]</param>
        /// <returns>PhpArray</returns>
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
