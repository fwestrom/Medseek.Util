namespace Medseek.Util.MicroServices
{
    using System;
    using System.Reflection;
    using Medseek.Util.Messaging;

    /// <summary>
    /// Describes a micro-service binding between a messaging system and a 
    /// component implementation.
    /// </summary>
    public class MicroServiceBinding
    {
        // for backwards compatibility, we need to default AutoDelete to true
        public MicroServiceBinding()
        {
            AutoDelete = true;
        }

        /// <summary>
        /// Gets or sets the messaging system address associated with the 
        /// binding.
        /// </summary>
        public MqAddress Address
        {
            get;
            set;
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
        /// Gets or sets the service type provided by the micro-service 
        /// component implementation.
        /// </summary>
        public Type Service
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the method associated with the micro-service binding.
        /// </summary>
        public MethodInfo Method
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
        /// Gets or sets a value indicating whether the queue specified in this binding should be set to auto-delete
        /// </summary>
        public bool AutoDelete
        {
            get; 
            set;
        }

    }
}