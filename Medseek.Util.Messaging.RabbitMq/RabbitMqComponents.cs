namespace Medseek.Util.Messaging.RabbitMq
{
    using System;

    /// <summary>
    /// Provides information about the RabbitMQ utility components.
    /// </summary>
    [Obsolete("RabbitMqComponents will be removed in a future release; please use RabbitMqPlugin directly instead.")]
    public class RabbitMqComponents : RabbitMqPlugin
    {
        private static readonly RabbitMqPlugin ThePlugin = new RabbitMqPlugin();

        /// <summary>
        /// Prevents a default instance of the <see 
        /// cref="RabbitMqComponents" /> class from being created.
        /// </summary>
        private RabbitMqComponents()
        {
        }

        /// <summary>
        /// Gets the plugin that provides pluggable functionality using the 
        /// Castle project integration components.
        /// </summary>
        public static RabbitMqPlugin Plugin
        {
            get
            {
                return ThePlugin;
            }
        }
    }
}