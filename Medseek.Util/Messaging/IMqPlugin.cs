namespace Medseek.Util.Messaging
{
    using Medseek.Util.Ioc;

    /// <summary>
    /// Interface for types that provide the pluggable functionality for 
    /// integrating with a messaging middleware system.
    /// </summary>
    public interface IMqPlugin : IInstallable
    {
    }
}