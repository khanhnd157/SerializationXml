using System.Text;

namespace CodeMazeNET.Serialization.Xml
{
    public class XmlDeclarationOptions
    {
        public string Version { get; set; } = "1.0";
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public bool Standalone { get; set; } = true;
    }

}
