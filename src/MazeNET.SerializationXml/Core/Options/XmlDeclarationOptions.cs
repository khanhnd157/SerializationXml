using System.Text;

namespace MazeNET.SerializationXml.Core.Options
{
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
}
