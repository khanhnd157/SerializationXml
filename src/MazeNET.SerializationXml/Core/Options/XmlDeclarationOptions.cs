using System.Text;

namespace MazeNET.SerializationXml.Core.Options
{
#if NET9_0_OR_GREATER
    /// <summary>
    /// Options for XML declaration
    /// </summary>
    public class XmlDeclarationOptions
    {
        /// <summary>
        /// XML version (default: "1.0")
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// XML encoding (default: UTF-8)
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Standalone declaration (default: true)
        /// </summary>
        public bool Standalone { get; set; } = true;
    }
#else
    /// <summary>
    /// Options for XML declaration
    /// </summary>
    public class XmlDeclarationOptions
    {
        /// <summary>
        /// XML version (default: "1.0")
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// XML encoding (default: UTF-8)
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Standalone declaration (default: true)
        /// </summary>
        public bool Standalone { get; set; } = true;
    }
#endif
}

