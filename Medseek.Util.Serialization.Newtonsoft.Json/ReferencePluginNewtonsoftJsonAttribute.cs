namespace Medseek.Util.Serialization.Newtonsoft.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Medseek.Util.Ioc;

    /// <summary>
    /// Used to mark an assembly, module, or class that requires the Newtonsoft.Json Utility Library Plugin.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Module, AllowMultiple = true)]
    public class ReferencePluginNewtonsoftJsonAttribute : ReferencePluginAttribute
    {
    }
}