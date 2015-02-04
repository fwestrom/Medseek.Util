namespace Medseek.Util.MicroServices
{
    using System;
    using System.Reflection;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Describes a micro-service method binding to a topic exchange.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
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
        public MicroServiceBindingAttribute(string exchange, string bindingKey, string queue = null)
            : this(string.Format("topic://{0}/{1}", exchange, bindingKey) + (!string.IsNullOrWhiteSpace(queue) ? "/" + queue : string.Empty))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="MicroServiceBindingAttribute" /> class.
        /// </summary>
        /// <param name="addressValue">
        /// The string value to use for the binding address.
        /// </param>
        public MicroServiceBindingAttribute(string addressValue)
        {
            Address = new MqAddress(addressValue);
            AutoDelete = true;
        }

        /// <summary>
        /// Gets the messaging system address associated with the binding.
        /// </summary>
        public MqAddress Address
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether automatic message 
        /// acknowledgement is disabled.
        /// </summary>
        public bool AutoAckDisabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is one way].
        /// </summary>
        public bool IsOneWay
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [to auto delete]
        /// </summary>
        public bool AutoDelete
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a micro-service binding description for the specified 
        /// method marked by the attribute.
        /// </summary>
        /// <typeparam name="T">
        /// The type of micro-service binding object to return.
        /// </typeparam>
        public T ToBinding<T>(MethodInfo method, Type service)
            where T : MicroServiceBinding, new()
        {
            return new T
            {
                Address = Address,
                AutoAckDisabled = AutoAckDisabled,
                Method = method, 
                Service = service,
                IsOneWay = IsOneWay,
                AutoDelete = AutoDelete
            };
        }
    }
}