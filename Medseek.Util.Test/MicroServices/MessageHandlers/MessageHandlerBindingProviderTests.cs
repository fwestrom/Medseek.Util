namespace Medseek.Util.MicroServices.MessageHandlers
{
	using System;
	using System.Linq;
	using NUnit.Framework;

	/// <summary>
	/// Tests the <see cref="MessageHandlerBindingProvider"/> class.
	/// </summary>
	[TestFixture]
	public class MessageHandlerBindingProviderTests
	{
		const string expectedBindingMethodName = "Handle";
		private MessageHandlerBindingProvider provider;
		
		[SetUp]
		public void SetUp()
		{
			provider = new MessageHandlerBindingProvider();
		}

		/// <summary>
		/// Tests that the <see cref="MessageHandlerBindingProvider"/>
		/// creates a binding for a standard one-way message handler.
		/// </summary>
		[Test]
		public void GetBindingsIdentifiesMessageHandler()
		{
			var bindings = provider.GetBindings<MicroServiceBinding>(typeof(TestMessageHandler));
			Assert.NotNull(bindings);
			Assert.AreEqual(1, bindings.Count());
			var binding = bindings.ElementAt(0);
			Assert.IsTrue(binding.IsOneWay);
			Assert.AreEqual(expectedBindingMethodName, binding.Method.Name);
			Assert.AreEqual(typeof(TestMessage), binding.Method.GetParameters().ElementAt(0).ParameterType);
			Assert.AreEqual(typeof(TestMessageHandler), binding.Service);
			Assert.AreEqual(binding.Address.Value, "topic://assembly-exchange/assembly-prefix.TestMessage/Medseek.Util.MicroServices.MessageHandlers.TestMessageHandler");
		}

		/// <summary>
		/// Tests that the <see cref="MessageHandlerBindingProvider"/>
		/// creates a binding for a standard rpc message handler.
		/// </summary>
		[Test]
		public void GetBindingsIdentifiesRpcMessageHandler()
		{
			var bindings = provider.GetBindings<MicroServiceBinding>(typeof(TestRpcMessageHandler));
			Assert.NotNull(bindings);
			Assert.AreEqual(1, bindings.Count());
			var binding = bindings.ElementAt(0);
			Assert.IsFalse(binding.IsOneWay);
			Assert.AreEqual(expectedBindingMethodName, binding.Method.Name);
			Assert.AreEqual(typeof(TestMessage), binding.Method.GetParameters().ElementAt(0).ParameterType);
			Assert.AreEqual(typeof(TestRpcMessageHandler), binding.Service);
			Assert.AreEqual(binding.Address.Value, "topic://assembly-exchange/assembly-prefix.TestMessage/Medseek.Util.MicroServices.MessageHandlers.TestRpcMessageHandler");
		}

		/// <summary>
		/// Tests that the <see cref="MessageHandlerBindingProvider"/>
		/// creates a binding for a message handler subclassed from another message handler.
		/// </summary>
		[Test]
		public void GetBindingsIdentifiesSubclassedRpcMessageHandler()
		{
			var bindings = provider.GetBindings<MicroServiceBinding>(typeof(SubClassedTestRpcMessageHandler));
			Assert.NotNull(bindings);
			Assert.AreEqual(1, bindings.Count());
			var binding = bindings.ElementAt(0);
			Assert.IsFalse(binding.IsOneWay);
			Assert.AreEqual(expectedBindingMethodName, binding.Method.Name);
			Assert.AreEqual(typeof(TestMessage), binding.Method.GetParameters().ElementAt(0).ParameterType);
			Assert.AreEqual(typeof(SubClassedTestRpcMessageHandler), binding.Service);
			Assert.AreEqual(binding.Address.Value, "topic://assembly-exchange/assembly-prefix.TestMessage/Medseek.Util.MicroServices.MessageHandlers.SubClassedTestRpcMessageHandler");
		}

		/// <summary>
		/// Tests that the <see cref="MessageHandlerBindingProvider"/>
		/// creates a binding for a message handler subclassed from an abstract message handler.
		/// </summary>
		[Test]
		public void GetBindingsIdentifiesConcreteRpcMessageHandler()
		{
			var bindings = provider.GetBindings<MicroServiceBinding>(typeof(ConcreteTestRpcMessageHandler));
			Assert.NotNull(bindings);
			Assert.AreEqual(1, bindings.Count());
			var binding = bindings.ElementAt(0);
			Assert.IsFalse(binding.IsOneWay);
			Assert.AreEqual(expectedBindingMethodName, binding.Method.Name);
			Assert.AreEqual(typeof(TestMessage), binding.Method.GetParameters().ElementAt(0).ParameterType);
			Assert.AreEqual(typeof(ConcreteTestRpcMessageHandler), binding.Service);
			Assert.AreEqual(binding.Address.Value, "topic://assembly-exchange/assembly-prefix.TestMessage/Medseek.Util.MicroServices.MessageHandlers.ConcreteTestRpcMessageHandler");
		}

		/// <summary>
		/// Tests that the <see cref="MessageHandlerBindingProvider"/>
		/// does not create a binding for an abstract message handler.
		/// </summary>
		[Test]
		public void GetBindingsIgnoresAbstractMessageHandler()
		{
			var bindings = provider.GetBindings<MicroServiceBinding>(typeof(AbstractTestRpcMessageHandler));
			Assert.NotNull(bindings);
			Assert.AreEqual(0, bindings.Count());
		}

		/// <summary>
		/// Tests that the <see cref="MessageHandlerBindingProvider"/>
		/// does not create a binding for a type that is not a message handler.
		/// </summary>
		[Test]
		public void GetBindingsIgnoresNonMessageHandlerType()
		{
			var bindings = provider.GetBindings<MicroServiceBinding>(typeof(string));
			Assert.NotNull(bindings);
			Assert.AreEqual(0, bindings.Count());
		}
	}

#region Test Types
	class TestMessage : MessageBase
	{
	}

	class TestReply : ReplyBase
	{

	}

	class TestMessageHandler : MessageHandlerBase<TestMessage>
	{
		public override void Handle(TestMessage message)
		{
			throw new NotImplementedException();
		}
	}

	class TestRpcMessageHandler : RpcMessageHandlerBase<TestMessage, TestReply>
	{
		public override TestReply Handle(TestMessage message)
		{
			throw new NotImplementedException();
		}
	}

	class SubClassedTestRpcMessageHandler : TestRpcMessageHandler
	{
		public override TestReply Handle(TestMessage message)
		{
			return base.Handle(message);
		}
	}

	abstract class AbstractTestRpcMessageHandler : RpcMessageHandlerBase<TestMessage, TestReply>
	{
	}

	class ConcreteTestRpcMessageHandler : AbstractTestRpcMessageHandler
	{
		public override TestReply Handle(TestMessage message)
		{
			throw new NotImplementedException();
		}
	}

#endregion
}
