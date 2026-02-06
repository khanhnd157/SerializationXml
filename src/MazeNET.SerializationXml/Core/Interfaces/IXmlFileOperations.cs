using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace MazeNET.SerializationXml.Core.Interfaces
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
        /// Load XML file to object
        /// </summary>
        T LoadFromFile<T>(string fullPath);

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
        /// Load XML file to object asynchronously
        /// </summary>
        Task<T> LoadFromFileAsync<T>(string fullPath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Load XML file to XmlDocument asynchronously
        /// </summary>
        Task<XmlDocument> LoadXmlAsync(string path, CancellationToken cancellationToken = default);
    }
}
