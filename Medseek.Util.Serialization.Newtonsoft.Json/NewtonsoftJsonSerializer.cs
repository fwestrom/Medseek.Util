namespace Medseek.Util.Serialization.Newtonsoft.Json
{
    using System;
    using System.IO;
    using System.Text;

    using Medseek.Util.Ioc;

    using global::Newtonsoft.Json;

    [Register(typeof(ISerializer), OnlyNewServices = false)]
    public class NewtonsoftJsonSerializer : ISerializer
    {
        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <value>
        /// The content types.
        /// </value>
        public string[] ContentTypes 
        { 
            get
            {
                return new[] { "application/json" };
            } 
        }

        /// <summary>
        /// Determines whether this instance can deserialize the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="target">The target.</param>
        /// <returns><c>true</c> if the serializer can deserialize the target.</returns>
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
            var json = JsonConvert.SerializeObject(target);
            return Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// Deserializes the specified data.
        /// </summary>
        /// <typeparam name="T">Input type.</typeparam>
        /// <param name="data">The data.</param>
        /// <returns>The type parameter type.</returns>
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
            data.Position = 0;
            var sr = new StreamReader(data);
            var dataStr = sr.ReadToEnd();
            return JsonConvert.DeserializeObject(dataStr, type);
        }
    }
}
