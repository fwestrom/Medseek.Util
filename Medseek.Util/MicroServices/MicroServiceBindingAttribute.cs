namespace Medseek.Util.MicroServices
{
    using System;
    using System.Reflection;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Describes a micro-service method binding to a topic exchange.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MicroServiceBindingAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="MicroServiceBindingAttribute" /> class.
        /// </summary>
        /// <param name="exchange">
        /// The name of the exchange.
        /// </param>
        /// <param name="bindingKey">
        /// The binding key to use when binding the queue to the exchange.
        /// </param>
        /// <param name="queue">
        /// The name of the queue.
        /// </param>
        public MicroServiceBindingAttribute(string exchange, string bindingKey, string queue)
        {
            BindingKey = bindingKey;
            Exchange = exchange;
            Queue = queue;
        }

        /// <summary>
        /// Gets or sets the binding key to use when binding the queue to the
        /// exchange.
        /// </summary>
        public string BindingKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the exchange.
        /// </summary>
        public string Exchange
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        public string Queue
        {
            get; 
            set;
        }

        /// <summary>
        /// Returns a micro-service binding description for the specified 
        /// method marked by the attribute.
        /// </summary>
        public MicroServiceBinding ToBinding(MethodInfo method, Type service)
        {
            return new MicroServiceBinding
            {
                Address = new MqAddress(string.Format("{0}://{1}/{2}/{3}", "topic", Exchange, BindingKey, Queue)),
                Method = method, 
                Service = service,
            };
        }
    }
}