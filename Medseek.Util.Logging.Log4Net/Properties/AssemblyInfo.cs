﻿using System.Reflection;
using System.Runtime.InteropServices;
using Medseek.Util.Properties;

[assembly: AssemblyDescription(UtilAssembly.Description + " (log4net plugin)")]
[assembly: AssemblyTitle(UtilAssembly.Title + ".Logging.Log4Net")]
[assembly: Guid("9c7c2aab-3ced-474a-aaa7-664a943d9b77")]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]