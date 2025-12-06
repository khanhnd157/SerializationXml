using System;
using System.Xml;
using MazeNET.SerializationXml.Core.Options;

namespace MazeNET.SerializationXml.Core.Interfaces
{
    /// <summary>
    /// Interface for XmlDocument conversion operations
    /// </summary>
    public interface IXmlDocumentConverter
    {
        /// <summary>
        /// Convert XmlDocument to string
        /// </summary>
        string ConvertToString(XmlDocument document);

        /// <summary>
        /// Build XmlDocument with custom options
        /// </summary>
        XmlDocument Build(XmlDocument document, Func<XmlOptionsBuilder, XmlOptionsBuilder> builder);
    }
}

