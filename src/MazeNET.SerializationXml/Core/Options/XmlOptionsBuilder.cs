using System;
using System.Xml;

namespace MazeNET.SerializationXml.Core.Options
{
    /// <summary>
    /// Builder for XML options
    /// </summary>
    public class XmlOptionsBuilder
    {
        private readonly XmlOptions _options;

        internal XmlOptionsBuilder()
        {
            _options = new XmlOptions();
        }

        /// <summary>
        /// Add XML declaration options
        /// </summary>
        public XmlOptionsBuilder AddDeclaration(XmlDeclarationOptions declaration)
        {
            if (declaration == null) throw new ArgumentNullException(nameof(declaration));
            _options.Declaration = declaration;
            return this;
        }

        /// <summary>
        /// Add XML prefix
        /// </summary>
        public XmlOptionsBuilder AddPrefix(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Prefix cannot be null or whitespace.", nameof(prefix));
            _options.Prefix = prefix;
            return this;
        }

        /// <summary>
        /// Remove or keep XML schema
        /// </summary>
        public XmlOptionsBuilder RemoveSchema(bool remove = true)
        {
            _options.RemoveXmlSchema = remove;
            return this;
        }

        /// <summary>
        /// Remove or keep XML declaration
        /// </summary>
        public XmlOptionsBuilder RemoveDeclaration(bool remove = true)
        {
            _options.RemoveDeclaration = remove;
            return this;
        }

        /// <summary>
        /// Remove or keep CDATA tags
        /// </summary>
        public XmlOptionsBuilder RemoveTagCDDATA(bool remove = true)
        {
            _options.RemoveTagCDDATA = remove;
            return this;
        }

        /// <summary>
        /// Set root element name (must be a valid XML name)
        /// </summary>
        public XmlOptionsBuilder RootElement(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Root element name cannot be null or whitespace.", nameof(name));

            try
            {
                XmlConvert.VerifyName(name);
            }
            catch (XmlException ex)
            {
                throw new ArgumentException($"'{name}' is not a valid XML element name.", nameof(name), ex);
            }

            _options.RootName = name;
            return this;
        }

        /// <summary>
        /// Build the XML options
        /// </summary>
        public XmlOptions Build()
        {
            return _options;
        }
    }
}
