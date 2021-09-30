namespace CodeMazeNET.Serialization.Xml
{
    public class XmlOptions
    {
        public string PreFix { get; set; }
        public string RootName { get; set; }
        public XmlDeclarationOptions Declaration { get; set; }
        public bool RemoveXmlSchema { get; set; } = false;
        public bool RemoveDeclaration { get; set; } = false;
        public bool RemoveTagCDDATA { get; set; } = false;
    }

}
