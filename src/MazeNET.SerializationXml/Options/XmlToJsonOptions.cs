namespace MazeNET.SerializationXml.Options
{
    /// <summary>
    /// Options for XML to JSON conversion
    /// </summary>
    public class XmlToJsonOptions
    {
        /// <summary>
        /// Indent JSON output (default: true)
        /// </summary>
        public bool Indent { get; set; } = true;

        /// <summary>
        /// Omit root object wrapper in JSON output (default: false)
        /// </summary>
        public bool OmitRootObject { get; set; }

        /// <summary>
        /// Remove XML declaration node from JSON output (default: true)
        /// </summary>
        public bool OmitXmlDeclaration { get; set; } = true;

        /// <summary>
        /// Prefix for XML attributes in JSON keys (default: "@")
        /// </summary>
        public string AttributePrefix { get; set; } = "@";

        /// <summary>
        /// Key name for text content nodes (default: "#text")
        /// </summary>
        public string TextNodeKey { get; set; } = "#text";

        /// <summary>
        /// Key name for CDATA content nodes (default: "#cdata-section")
        /// </summary>
        public string CDataNodeKey { get; set; } = "#cdata-section";

        /// <summary>
        /// Include XML namespace attributes in JSON (default: false)
        /// </summary>
        public bool IncludeNamespaces { get; set; }

        /// <summary>
        /// Default options
        /// </summary>
        public static XmlToJsonOptions Default => new XmlToJsonOptions();
    }
}
