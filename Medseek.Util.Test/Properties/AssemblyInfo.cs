using System.Reflection;
using System.Runtime.InteropServices;
using Medseek.Util.MicroServices;
using Medseek.Util.Properties;

[assembly: AssemblyDescription(UtilAssembly.Description + " (unit test assembly)")]
[assembly: AssemblyTitle(UtilAssembly.Title + ".Test")]
[assembly: Guid("28305f39-3320-450a-9014-a61550d32809")]

[assembly:MicroServiceBindingDefaults("assembly-exchange", "assembly-exchangeType", "assembly-prefix")]