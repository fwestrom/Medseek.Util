namespace Medseek.Util.MicroServices
{
    using System;
    using System.IO;
    using System.Linq;
    using Castle.Core.Internal;
    using Medseek.Util.Messaging;
    using Medseek.Util.Serialization;
    using Medseek.Util.Testing;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="MicroServiceSerializer"/> class.
    /// </summary>
    [TestFixture]
    public sealed class MicroServiceSerializerTests : TestFixture<MicroServiceSerializer>
    {
        private const string ContentType0 = "application/x-contentType0";
        private const string ContentType1 = "application/x-contentType1";
        private const string ContentTypeX = "application/x-contentTypeX";
        private string contentType;
        private Mock<IMessageContext> messageContext;
        private Mock<IMessageProperties> messageProperties;
        private Mock<ISerializer> serializer0;
        private Mock<ISerializer> serializer1;
        private Mock<ISerializer>[] serializers;
        private object serializerState;
        private Mock<Stream> stream;

        /// <summary>
        /// Sets up before each test is executed.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            contentType = ContentTypeX;
            messageContext = new Mock<IMessageContext>();
            messageProperties = new Mock<IMessageProperties>();
            serializer0 = new Mock<ISerializer>();
            serializer1 = new Mock<ISerializer>();
            serializers = new[] { serializer0, serializer1 };
            serializerState = null;
            stream = new Mock<Stream>();

            messageContext.Setup(x => 
                x.Properties)
                .Returns(messageProperties.Object);
            messageProperties.Setup(x =>
                x.ContentType)
                .Returns(() => contentType);

            serializer0.Setup(x => 
                x.CanDeserialize(It.IsAny<Type>(), It.IsAny<Stream>(), ContentType0))
                .Returns(true);
            serializer0.Setup(x =>
                x.CanSerialize(It.IsAny<Type>(), ContentType0))
                .Returns(true);
            serializer1.Setup(x =>
                x.CanDeserialize(It.IsAny<Type>(), It.IsAny<Stream>(), ContentType1))
                .Returns(true);
            serializer1.Setup(x =>
                x.CanSerialize(It.IsAny<Type>(), ContentType1))
                .Returns(true);

            var serializerFactory = Mock<ISerializerFactory>();
            serializerFactory.Setup(x => 
                x.GetAllSerializers())
                .Returns(() => serializers.Select(x => x.Object).ToArray());

            // ReSharper disable once UnusedVariable
            var obj = Obj;
        }

        /// <summary>
        /// Verifies that the source stream is returned for a single stream 
        /// type.
        /// </summary>
        [Test]
        public void DeserializeReturnsSourceIfTypeIsStream()
        {
            var types = new[] { typeof(Stream) };
            var results = Obj.Deserialize(messageContext.Object, types, stream.Object, ref serializerState);

            Assert.That(results.Single(), Is.SameAs(stream.Object));
        }

        /// <summary>
        /// Verifies that the deserialized object is returned from the 
        /// serializer.
        /// </summary>
        [TestCase(0, ContentType0)]
        [TestCase(1, ContentType1)]
        public void DeserializeReturnsResultFromSerializer(int index, string contentType)
        {
            this.contentType = contentType;
            var serializerResult = new object();
            var serializer = serializers[index];
            var type = typeof(object);
            serializer.Setup(x => 
                x.Deserialize(type, stream.Object))
                .Returns(serializerResult);

            var types = new[] { type };
            var results = Obj.Deserialize(messageContext.Object, types, stream.Object, ref serializerState);

            Assert.That(results.Single(), Is.SameAs(serializerResult));
        }

        /// <summary>
        /// Verifies that an exception is thrown if no serializer is found that
        /// supports the content-type and object type.
        /// </summary>
        [Test]
        public void DeserializeThrowsIfNoSerializer()
        {
            contentType = ContentTypeX;
            serializers.ForEach(serializer => 
                serializer.Setup(x => 
                    x.CanDeserialize(It.IsAny<Type>(), It.IsAny<Stream>(), ContentTypeX))
                    .Returns(false));

            var types = new[] { typeof(object) };
            TestDelegate action = () => Obj.Deserialize(messageContext.Object, types, stream.Object, ref serializerState);

            Assert.That(action, Throws.InstanceOf<NotSupportedException>());
        }

        /// <summary>
        /// Verifies that no exception is thrown when attempting to serialize a
        /// void type.
        /// </summary>
        [Test]
        public void SerializeDoesNotThrowIfVoidType()
        {
            var type = typeof(void);
            TestDelegate action = () => Obj.Serialize(messageContext.Object, type, null, stream.Object, ref serializerState);
            Assert.That(action, Throws.Nothing);
        }

        /// <summary>
        /// Verifies that the deserialized object is returned from the 
        /// serializer.
        /// </summary>
        [TestCase(0, ContentType0)]
        [TestCase(1, ContentType1)]
        public void SerializeInvokesSerializerToWriteObject(int index, string contentType)
        {
            this.contentType = contentType;
            var serializer = serializers[index];
            var type = typeof(object);
            var value = new object();
            serializer.Setup(x =>
                x.Serialize(type, value, stream.Object))
                .Verifiable();

            Obj.Serialize(messageContext.Object, type, value, stream.Object, ref serializerState);

            serializer.Verify();
        }

        /// <summary>
        /// Verifies that an exception is thrown if no serializer is found that
        /// supports the content-type and object type.
        /// </summary>
        [Test]
        public void SerializeThrowsIfNoSerializer()
        {
            serializers.ForEach(serializer =>
                serializer.Setup(x =>
                    x.CanDeserialize(It.IsAny<Type>(), It.IsAny<Stream>(), It.IsAny<string>()))
                    .Returns(false));

            var type = typeof(object);
            var value = new object();
            TestDelegate action = () => Obj.Serialize(messageContext.Object, type, value, stream.Object, ref serializerState);

            Assert.That(action, Throws.InstanceOf<NotSupportedException>());
        }
    }
}