using System;

namespace MazeNET.SerializationXml.Exceptions
{
    /// <summary>
    /// Exception thrown when XML serialization or deserialization fails
    /// </summary>
    public class XmlSerializationException : Exception
    {
        /// <summary>
        /// The type that was being serialized/deserialized when the error occurred
        /// </summary>
        public Type? TargetType { get; }

        /// <summary>
        /// The operation that failed (Serialize, Deserialize, SaveToFile, LoadFromFile, LoadXml)
        /// </summary>
        public string? Operation { get; }

        /// <summary>
        /// Initializes a new instance with a message
        /// </summary>
        public XmlSerializationException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance with a message and inner exception
        /// </summary>
        public XmlSerializationException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance with message, target type, operation name, and inner exception
        /// </summary>
        public XmlSerializationException(string message, Type targetType, string operation, Exception innerException)
            : base(message, innerException)
        {
            TargetType = targetType;
            Operation = operation;
        }
    }
}
