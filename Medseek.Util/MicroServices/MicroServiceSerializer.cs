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
        public object[] Deserialize(string contentType, Type[] types, Stream source)
        {
            object serializerState = null;
            return Read(contentType, types, source, ref serializerState);
        }

        /// <summary>
        /// Serializes the result from invoking a micro-service operation.
        /// </summary>
        public void Serialize(string contentType, Type type, object value, Stream destination)
        {
            object serializerState = null;
            Write(contentType, type, value, destination, ref serializerState);
        }

        /// <summary>
        /// Deserializes the parameters for invoking a micro-service operation.
        /// </summary>
        object[] IMicroServiceSerializer.Deserialize(IMessageContext messageContext, Type[] types, Stream source, ref object serializerState)
        {
            var contentType = messageContext.Properties.ContentType;
            return Read(contentType, types, source, ref serializerState);
        }

        /// <summary>
        /// Serializes the result from invoking a micro-service operation.
        /// </summary>
        void IMicroServiceSerializer.Serialize(string contentType, Type type, object value, Stream destination, ref object serializerState)
        {
            Write(contentType, type, value, destination, ref serializerState);
        }

        /// <summary>
        /// Serializes the result from invoking a micro-service operation.
        /// </summary>
        void IMicroServiceSerializer.Serialize(IMessageContext messageContext, Type type, object value, Stream destination)
        {
            var contentType = messageContext.Properties.ContentType;
            object serializerState = null;
            Write(contentType, type, value, destination, ref serializerState);
        }

        /// <summary>
        /// Serializes the result from invoking a micro-service operation.
        /// </summary>
        void IMicroServiceSerializer.Serialize(IMessageContext messageContext, Type type, object value, Stream destination, ref object serializerState)
        {
            var contentType = messageContext.Properties.ContentType;
            Write(contentType, type, value, destination, ref serializerState);
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

        private object[] Read(string contentType, Type[] types, Stream source, ref object serializerState)
        {
            var results = new object[types.Length];

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

        private void Write(string contentType, Type type, object value, Stream destination, ref object serializerState)
        {
            if (type != typeof(void))
            {
                if (type == typeof(Stream))
                {
                    var stream = (Stream)value;
                    stream.CopyTo(destination);
                }
                else
                {
                    var serializer = GetSerializer(contentType, type, ref serializerState);
                    serializer.Serialize(type, value, destination);
                }
            }
        }
    }
}