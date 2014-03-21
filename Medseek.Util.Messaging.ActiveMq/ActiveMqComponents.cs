namespace Medseek.Util.Messaging.ActiveMq
{
    using Medseek.Util.Ioc;

    /// <summary>
    /// Provides information about the ActiveMQ utility components.
    /// </summary>
    public class ActiveMqComponents : ComponentsInstallable
    {
        /// <summary>
        /// The name of the default ActiveMQ connection component.
        /// </summary>
        public const string DefaultConnection = "Medseek.Util.Messaging.ActiveMq.ActiveMqConnection.Default";
    }
}