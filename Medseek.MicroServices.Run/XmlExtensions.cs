namespace Medseek.MicroServices.Run
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    internal static class XmlExtensions
    {
        internal static string Attrib(this XElement xe, string key)
        {
            var xa = xe.Attribute(XName.Get(key, string.Empty));
            return xa != null ? xa.Value : null;
        }

        internal static string ArgValue(this IEnumerable<string> args, string key)
        {
            var prefix = string.Format("/{0}=", key.TrimStart('/'));
            return args
                .Where(x => x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Substring(prefix.Length))
                .FirstOrDefault();
        }
    }
}