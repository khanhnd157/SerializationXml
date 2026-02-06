using System.Xml;

namespace MazeNET.SerializationXml.Abstractions
{
    /// <summary>
    /// Interface for XML to typed JSON conversion (XML → T → JSON)
    /// </summary>
    public interface IXmlTypedJsonConverter
    {
        /// <summary>
        /// Convert XmlDocument to JSON shaped by type T. Unmatched properties are null.
        /// </summary>
        string ConvertToJson<T>(XmlDocument document, bool indent = true) where T : new();

        /// <summary>
        /// Convert XML string to JSON shaped by type T. Unmatched properties are null.
        /// </summary>
        string ConvertToJson<T>(string xml, bool indent = true) where T : new();

        /// <summary>
        /// Convert XML file to JSON shaped by type T. Unmatched properties are null.
        /// </summary>
        string ConvertFileToJson<T>(string filePath, bool indent = true) where T : new();
    }
}
