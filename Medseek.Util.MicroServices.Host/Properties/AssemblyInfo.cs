using System.Reflection;
using System.Runtime.InteropServices;
using Medseek.Util.Properties;
using Medseek.Util.Serialization.Newtonsoft.Json;

[assembly: AssemblyDescription(UtilAssembly.Description + " (micro-services hosting application)")]
[assembly: AssemblyTitle(UtilAssembly.Title + ".MicroServices.Host")]
[assembly: Guid("78ff3652-0e7c-42c6-be73-e718b9a7ea14")]
[assembly: ReferencePluginNewtonsoftJson]