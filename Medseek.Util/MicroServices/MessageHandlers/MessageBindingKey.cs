namespace Medseek.Util.MicroServices.MessageHandlers
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Generates a binding key for a message type.
	/// </summary>
	public class MessageBindingKey
	{
		private readonly string value;

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBindingKey"/> class.
		/// </summary>
		/// <param name="messageType"></param>
		/// <param name="bindingKeyPrefix"></param>
		public MessageBindingKey(Type messageType, string bindingKeyPrefix)
		{
			value = MakeBindingKey(messageType, bindingKeyPrefix);
		}

		private static string MakeBindingKey(Type messageType, string bindingKeyPrefix)
		{
			var bindingKeyName = messageType.Name;
			var bindingKeyNameAttribute = messageType.GetCustomAttribute<BindingKeyNameAttribute>();
			if (bindingKeyNameAttribute != null)
				bindingKeyName = bindingKeyNameAttribute.BindingKeyName;
			var bindingKey = bindingKeyPrefix + (bindingKeyPrefix.EndsWith(".") ? "" : ".") +  bindingKeyName;
			return bindingKey;
		}

		/// <summary>
		/// Returns the binding key string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return value;
		}
	}
}
