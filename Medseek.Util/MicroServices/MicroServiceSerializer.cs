namespace Medseek.Util.MicroServices
{
    using System;
    using System.IO;
    using System.Linq;
    using Medseek.Util.Ioc;
    using Medseek.Util.Serialization;

    /// <summary>
    /// Provides serialization functionality for operations dispatch of micro-services.
    /// </summary>
    [Register(typeof(IMicroServiceSerializer))]
    public class MicroServiceSerializer : IMicroServiceSerializer
    {
        private readonly ISerializer[] serializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceSerializer"/> class.
        /// </summary>
        public MicroServiceSerializer(ISerializerFactory serializerFactory)
        {
            if (serializerFactory == null)
                throw new ArgumentNullException("serializerFactory");

            serializers = serializerFactory.GetAllSerializers();
        }

        /// <summary>
        /// Deserializes the parameters for invoking a micro-service operation.
        /// </summary>
        public object[] Deserialize(IMessageContext messageContext, Type[] types, Stream source, ref object serializerState)
        {
            var results = new object[types.Length];

            var contentType = messageContext.Properties.ContentType;
            var type = types.SingleOrDefault();
            if (type != null)
            {
                object value;
                if (type == typeof(Stream))
                {
                    value = source;
                }
                else
                {
                    var serializer = GetSerializer(contentType, type, ref serializerState);
                    value = serializer.Deserialize(type, source);
                }

                results[0] = value;
            }

            return results;
        }

        /// <summary>
        /// Serializes the result from invoking a micro-service operation.
        /// </summary>
        public void Serialize(IMessageContext messageContext, Type type, object value, Stream destination)
        {
            object serializerState = null;
            Serialize(messageContext, type, value, destination, ref serializerState);
        }

        /// <summary>
        /// Serializes the result from invoking a micro-service operation.
        /// </summary>
        public void Serialize(IMessageContext messageContext, Type type, object value, Stream destination, ref object serializerState)
        {
            var contentType = messageContext.Properties.ContentType;
            if (type != typeof(void))
            {
                var serializer = GetSerializer(contentType, type, ref serializerState);
                serializer.Serialize(type, value, destination);
            }
        }

        private ISerializer GetSerializer(string contentType, Type type, ref object serializerState)
        {
            var serializer = serializerState as ISerializer;
            if (serializer == null || !serializer.CanSerialize(type, contentType))
            {
                serializer = serializers.FirstOrDefault(x => x.CanDeserialize(type, null, contentType));
                if (serializer == null)
                    throw new NotSupportedException("Unable to deserialize from content-type " + contentType + " to objects of type " + type + ".");
            }

            return serializer;
        }
    }
}