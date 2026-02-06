using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MazeNET.SerializationXml.Options;

namespace MazeNET.SerializationXml.Abstractions
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

        /// <summary>
        /// Serialize an object to XmlDocument with custom options asynchronously
        /// </summary>
        Task<XmlDocument> SerializeAsync<T>(T dataObject, Func<XmlOptionsBuilder, XmlOptionsBuilder> builder, CancellationToken cancellationToken = default);

        /// <summary>
        /// Serialize an object to XmlDocument with default options asynchronously
        /// </summary>
        Task<XmlDocument> SerializeAsync<T>(T dataObject, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deserialize XML string to object asynchronously
        /// </summary>
        Task<T> DeserializeAsync<T>(string dataxml, CancellationToken cancellationToken = default) where T : new();

        /// <summary>
        /// Deserialize XmlDocument to object asynchronously
        /// </summary>
        Task<T> DeserializeAsync<T>(XmlDocument xmlDoc, CancellationToken cancellationToken = default) where T : new();
    }
}
