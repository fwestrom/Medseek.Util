namespace Medseek.Util.MicroServices
{
    using System;
    using System.IO;

    /// <summary>
    /// Provides extension methods for working with the micro-service serializer.
    /// </summary>
    public static class MicroServiceSerializerExtensions
    {
        /// <summary>
        /// Deserializes an object from a specified content type and source.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object to read.
        /// </typeparam>
        /// <param name="serializer">
        /// The serializer to use for reading the object.
        /// </param>
        /// <param name="contentType">
        /// The content type to deserialize.
        /// </param>
        /// <param name="source">
        /// The source of the serialized object data.
        /// </param>
        public static T Deserialize<T>(this IMicroServiceSerializer serializer, string contentType, Stream source)
        {
            var values = serializer.Deserialize(contentType, new[] { typeof(T) }, source);
            var value = (T)values[0];
            return value;
        }

        /// <summary>
        /// Serializes an object to a specified content type and destination.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object to serialize.
        /// </typeparam>
        /// <param name="serializer">
        /// The serializer to use for writing the object.
        /// </param>
        /// <param name="contentType">
        /// The content type to use for serialization.
        /// </param>
        /// <param name="destination">
        /// The destination of the serialized object data.
        /// </param>
        /// <param name="value">
        /// The value to serialize.
        /// </param>
        public static void Serialize<T>(this IMicroServiceSerializer serializer, string contentType, Stream destination, T value)
        {
            serializer.Serialize(contentType, typeof(T), value, destination);
        }

        /// <summary>
        /// Serializes an object to an array of bytes formatted to the 
        /// specified content type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object to serialize.
        /// </typeparam>
        /// <param name="serializer">
        /// The serializer to use for writing the object.
        /// </param>
        /// <param name="contentType">
        /// The content type to use for serialization.
        /// </param>
        /// <param name="value">
        /// The value to serialize.
        /// </param>
        /// <returns>
        /// An array of bytes representing the serialized value.
        /// </returns>
        public static byte[] Serialize<T>(this IMicroServiceSerializer serializer, string contentType, T value)
        {
            return serializer.Serialize(contentType, typeof(T), value);
        }

        /// <summary>
        /// Serializes an object to an array of bytes formatted to the 
        /// specified content type.
        /// </summary>
        /// <param name="serializer">
        /// The serializer to use for writing the object.
        /// </param>
        /// <param name="contentType">
        /// The content type to use for serialization.
        /// </param>
        /// <param name="type">
        /// The type to specify when serializing the object, or null to get the
        /// type of the value from the specified object.
        /// </param>
        /// <param name="value">
        /// The value to serialize.
        /// </param>
        /// <returns>
        /// An array of bytes representing the serialized value.
        /// </returns>
        public static byte[] Serialize(this IMicroServiceSerializer serializer, string contentType, Type type, object value)
        {
            using (var ms = new MemoryStream())
            {
                var serializeType = type ?? value.GetType();
                serializer.Serialize(contentType, serializeType, value, ms);
                return ms.ToArray();
            }
        }
    }
}