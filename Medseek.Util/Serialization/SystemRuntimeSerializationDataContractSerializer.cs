﻿namespace Medseek.Util.Serialization
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml;
    using Medseek.Util.Ioc;

    /// <summary>
    /// DataContractSerializer implementation of ISerializer.
    /// </summary>
    [Register(typeof(ISerializer), OnlyNewServices = false)]
    public class SystemRuntimeSerializationDataContractSerializer : ISerializer
    {
        private readonly XmlWriterSettings settings = new XmlWriterSettings { CloseOutput = false, ConformanceLevel = ConformanceLevel.Fragment, Indent = true };

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
                return new[] { "application/xml" };
            }
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
        public bool CanDeserialize(Type type, Stream source, string contentType)
        {
            return (contentType == null || ContentTypes.AsEnumerable().Contains(contentType))
                && (type.GetCustomAttribute<DataContractAttribute>() != null
                        || type.GetInterface("ICollection") != null
                        || (type.IsArray && CanDeserialize(type.GetElementType(), source, contentType))
                        || type.IsEnum
                        || type.IsPrimitive
                        || type == typeof(DateTime)
                        || type == typeof(DateTimeOffset)
                        || type == typeof(TimeSpan)
                        || type == typeof(Guid)
                        || type == typeof(Uri)
                        || type == typeof(XmlQualifiedName));
        }

        /// <summary>
        /// Determines whether this instance can serialize objects of the 
        /// specified type.
        /// </summary>
        /// <param name="type">
        ///     The type of object to serialize.
        /// </param>
        /// <param name="contentType"></param>
        /// <returns>
        /// A value indicating whether objects of the specified type can be 
        /// serialized.
        /// </returns>
        public bool CanSerialize(Type type, string contentType)
        {
            return ContentTypes.Contains(contentType) && type.GetCustomAttribute<DataContractAttribute>() != null;
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
            var serializer = new DataContractSerializer(type);
            var obj = serializer.ReadObject(source);
            return obj;
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
            var serializer = new DataContractSerializer(type);
            using (var xw = XmlWriter.Create(destination, settings))
                serializer.WriteObject(xw, obj);
        }
    }
}