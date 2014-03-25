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
    }
}