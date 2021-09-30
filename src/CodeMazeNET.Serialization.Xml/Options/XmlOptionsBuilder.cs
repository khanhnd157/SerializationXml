namespace CodeMazeNET.Serialization.Xml
{
    public class XmlOptionsBuilder
    {
        private readonly XmlOptions _options;

        internal XmlOptionsBuilder()
        {
            _options = new XmlOptions();
        }

        public XmlOptionsBuilder AddDeclaration(XmlDeclarationOptions declaration)
        {
            _options.Declaration = declaration;

            return this;
        }
        public XmlOptionsBuilder AddPrefix(string prefix)
        {
            _options.PreFix = prefix;

            return this;
        }

        public XmlOptionsBuilder RemoveSchema(bool remove = true)
        {
            _options.RemoveXmlSchema = remove;

            return this;
        }

        public XmlOptionsBuilder RemoveDeclaration(bool remove = true)
        {
            _options.RemoveDeclaration = remove;

            return this;
        }

        public XmlOptionsBuilder RemoveTagCDDATA(bool remove = true)
        {
            _options.RemoveTagCDDATA = remove;

            return this;
        }


        public XmlOptionsBuilder RootElement(string name)
        {
            _options.RootName = name;

            return this;
        }


        public XmlOptions Build()
        {
            return _options;
        }
    }

}
