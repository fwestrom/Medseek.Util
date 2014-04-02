namespace Medseek.Util.MicroServices.MessageHandlers
{
	using System;
	using System.Collections.Generic;
	using Medseek.Util.Ioc;
	using Medseek.Util.MicroServices;
	using Medseek.Util.MicroServices.BindingProviders;

	/// <summary>
	/// Provides <see cref="MicroServiceBinding"/>s based on <see cref="IMessageHandler"/> and <see cref="IRpcMessageHandler"/> implementations.
	/// </summary>
	[Register(typeof(IMicroServiceBindingProvider), OnlyNewServices=false)]
	public class MessageHandlerBindingProvider : IMicroServiceBindingProvider
	{
		/// <summary>
		/// Identifies the micro-service bindings associated with the specified
		/// type, which may or may not be expected to be bound.
		/// </summary>
		/// <param name="type">
		/// The type to analyze for micro-service bindings.
		/// </param>
		/// <returns>
		/// The micro-service bindings that were found.
		/// </returns>
		public IEnumerable<T> GetBindings<T>(Type type) where T : MicroServiceBinding, new()
		{
			if (IsMessageHandlerType(type))
			{
				var genericBaseType = GetGenericBaseType(type);
				if (genericBaseType.GenericTypeArguments.Length > 0)
				{
					var messageType = genericBaseType.GenericTypeArguments[0];
					var binding = new MessageHandlerBinding(messageType, type);
					yield return new T
					{
						Address = binding.Address,
						Method = binding.Method,
						Service = binding.Service,
						IsOneWay = binding.IsOneWay
					};
				}
			}
		}

		private static bool IsMessageHandlerType(Type type)
		{
			return !type.IsAbstract && (IsOneWayHandlerType(type) || IsRpcHandlerType(type));
		}

		private static bool IsOneWayHandlerType(Type type)
		{
			return typeof(IMessageHandler).IsAssignableFrom(type);
		}

		private static bool IsRpcHandlerType(Type type)
		{
			return typeof(IRpcMessageHandler).IsAssignableFrom(type);
		}

		private static Type GetGenericBaseType(Type type)
		{
			while (type.GenericTypeArguments.Length == 0 && type != typeof(object))
				type = type.BaseType;
			return type;
		}
	}
}
