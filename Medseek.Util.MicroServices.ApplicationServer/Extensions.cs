namespace Medseek.Util.MicroServices.ApplicationServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using Medseek.Util.Interactive;
    using Medseek.Util.Logging;

    internal static class Extensions
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal static string Attrib(this XElement xe, string key)
        {
            var xa = xe.Attribute(XName.Get(key, string.Empty));
            return xa != null ? xa.Value : null;
        }

        internal static string ArgValue(this IEnumerable<string> args, string key)
        {
            var prefix = string.Format("/{0}=", key.TrimStart('/'));
            Log.DebugFormat("ArgValue(key={0})", key);
            return args
                .Where(x => x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Substring(prefix.Length))
                .Do(x => Log.DebugFormat("ArgValue: value = {0}", x))
                .FirstOrDefault();
        }

        internal static Dictionary<string, string> ToArgDictionary(this IEnumerable<string> args)
        {
            return args.Where(x => x.StartsWith("/") && x.Contains("="))
                        .Select(x => x.TrimStart('/').Split('='))
                        .ToDictionary(x => x[0], x => x[1]);
        }

        internal static string Get(this Dictionary<string, string> dict, string key)
        {
            string value = null;
            dict.TryGetValue(key, out value);
            return value;
        }
    }
}