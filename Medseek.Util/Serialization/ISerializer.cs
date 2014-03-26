namespace Medseek.Util.Serialization
{
    using System;
    using System.IO;

    /// <summary>
    /// Interface for the object serializer utilities.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Gets the content types supported by the serializer.
        /// </summary>
        string[] ContentTypes
        {
            get;
        }

        /// <summary>
        /// Determines whether this instance can deserialize the specified type.
        /// </summary>
        /// <param name="type">The type of object to deserialize.</param>
        /// <param name="source">The source stream from which the object data can be read.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>
        /// A value indicating whether objects of the specified type can be
        /// deserialized.
        /// </returns>
        bool CanDeserialize(Type type, Stream source, string contentType);

        /// <summary>
        /// Determines whether this instance can serialize objects of the 
        /// specified type.
        /// </summary>
        /// <param name="type">
        /// The type of object to serialize.
        /// </param>
        /// <returns>
        /// A value indicating whether objects of the specified type can be 
        /// serialized.
        /// </returns>
        bool CanSerialize(Type type);

        /// <summary>
        /// Deserializes an object of the specified type from a stream.
        /// </summary>
        /// <param name="type">
        /// The type of object to deserialize.
        /// </param>
        /// <param name="source">
        /// The source stream from which the object data can be read.
        /// </param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        object Deserialize(Type type, Stream source);

        /// <summary>
        /// Serializes an object to a stream.
        /// </summary>
        /// <param name="type">
        /// The type of object to serialize.
        /// </param>
        /// <param name="obj">
        /// The object to serialize.
        /// </param>
        /// <param name="destination">
        /// The stream onto which the serialized object should be written.
        /// </param>
        void Serialize(Type type, object obj, Stream destination);
    }
}
