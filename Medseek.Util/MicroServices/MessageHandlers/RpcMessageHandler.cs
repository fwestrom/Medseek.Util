namespace Medseek.Util.MicroServices.MessageHandlers
{
	/// <summary>
	/// Generic base class for two-way message handlers.
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	/// <typeparam name="TReply"></typeparam>
	public abstract class RpcMessageHandler<TMessage, TReply> : IRpcMessageHandler
		where TMessage : Message
		where TReply : Reply
	{
		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public Reply Handle(Message message)
		{
			return Handle(message as TMessage);
		}

		/// <summary>
		/// Handles the typed message.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public abstract TReply Handle(TMessage message);
	}
}
