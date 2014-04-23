namespace Medseek.Util.MicroServices.Lookup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Medseek.Util.Ioc;
    using Medseek.Util.Logging;
    using Medseek.Util.Messaging;
    using Medseek.Util.Objects;

    /// <summary>
    /// Provides micro-service lookup functionality for translating addressing 
    /// information at runtime to the actual values that should be used to 
    /// communicate with the active services.
    /// </summary>
    [RegisterMicroService(typeof(IMicroServiceLookup), Lifestyle = Lifestyle.Singleton, Start = true)]
    public class MicroServiceLookup : Disposable, IMicroServiceLookup, IStartable
    {
        internal const string LookupAddressPrefix = "topic://medseek-util/medseek-lookup.1.";
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Dictionary<string, LookupEntry> lookupCache = new Dictionary<string, LookupEntry>();
        private readonly List<LookupOperation> lookupOperations = new List<LookupOperation>();
        private readonly object sync = new object();
        private readonly IMqChannel channel;
        private readonly IMessageContext messageContext;
        private readonly IMicroServiceSerializer serializer;
        private bool started;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceLookup"/> class.
        /// </summary>
        public MicroServiceLookup(IMqChannel channel, IMessageContext messageContext, IMicroServiceSerializer serializer)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");
            if (messageContext == null)
                throw new ArgumentNullException("messageContext");
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            this.channel = channel;
            this.messageContext = messageContext;
            this.serializer = serializer;
        }

        /// <summary>
        /// Starts the micro-service lookup by making appropriate messaging 
        /// system subscriptions.
        /// </summary>
        public void Start()
        {
            ThrowIfDisposed();
            if (started)
                throw new InvalidOperationException("Already started.");

            Log.InfoFormat("Starting micro-service lookup component.");
            started = true;

            channel.Returned += OnChannelReturned;
        }

        /// <summary>
        /// Resolves a micro-service using the lookup functionality and the 
        /// specified original address.
        /// </summary>
        /// <param name="address">
        /// The original address corresponding to the event, request, service, 
        /// or other relevant messaging primitive. 
        /// </param>
        /// <param name="timeout">
        /// The maximum amount of time to wait for a query to complete before 
        /// it will be aborted.
        /// </param>
        /// <returns>
        /// The resolved address, or null if no reply was returned by the 
        /// query.
        /// </returns>
        public MqAddress Resolve(MqAddress address, TimeSpan timeout)
        {
            ThrowIfDisposed();

            var op = new LookupOperation(address.Value);
            lock (sync)
            {
                LookupEntry cachedResult;
                if (lookupCache.TryGetValue(op.Id, out cachedResult))
                    return cachedResult.Address;

                lookupOperations.Add(op);
            }
            try
            {
                var query = new LookupQuery { Id = op.Id };
                var publishAddress = new MqAddress(LookupAddressPrefix + "query." + op.Id + "/");
                using (var publisher = channel.CreatePublisher(publishAddress))
                {
                    var contentType = messageContext.Properties.ContentType;
                    var body = serializer.Serialize(contentType, query);
                    publisher.Mandatory = true;
                    publisher.Publish(body, new MessageProperties
                    {
                        ContentType = contentType,
                        CorrelationId = op.CorrelationId,
                        ReplyToString = LookupAddressPrefix + "reply." + op.Id,
                    });
                }

                op.Wait(timeout);
                if (op.Exception != null)
                    throw op.Exception;

                var result = op.Result;
                return result.Address;
            }
            finally
            {
                lock (sync)
                    lookupOperations.Remove(op);
                op.Dispose();
            }
        }

        /// <summary>
        /// Handles micro-service lookup information updates.
        /// </summary>
        [MicroServiceBinding(LookupAddressPrefix + "reply.#", IsOneWay = true)]
        public void Reply(LookupRecord record)
        {
            OnReceived(record, LookupResultType.Reply);
        }

        /// <summary>
        /// Handles micro-service lookup information updates.
        /// </summary>
        [MicroServiceBinding(LookupAddressPrefix + "update.#", IsOneWay = true)]
        public void Update(LookupRecord record)
        {
            OnReceived(record, LookupResultType.Update);
        }

        /// <summary>
        /// Disposes the micro-service lookup component.
        /// </summary>
        protected override void OnDisposing()
        {
            channel.Returned -= OnChannelReturned;
            lock (sync)
            {
                foreach (var lookupOperation in lookupOperations)
                {
                    Log.WarnFormat("Disposing lookup, aborting query; Id = {0}, CorrelationId = {1}.", lookupOperation.Id, lookupOperation.CorrelationId);
                    lookupOperation.Complete(new OperationCanceledException("The micro-service lookup component is being disposed."));
                }
            }
        }

        private void OnChannelReturned(object sender, ReturnedEventArgs e)
        {
            lock (sync)
            {
                Log.WarnFormat("Undeliverable lookup query returned; Address = {0}, CorrelationId = {1}, ReplyCode = {2}, ReplyText = {3}.", e.Address, e.MessageContext.Properties.CorrelationId, e.ReplyCode, e.ReplyText);
                foreach (var lookupOperation in lookupOperations.Where(x => x.CorrelationId == e.MessageContext.Properties.CorrelationId))
                    lookupOperation.Complete(new LookupEntry(null, lookupOperation.Id, LookupResultType.Returned));
            }
        }

        private void OnReceived(LookupRecord record, LookupResultType lookupResultType)
        {
            var correlationId = messageContext.Properties.CorrelationId;
            var entry = new LookupEntry(new MqAddress(record.Address), record.Id, lookupResultType);
            lock (sync)
            {
                Log.InfoFormat("Updating lookup cache; Id = {0}, Address = {1}, Type = {2}.", entry.Id, entry.Address, entry.Type);
                lookupCache[entry.Id] = entry;
                foreach (var lo in lookupOperations.Where(x => x.CorrelationId == correlationId))
                {
                    Log.WarnFormat("Completing lookup operation; Id = {0}, Address = {1}, CorrelationId = {2}", record.Id, record.Address, correlationId);
                    lo.Complete(entry);
                }
            }
        }

        private class LookupOperation : Disposable
        {
            private readonly string correlationId = "MicroServiceLookup." + Guid.NewGuid().ToString("N");
            private readonly Lazy<ManualResetEventSlim> sync = new Lazy<ManualResetEventSlim>(() => new ManualResetEventSlim());
            private readonly string id;

            internal LookupOperation(string id)
            {
                if (id == null)
                    throw new ArgumentNullException("id");

                this.id = id;
                Disposing += (sender, e) =>
                {
                    if (sync.IsValueCreated)
                        sync.Value.Dispose();
                };
            }

            internal string Id
            {
                get
                {
                    return id;
                }
            }

            internal string CorrelationId
            {
                get
                {
                    return correlationId;
                }
            }

            internal Exception Exception
            {
                get;
                private set;
            }

            internal LookupEntry Result
            {
                get;
                private set;
            }

            internal void Complete(LookupEntry result)
            {
                Result = result;
                sync.Value.Set();
            }

            internal void Complete(Exception exception)
            {
                Exception = exception;
                sync.Value.Set();
            }

            internal void Wait(TimeSpan timeout)
            {
                var success = sync.Value.Wait(timeout);
                if (!success)
                    Exception = new TimeoutException("Wait time expired while waiting for lookup to complete (timeout = " + timeout + ")");
            }
        }
    }
}