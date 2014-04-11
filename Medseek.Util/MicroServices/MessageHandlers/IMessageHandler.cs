namespace Medseek.Util.MicroServices.MessageHandlers
{
	/// <summary>
	/// Handles messages and does not reply (one-way).
	/// </summary>
	public interface IMessageHandler
	{
		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message"></param>
		void Handle(MessageBase message);
	}
}
