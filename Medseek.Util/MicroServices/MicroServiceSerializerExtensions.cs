namespace Medseek.Util.MicroServices
{
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
    }
}