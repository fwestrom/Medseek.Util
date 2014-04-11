namespace Medseek.Util.MicroServices
{
    using System;
    using System.IO;

    /// <summary>
    /// Interface for types that provide serialization functionality for 
    /// operations dispatch of micro-services.
    /// </summary>
    public interface IMicroServiceSerializer
    {
        /// <summary>
        /// Deserializes the parameters for invoking a micro-service operation.
        /// </summary>
        object[] Deserialize(IMessageContext messageContext, Type[] types, Stream source, ref object serializerState);

        /// <summary>
        /// Serializes the result from invoking a micro-service operation.
        /// </summary>
        void Serialize(IMessageContext messageContext, Type type, object value, Stream destination);

        /// <summary>
        /// Serializes the result from invoking a micro-service operation.
        /// </summary>
        void Serialize(IMessageContext messageContext, Type type, object value, Stream destination, ref object serializerState);
    }
}