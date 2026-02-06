using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace MazeNET.SerializationXml.Abstractions
{
    /// <summary>
    /// Interface for XML file operations
    /// </summary>
    public interface IXmlFileOperations
    {
        /// <summary>
        /// Save object to XML file
        /// </summary>
        bool SaveToFile<T>(string fullPath, T objectToSerialize);

        /// <summary>
        /// Save XmlDocument to file
        /// </summary>
        bool SaveToFile<T>(string fullPath, XmlDocument document);

        /// <summary>
        /// Load XML file to object. Auto-selects streaming for large files (default threshold: 50MB).
        /// </summary>
        T LoadFromFile<T>(string fullPath, long streamingThresholdBytes = 50 * 1024 * 1024);

        /// <summary>
        /// Load XML file to XmlDocument
        /// </summary>
        XmlDocument LoadXml(string path);

        /// <summary>
        /// Save object to XML file asynchronously
        /// </summary>
        Task<bool> SaveToFileAsync<T>(string fullPath, T objectToSerialize, CancellationToken cancellationToken = default);

        /// <summary>
        /// Save XmlDocument to file asynchronously
        /// </summary>
        Task<bool> SaveToFileAsync<T>(string fullPath, XmlDocument document, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load XML file to object asynchronously. Auto-selects streaming for large files (default threshold: 50MB).
        /// </summary>
        Task<T> LoadFromFileAsync<T>(string fullPath, long streamingThresholdBytes = 50 * 1024 * 1024, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load XML file to XmlDocument asynchronously
        /// </summary>
        Task<XmlDocument> LoadXmlAsync(string path, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stream-deserialize large XML file to object using XmlReader (low memory)
        /// </summary>
        T DeserializeStream<T>(string filePath);

        /// <summary>
        /// Stream-deserialize large XML file to object using XmlReader asynchronously (low memory)
        /// </summary>
        Task<T> DeserializeStreamAsync<T>(string filePath, CancellationToken cancellationToken = default);

    }
}
