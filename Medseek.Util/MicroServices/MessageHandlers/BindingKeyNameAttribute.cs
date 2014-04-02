namespace Medseek.Util.MicroServices.MessageHandlers
{
	using System;

	/// <summary>
	/// Allows a message class to specify a binding key name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class BindingKeyNameAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the Routing Key Name.
		/// </summary>
		public string BindingKeyName { get; set; }

		/// <summary>
		///  Initializes a new instance of the <see cref="BindingKeyNameAttribute" /> class.
		/// </summary>
		/// <param name="bindingKeyName">The custom binding key name.</param>
		public BindingKeyNameAttribute(string bindingKeyName)
		{
			BindingKeyName = bindingKeyName;
		}
	}
}
