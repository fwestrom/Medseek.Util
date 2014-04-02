namespace Medseek.Util.MicroServices.MessageHandlers
{
	using System;
	using System.Globalization;
	using Medseek.Util.Messaging;

	/// <summary>
	/// MicroServiceBinding for Message Handlers.
	/// </summary>
	public class MessageHandlerBinding : MicroServiceBinding
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MessageHandlerBinding"/> class.
		/// </summary>
		/// <param name="messageType">The Message type.</param>
		/// <param name="handlerType">The Handler type.</param>
		public MessageHandlerBinding(Type messageType, Type handlerType)
		{
			Service = handlerType;
			Method = handlerType.GetMethod("Handle", new Type[] { messageType });
			IsOneWay = typeof(IMessageHandler).IsAssignableFrom(handlerType);
			var bindingDefaults = MicroServiceBindingDefaultsAttribute.FindNearestAttribution(Method);
			if (bindingDefaults == null)
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "No {0} was found on the method, type or assembly for {1}", typeof(MicroServiceBindingDefaultsAttribute).Name, Method));
			var bindingKey = new MessageBindingKey(messageType, bindingDefaults.BindingKeyPrefix);
			var queueName = handlerType.FullName;
			Address = new MqAddress(string.Format("{0}://{1}/{2}/{3}", "topic", bindingDefaults.ExchangeName, bindingKey, queueName));
		}

		/// <summary>
		/// Gets the <see cref="MicroServiceBinding"/> represented by this instance.
		/// </summary>
		public MicroServiceBinding ConvertToMicroServiceBinding()
		{
			return new MicroServiceBinding { Address = Address, IsOneWay = IsOneWay, Method = Method, Service = Service };
		}
	}
}
