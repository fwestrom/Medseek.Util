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
        /// Gets the content types.
        /// </summary>
        /// <value>
        /// The content types.
        /// </value>
        string[] ContentTypes { get; }

        /// <summary>
        /// Determines whether this instance can deserialize the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="targert">The target.</param>
        /// <returns><c>true</c> if the serializer can deserialize the target.</returns>
        bool CanDeserialize(Type type, Stream targert);

        /// <summary>
        /// Serializes the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="target">The target.</param>
        /// <returns><see cref="byte"/><see cref="Array"/></returns>
        byte[] Serialize(Type type, object target);

        /// <summary>
        /// Deserializes the specified data.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> to deserialize.</typeparam>
        /// <param name="data">The data.</param>
        /// <returns><see cref="T"/></returns>
        T Deserialize<T>(byte[] data);

        /// <summary>
        /// Deserializes the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="data">The data.</param>
        /// <returns><see cref="type"/></returns>
        object Deserialize(Type type, Stream data);
    }
}
