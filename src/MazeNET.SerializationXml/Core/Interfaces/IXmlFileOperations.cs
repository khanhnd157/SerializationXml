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
    }
}

