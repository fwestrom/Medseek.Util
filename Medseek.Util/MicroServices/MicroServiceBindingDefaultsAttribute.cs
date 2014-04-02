namespace Medseek.Util.MicroServices
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Specifies binding defaults.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
	public class MicroServiceBindingDefaultsAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MicroServiceBindingDefaultsAttribute"/> class. 
		/// </summary>
		/// <param name="exchangeName"></param>
		/// <param name="exchangeType"></param>
		/// <param name="bindingKeyPrefix"></param>
		public MicroServiceBindingDefaultsAttribute(string exchangeName, string exchangeType, string bindingKeyPrefix)
		{
			ExchangeName = exchangeName;
			ExchangeType = exchangeType;
			BindingKeyPrefix = bindingKeyPrefix;
		}

		/// <summary>
		/// Gets or sets the Exchange name.
		/// </summary>
		public string ExchangeName { get; set; }

		/// <summary>
		/// Gets or sets the Exchange type (topic, fanout, direct).
		/// </summary>
		public string ExchangeType { get; set; }

		/// <summary>
		/// Gets or sets a string to be prepended to binding keys.
		/// </summary>
		public string BindingKeyPrefix { get; set; }

		/// <summary>
		/// Finds the nearest attribution to the method.
		/// </summary>
		/// <param name="methodInfo">the method info.</param>
		/// <returns>A <see cref="MicroServiceBindingDefaultsAttribute"/> or null.</returns>
		public static MicroServiceBindingDefaultsAttribute FindNearestAttribution(MethodInfo methodInfo)
		{
			var attribute = methodInfo.GetCustomAttribute<MicroServiceBindingDefaultsAttribute>();
			if(attribute == null)
				attribute = methodInfo.DeclaringType.GetCustomAttribute<MicroServiceBindingDefaultsAttribute>();
			if (attribute == null)
				attribute = methodInfo.DeclaringType.Assembly.GetCustomAttribute<MicroServiceBindingDefaultsAttribute>();
			return attribute;
		}
	}
}
