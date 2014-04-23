namespace Medseek.Util.MicroServices
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;
    using Medseek.Util.Messaging;
    using Medseek.Util.Objects;

    /// <summary>
    /// Provides the ability to invoke a remote micro-service.
    /// </summary>
    [Register(typeof(IRemoteMicroServiceInvoker), Lifestyle = Lifestyle.Transient)]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "MQ = Message Queuing System")]
    public class RemoteMicroServiceInvoker : Disposable, IRemoteMicroServiceInvoker
    {
        private readonly IMicroServiceDispatcher dispatcher;
        private readonly IMessageContextAccess messageContextAccess;
        private readonly IMicroServiceSerializer serializer;
        private TimeSpan timeout = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Initializes a new instance of the <see 
        /// cref="RemoteMicroServiceInvoker"/> class.
        /// </summary>
        public RemoteMicroServiceInvoker(
            IMicroServiceDispatcher dispatcher, 
            IMessageContextAccess messageContextAccess, 
            IMicroServiceSerializer serializer)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");
            if (messageContextAccess == null)
                throw new ArgumentNullException("messageContextAccess");
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            this.dispatcher = dispatcher;
            this.messageContextAccess = messageContextAccess;
            this.serializer = serializer;
        }

        /// <summary>
        /// Gets or sets the timeout used by remote micro-service invocation 
        /// message transmission operations.
        /// </summary>
        public TimeSpan Timeout
        {
            get
            {
                return timeout > TimeSpan.Zero ? timeout : TimeSpan.Zero;
            }
            set
            {
                timeout = value;
            }
        }

        /// <summary>
        /// Sends a message to a remote micro-service.
        /// </summary>
        /// <param name="address">
        /// The address of the micro-service to which the message should be 
        /// sent.
        /// </param>
        /// <param name="body">
        /// The message body.
        /// </param>
        /// <param name="properties">
        /// The message properties.
        /// </param>
        public void Send(MqAddress address, byte[] body, MessageProperties properties)
        {
            ThrowIfDisposed();
            if (address == null)
                throw new ArgumentNullException("address");
            if (body == null)
                throw new ArgumentNullException("body");
            if (properties == null)
                throw new ArgumentNullException("properties");

            dispatcher.Send(address, body, properties);
        }

        /// <summary>
        /// Sends a message to a remote micro-service.
        /// </summary>
        /// <param name="address">
        /// The address of the micro-service to which the message should be 
        /// sent.
        /// </param>
        /// <param name="bodyType">
        /// The type of object to serialize in the message body.
        /// </param>
        /// <param name="bodyValue">
        /// The object to use for the message body.
        /// </param>
        /// <param name="properties">
        /// The message properties.
        /// </param>
        public void Send(MqAddress address, Type bodyType, object bodyValue, MessageProperties properties)
        {
            var body = serializer.Serialize(properties.ContentType, bodyType, bodyValue);
            Send(address, body, properties);
        }

        /// <summary>
        /// Invokes the bound method that provides the micro-service operation.
        /// </summary>
        /// <param name="binding">
        /// The micro-service binding description identifying the micro-service
        /// invocation to perform.
        /// </param>
        /// <param name="parameters">
        /// The values to pass as the parameters to the micro-service.
        /// </param>
        public void Send(MicroServiceBinding binding, params object[] parameters)
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

            using (var ms = new MemoryStream())
            using (messageContextAccess.Enter())
            {
                var messageContext = messageContextAccess.Current;
                var properties = messageContext.Properties;

                var contentType = messageContext.Properties.ContentType;
                for (var i = 0; i < parameterTypes.Length; i++)
                {
                    var parameterType = parameterTypes[i];
                    var parameterValue = parameters[i];
                    serializer.Serialize(contentType, parameterType, parameterValue, ms);
                }

                var body = ms.ToArray();
                Send(binding.Address, body, properties);
            }
        }
    }
}