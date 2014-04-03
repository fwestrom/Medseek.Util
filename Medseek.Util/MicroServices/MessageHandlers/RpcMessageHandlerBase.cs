namespace Medseek.Util.MicroServices.MessageHandlers
{
	/// <summary>
	/// Generic base class for two-way message handlers.
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	/// <typeparam name="TReply"></typeparam>
	public abstract class RpcMessageHandlerBase<TMessage, TReply> : IRpcMessageHandler
		where TMessage : MessageBase
		where TReply : ReplyBase
	{
		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public ReplyBase Handle(MessageBase message)
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
