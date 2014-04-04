namespace Medseek.Util.MicroServices
{
    using System;
    using System.IO;
    using System.Linq;
    using Medseek.Util.Messaging;
    using Medseek.Util.Objects;

    /// <summary>
    /// Provides the ability to invoke a remote micro-service.
    /// </summary>
    public class RemoteMicroServiceInvoker : Disposable, IMicroServiceInvoker
    {
        private readonly MicroServiceBinding binding;
        private readonly IMqChannel channel;
        private readonly IMessageContextAccess messageContextAccess;
        private readonly IMicroServiceSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="RemoteMicroServiceInvoker"/> class.
        /// </summary>
        public RemoteMicroServiceInvoker(
            MicroServiceBinding binding, 
            IMqChannel channel, 
            IMessageContextAccess messageContextAccess, 
            IMicroServiceSerializer serializer)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");
            if (channel == null)
                throw new ArgumentNullException("channel");
            if (messageContextAccess == null)
                throw new ArgumentNullException("messageContextAccess");
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            this.binding = binding;
            this.channel = channel;
            this.messageContextAccess = messageContextAccess;
            this.serializer = serializer;
        }

        /// <summary>
        /// Gets the micro-service binding description associated with the 
        /// invoker.
        /// </summary>
        public MicroServiceBinding Binding
        {
            get
            {
                return binding;
            }
        }

        /// <summary>
        /// Invokes the bound method that provides the micro-service operation.
        /// </summary>
        /// <param name="parameters">
        /// The values to pass as the method parameters.
        /// </param>
        /// <returns>
        /// The return value produced by the method, or null if the method 
        /// has a void return type.
        /// </returns>
        public object Invoke(object[] parameters)
        {
            ThrowIfDisposed();
            var parameterTypes = binding.Method.GetParameters().Select(x => x.ParameterType).ToArray();
            if (parameters.Length != parameterTypes.Length)
                throw new ArgumentException("Incorrect number of parameter values specified for method " + binding.Method + ".");
            if (parameters.Length > 1)
                throw new NotSupportedException();

            var waitForReply = binding.Method.ReturnType != typeof(void) && !binding.IsOneWay;
            if (waitForReply)
                throw new NotImplementedException("Support for waiting for a reply is not yet available.");

            var address = channel.Plugin.ToPublisherAddress(binding.Address);
            using (var publisher = channel.CreatePublisher(address))
            using (var ms = new MemoryStream())
            using (messageContextAccess.Enter())
            {
                var messageContext = messageContextAccess.Current;
                var properties = messageContext.Properties;
                properties.ReplyTo = null;
                properties.RoutingKey = address.RoutingKey;

                object serializerState = null;
                foreach (var parameterType in parameterTypes)
                {
                    var parameterValue = parameters.Single();
                    serializer.Serialize(messageContext, parameterType, parameterValue, ms);
                }

                var body = ms.ToArray();
                publisher.Publish(body, properties);

                // TODO: Wait for reply if appropriate.
            }

            return null;
        }
    }
}