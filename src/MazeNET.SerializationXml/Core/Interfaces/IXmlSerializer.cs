using System;
using System.Xml;
using MazeNET.SerializationXml.Core.Options;

namespace MazeNET.SerializationXml.Core.Interfaces
{
    /// <summary>
    /// Interface for XML serialization operations
    /// </summary>
    public interface IXmlSerializer
    {
        /// <summary>
        /// Serialize an object to XmlDocument with custom options
        /// </summary>
        XmlDocument Serialize<T>(T dataObject, Func<XmlOptionsBuilder, XmlOptionsBuilder> builder);

        /// <summary>
        /// Serialize an object to XmlDocument with default options
        /// </summary>
        XmlDocument Serialize<T>(T dataObject);

        /// <summary>
        /// Deserialize XML string to object
        /// </summary>
        T Deserialize<T>(string dataxml) where T : new();

        /// <summary>
        /// Deserialize XmlDocument to object
        /// </summary>
        T Deserialize<T>(XmlDocument xmlDoc) where T : new();
    }
}

