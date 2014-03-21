namespace Medseek.Util.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    using Medseek.Util.Ioc;

    /// <summary>
    /// DataContractSerializer implementation of ISerializer.
    /// </summary>
    [Register(typeof(ISerializer))]
    public class SystemRuntimeSerializationDataContractSerializer : ISerializer
    {
        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <value>
        /// The content types.
        /// </value>
        public string[] ContentTypes 
        {
            get { return new[] { "application/xml" }; }
        }

        /// <summary>
        /// Determines whether this instance can deserialize the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if it can serialize the stream to the type of the <see cref="type"/></returns>
        public bool CanDeserialize(Type type, Stream target)
        {
            try
            {
                Deserialize(type, target);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Serializes the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="target">The target.</param>
        /// <returns>
        ///   <see cref="byte" /><see cref="Array" />
        /// </returns>
        public byte[] Serialize(Type type, object target)
        {
            if (type == typeof(void))
                return new byte[0];

            var serializer = new DataContractSerializer(type);

            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, target);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes the specified data.
        /// </summary>
        /// <typeparam name="T"><see cref="Type" /> to deserialize.</typeparam>
        /// <param name="data">The data.</param>
        /// <returns>
        ///   <see cref="T" />
        /// </returns>
        /// <exception cref="System.NotImplementedException">This method is not implemented.</exception>
        public T Deserialize<T>(byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="data">The data.</param>
        /// <returns>
        ///   <see cref="type" />
        /// </returns>
        public object Deserialize(Type type, Stream data)
        {
            var serializer = new DataContractSerializer(type);
            var result = serializer.ReadObject(data);
            return result;
        }
    }
}
