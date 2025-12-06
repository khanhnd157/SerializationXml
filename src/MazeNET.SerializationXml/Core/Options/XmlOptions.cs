namespace MazeNET.SerializationXml.Core.Options
{
#if NET9_0_OR_GREATER
    /// <summary>
    /// Options for XML serialization
    /// </summary>
    public class XmlOptions
    {
        /// <summary>
        /// XML prefix
        /// </summary>
        public string? PreFix { get; set; }

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
        public bool RemoveXmlSchema { get; set; } = false;

        /// <summary>
        /// Remove XML declaration (default: false)
        /// </summary>
        public bool RemoveDeclaration { get; set; } = false;

        /// <summary>
        /// Remove CDATA tags (default: false)
        /// </summary>
        public bool RemoveTagCDDATA { get; set; } = false;
    }
#else
    /// <summary>
    /// Options for XML serialization
    /// </summary>
    public class XmlOptions
    {
        /// <summary>
        /// XML prefix
        /// </summary>
        public string PreFix { get; set; }

        /// <summary>
        /// Root element name
        /// </summary>
        public string RootName { get; set; }

        /// <summary>
        /// XML declaration options
        /// </summary>
        public XmlDeclarationOptions Declaration { get; set; }

        /// <summary>
        /// Remove XML schema (default: false)
        /// </summary>
        public bool RemoveXmlSchema { get; set; } = false;

        /// <summary>
        /// Remove XML declaration (default: false)
        /// </summary>
        public bool RemoveDeclaration { get; set; } = false;

        /// <summary>
        /// Remove CDATA tags (default: false)
        /// </summary>
        public bool RemoveTagCDDATA { get; set; } = false;
    }
#endif
}

