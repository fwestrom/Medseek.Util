namespace Medseek.Util.Messaging
{
    /// <summary>
    /// Interface for types that provide the functionality of an RPC client 
    /// using a messaging system channel for communication.
    /// </summary>
    /// <typeparam name="TRequestBody">
    /// The type used to specify the body of the request message.
    /// </typeparam>
    /// <typeparam name="TReplyBody">
    /// The type used to return the body of the reply message.
    /// </typeparam>
    public interface IMqRpcClient<in TRequestBody, out TReplyBody> : IMqDisposable
    {
        /// <summary>
        /// Sends a one-way message to the RPC service.
        /// </summary>
        /// <param name="body">
        /// An array containing the body of the outgoing message.
        /// </param>
        void Cast(TRequestBody body);

        /// <summary>
        /// Makes a blocking call to the RPC service.
        /// </summary>
        /// <param name="body">
        /// An array containing the body of the request message.
        /// </param>
        /// <returns>
        /// A stream that can be used to read the body of the response message.
        /// </returns>
        TReplyBody Call(TRequestBody body);
    }
}