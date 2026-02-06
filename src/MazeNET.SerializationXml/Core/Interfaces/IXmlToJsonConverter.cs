using System.Xml;
using MazeNET.SerializationXml.Core.Options;

namespace MazeNET.SerializationXml.Core.Interfaces
{
    /// <summary>
    /// Interface for XML to JSON conversion operations
    /// </summary>
    public interface IXmlToJsonConverter
    {
        /// <summary>
        /// Convert XmlDocument to JSON string
        /// </summary>
        string Convert(XmlDocument document);

        /// <summary>
        /// Convert XmlDocument to JSON string with custom options
        /// </summary>
        string Convert(XmlDocument document, XmlToJsonOptions options);

        /// <summary>
        /// Convert XML string to JSON string
        /// </summary>
        string Convert(string xml);

        /// <summary>
        /// Convert XML string to JSON string with custom options
        /// </summary>
        string Convert(string xml, XmlToJsonOptions options);

        /// <summary>
        /// Convert XML file to JSON string
        /// </summary>
        string ConvertFile(string filePath);

        /// <summary>
        /// Convert XML file to JSON string with custom options
        /// </summary>
        string ConvertFile(string filePath, XmlToJsonOptions options);

        /// <summary>
        /// Convert XmlDocument to typed object. Unmatched properties remain null/default.
        /// </summary>
        T ConvertTo<T>(XmlDocument document) where T : new();

        /// <summary>
        /// Convert XML string to typed object. Unmatched properties remain null/default.
        /// </summary>
        T ConvertTo<T>(string xml) where T : new();

        /// <summary>
        /// Convert XML file to typed object. Unmatched properties remain null/default.
        /// </summary>
        T ConvertFileTo<T>(string filePath) where T : new();
    }
}
