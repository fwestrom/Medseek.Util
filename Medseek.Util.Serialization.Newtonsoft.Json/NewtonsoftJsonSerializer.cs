namespace Medseek.Util.Serialization.Newtonsoft.Json
{
    using System;
    using System.IO;
    using System.Linq;
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
        /// <param name="contentType">Type of the content.</param>
        /// <returns>
        ///   <c>true</c> if the serializer can deserialize the target.
        /// </returns>
        public bool CanDeserialize(Type type, Stream target, string contentType)
        {
            return ContentTypes.Contains(contentType);
        }

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
        public bool CanSerialize(Type type)
        {
            return true;
        }

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
        public object Deserialize(Type type, Stream source)
        {
            var sr = new StreamReader(source);
            var json = sr.ReadToEnd();
            return JsonConvert.DeserializeObject(json, type);
        }

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
        public void Serialize(Type type, object obj, Stream destination)
        {
            var json = JsonConvert.SerializeObject(obj);
            var data = Encoding.UTF8.GetBytes(json);
            destination.Write(data, 0, data.Length);
        }
    }
}