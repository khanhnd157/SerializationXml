using System.Xml;
using MazeNET.SerializationXml.Options;

namespace MazeNET.SerializationXml.Abstractions
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
    }
}
