using System.Xml;

namespace MazeNET.SerializationXml.Abstractions
{
    /// <summary>
    /// Interface for XML to typed object mapping. Unmatched properties remain null/default.
    /// </summary>
    public interface IXmlToObjectMapper
    {
        /// <summary>
        /// Convert XmlDocument to typed object
        /// </summary>
        T MapTo<T>(XmlDocument document) where T : new();

        /// <summary>
        /// Convert XML string to typed object
        /// </summary>
        T MapTo<T>(string xml) where T : new();

        /// <summary>
        /// Convert XML file to typed object
        /// </summary>
        T MapFileTo<T>(string filePath) where T : new();
    }
}
