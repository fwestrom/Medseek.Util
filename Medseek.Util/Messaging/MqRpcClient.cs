namespace Medseek.Util.Messaging
{
    using System;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// Provides the functionality for implementing an RPC client using 
    /// asynchronous messaging systems.
    /// </summary>
    public class MqRpcClient : MqSynchronizedDisposable, IMqRpcClient<byte[], Stream>
    {
        private readonly MqAddress replyTo = new MqAddress("RpcClient-" + Guid.NewGuid().ToString("N"));
        private readonly IMqConsumer consumer;
        private readonly IMqPublisher publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqRpcClient"/> class.
        /// </summary>
        public MqRpcClient(IMqChannel channel, MqAddress address)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");

            consumer = channel.CreateConsumer(replyTo, true);
            publisher = channel.CreatePublisher(address);
        }

        /// <summary>
        /// Sends a one-way message to the RPC service.
        /// </summary>
        /// <param name="body">
        /// An array containing the body of the outgoing message.
        /// </param>
        public void Cast(byte[] body)
        {
            using (EnterDisposeLock())
                publisher.Publish(body);
        }

        /// <summary>
        /// Makes a blocking call to the RPC service.
        /// </summary>
        /// <param name="body">
        /// An array containing the body of the request message.
        /// </param>
        /// <returns>
        /// A stream that can be used to read the body of the response message.
        /// </returns>
        public Stream Call(byte[] body)
        {
            using (var helper = new Helper(consumer))
            {
                using (EnterDisposeLock())
                {
                    helper.Initialize();
                    OnDisposableCreated(helper);

                    publisher.Publish(body, helper.CorrelationId, replyTo);
                }

                var response = helper.Wait();
                if (response == null)
                    throw new OperationCanceledException();

                return response.Body;
            }
        }

        /// <summary>
        /// Disposes the RPC client.
        /// </summary>
        protected override void OnDisposingMqDisposable()
        {
            consumer.Dispose();
            publisher.Dispose();
        }

        private class Helper : MqSynchronizedDisposable
        {
            private readonly string correlationId = Guid.NewGuid().ToString("N");
            private readonly object sync = new object();
            private readonly IMqConsumer consumer;
            private Action<ReceivedEventArgs> completeAction;
            private ReceivedEventArgs result;
            private bool done;

            internal Helper(IMqConsumer consumer)
            {
                this.consumer = consumer;
            }

            internal string CorrelationId
            {
                get
                {
                    return correlationId;
                }
            }

            internal void Initialize()
            {
                Interlocked.Exchange(ref completeAction, Complete);
                consumer.Received += OnReceived;
                done = false;
            }

            internal ReceivedEventArgs Wait()
            {
                lock (sync)
                {
                    if (!done)
                        Monitor.Wait(sync);
                    return result;
                }
            }

            protected override void OnDisposingMqDisposable()
            {
                var complete = Interlocked.Exchange(ref completeAction, null);
                if (complete != null)
                    complete(null);
            }

            private void OnReceived(object sender, ReceivedEventArgs e)
            {
                if (e.Properties.CorrelationId == CorrelationId)
                {
                    var complete = Interlocked.Exchange(ref completeAction, null);
                    if (complete != null)
                        complete(e);
                }
            }

            private void Complete(ReceivedEventArgs e)
            {
                consumer.Received -= OnReceived;
                lock (sync)
                {
                    done = true;
                    result = e;
                    Monitor.Pulse(sync);
                }
            }
        }
    }
}