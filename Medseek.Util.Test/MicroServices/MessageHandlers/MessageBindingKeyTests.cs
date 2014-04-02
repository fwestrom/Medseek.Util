namespace Medseek.Util.MicroServices.MessageHandlers
{
	using NUnit.Framework;

	/// <summary>
	/// Tests the <see cref="MessageBindingKey"/> class.
	/// </summary>
	[TestFixture]
	public class MessageBindingKeyTests
	{
		const string bindingKeyPrefix = "prefix.";

		/// <summary>
		/// Tests the default behavior of creating the binding key from the type name.
		/// </summary>
		[Test]
		public void CreatesBindingKeyFromClassName()
		{
			var key = new MessageBindingKey(typeof(TestBindingKeyMessage), bindingKeyPrefix);
			Assert.AreEqual("prefix.TestBindingKeyMessage", key.ToString());
		}

		/// <summary>
		/// Tests using a custom attribute to override the binding key name with a custom value.
		/// </summary>
		[Test]
		public void CreatesBindingKeyUsingAttribute()
		{
			var key = new MessageBindingKey(typeof(AttributedMessage), bindingKeyPrefix);
			Assert.AreEqual("prefix.CustomAction", key.ToString());
		}

		/// <summary>
		/// Tests the that it will ensure a dot after prefix, if missing
		/// </summary>
		[Test]
		public void EnsuresDotDelimitedKey()
		{
			var key = new MessageBindingKey(typeof(TestBindingKeyMessage), "prefix");
			Assert.AreEqual("prefix.TestBindingKeyMessage", key.ToString());
		}
	}

	class TestBindingKeyMessage : Message
	{
	}

	[BindingKeyName("CustomAction")]
	class AttributedMessage : Message
	{
	}
}
