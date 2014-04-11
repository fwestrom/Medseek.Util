namespace Medseek.Util.MicroServices.Host
{
    using System;

    /// <summary>
    /// Used to mark an assembly, module, or class that may be hosted by the 
    /// micro-service hosting application, such that the build-time assembly 
    /// resolver identifies the dependency on the hosting application project.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Module, AllowMultiple = true)]
    public class ReferenceMicroServiceHostAttribute : Attribute
    {
    }
}