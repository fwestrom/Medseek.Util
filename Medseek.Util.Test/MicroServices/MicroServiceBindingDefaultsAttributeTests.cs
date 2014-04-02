namespace Medseek.Util.MicroServices
{
	using System.Reflection;
	using NUnit.Framework;

	/// <summary>
	/// Tests the <see cref="MicroServiceBindingDefaultsAttribute"/> class.
	/// </summary>
	[TestFixture]
	[MicroServiceBindingDefaults("class-exchange", "class-exchangeType", "class-prefix")]
	public class MicroServiceBindingDefaultsAttributeTests
	{
		/// <summary>
		/// Tests that it finds the attribute on a method, even if defined on a class and assembly.
		/// </summary>
		[Test]
		[MicroServiceBindingDefaults("method-exchange", "method-exchangeType", "method-prefix")]
		public void FindsAttributeOnMethod()
		{
			var methodInfo = MethodInfo.GetCurrentMethod() as MethodInfo;
			var attribute = MicroServiceBindingDefaultsAttribute.FindNearestAttribution(methodInfo);
			Assert.IsNotNull(attribute);
			Assert.AreEqual("method-exchange", attribute.ExchangeName);
			Assert.AreEqual("method-exchangeType", attribute.ExchangeType);
			Assert.AreEqual("method-prefix", attribute.BindingKeyPrefix);
		}

		/// <summary>
		/// Tests that it finds the attribute on the class, even if defined on the assembly.
		/// </summary>
		[Test]
		public void FindsAttributeOnClass()
		{
			var methodInfo = MethodInfo.GetCurrentMethod() as MethodInfo;
			var attribute = MicroServiceBindingDefaultsAttribute.FindNearestAttribution(methodInfo);
			Assert.IsNotNull(attribute);
			Assert.AreEqual("class-exchange", attribute.ExchangeName);
			Assert.AreEqual("class-exchangeType", attribute.ExchangeType);
			Assert.AreEqual("class-prefix", attribute.BindingKeyPrefix);
		}
	}

	[TestFixture]
	public class MicroServiceBindingDefaultsAttributeTestsForAssembly
	{
		/// <summary>
		/// Tests that it finds the attribute on the assembly.
		/// </summary>
		[Test]
		public void FindsAttributeOnAssembly()
		{
			var methodInfo = MethodInfo.GetCurrentMethod() as MethodInfo;
			var attribute = MicroServiceBindingDefaultsAttribute.FindNearestAttribution(methodInfo);
			Assert.IsNotNull(attribute);
			Assert.AreEqual("assembly-exchange", attribute.ExchangeName);
			Assert.AreEqual("assembly-exchangeType", attribute.ExchangeType);
			Assert.AreEqual("assembly-prefix", attribute.BindingKeyPrefix);
		}
	}
}
