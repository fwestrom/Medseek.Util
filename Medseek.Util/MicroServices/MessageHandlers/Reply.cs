namespace Medseek.Util.MicroServices.MessageHandlers
{
	/// <summary>
	/// Base class for all replies.
	/// </summary>
	public class Reply
	{
		/// <summary>
		/// Gets or sets an optional message string.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the exception message, if an exception occurred.
		/// </summary>
		public string ExceptionMessage { get; set; }

		/// <summary>
		/// Gets or sets the stack trace, if an exception occurred.
		/// </summary>
		public string StackTrace { get; set; }
	}
}
