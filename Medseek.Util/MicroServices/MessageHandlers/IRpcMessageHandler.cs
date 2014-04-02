namespace Medseek.Util.MicroServices.MessageHandlers
{
	/// <summary>
	/// Handles messages and responds with a reply (two-way).
	/// </summary>
	public interface IRpcMessageHandler
	{
		/// <summary>
		/// Handles the message.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		Reply Handle(Message message);
	}
}
