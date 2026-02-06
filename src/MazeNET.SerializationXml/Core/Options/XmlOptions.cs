namespace MazeNET.SerializationXml.Core.Options
{
    /// <summary>
    /// Options for XML serialization
    /// </summary>
    public class XmlOptions
    {
        /// <summary>
        /// XML prefix
        /// </summary>
        public string? Prefix { get; set; }

        /// <summary>
        /// Root element name
        /// </summary>
        public string? RootName { get; set; }

        /// <summary>
        /// XML declaration options
        /// </summary>
        public XmlDeclarationOptions? Declaration { get; set; }

        /// <summary>
        /// Remove XML schema (default: false)
        /// </summary>
        public bool RemoveXmlSchema { get; set; }

        /// <summary>
        /// Remove XML declaration (default: false)
        /// </summary>
        public bool RemoveDeclaration { get; set; }

        /// <summary>
        /// Remove CDATA tags (default: false)
        /// </summary>
        public bool RemoveTagCDDATA { get; set; }
    }
}
