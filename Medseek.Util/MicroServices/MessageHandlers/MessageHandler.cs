namespace Medseek.Util.MicroServices.MessageHandlers
{
	/// <summary>
	/// Generic base class for one-way message handlers.
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	public abstract class MessageHandler<TMessage> : IMessageHandler
		where TMessage : Message
	{
		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message"></param>
		public void Handle(Message message)
		{
			Handle(message as TMessage);
		}

		/// <summary>
		/// Handles the typed message.
		/// </summary>
		/// <param name="message"></param>
		public abstract void Handle(TMessage message);
	}
}
