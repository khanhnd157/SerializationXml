using System;
using System.IO;
using System.Text;
using System.Xml;
using MazeNET.SerializationXml.Core.Interfaces;
using MazeNET.SerializationXml.Core.Options;

namespace MazeNET.SerializationXml.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for XmlDocument
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Convert XmlDocument to string
        /// </summary>
        public static string ConvertToString(this XmlDocument document)
        {
#if NET9_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(document);
#endif

            var conformanceLevel = ConformanceLevel.Auto;
            var isDeclaration = true;

            if ((document.FirstChild as XmlDeclaration) != null)
            {
                // remove old xml declaration
                conformanceLevel = ConformanceLevel.Fragment;
                isDeclaration = false;
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (XmlWriter writer = XmlWriter.Create(
                memoryStream,
                new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = isDeclaration,
                    ConformanceLevel = conformanceLevel,
                    Indent = true,
                    NewLineOnAttributes = false
                }))
            {
                document.WriteContentTo(writer);
                writer.Flush();
                memoryStream.Flush();
                memoryStream.Position = 0;
                using (StreamReader streamReader = new StreamReader(memoryStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Build XmlDocument with custom options
        /// </summary>
        public static XmlDocument Builder(this XmlDocument document, Func<XmlOptionsBuilder, XmlOptionsBuilder> builder)
        {
#if NET9_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(document);
            ArgumentNullException.ThrowIfNull(builder);
#endif

            var xmlbuilder = new XmlOptionsBuilder();
            var options = builder(xmlbuilder).Build();

            if (options != null)
            {
                XmlDocument xmldocResult = new XmlDocument();

                var _rootNode = options.RootName?.Trim();

                if (!string.IsNullOrEmpty(_rootNode))
                {
                    var _innerXml = (document.LastChild?.InnerXml ?? document.InnerXml);

                    XmlElement root = options.RemoveXmlSchema
                        ? xmldocResult.CreateElement(_rootNode)
                        : xmldocResult.CreateElement(_rootNode, document.LastChild?.NamespaceURI ?? string.Empty);

                    root.InnerXml = _innerXml;

                    xmldocResult.AppendChild(root);
                }

                if ((xmldocResult.FirstChild as XmlDeclaration) != null)
                {
                    // remove old xml declaration
                    xmldocResult.RemoveChild(xmldocResult.FirstChild);
                }

                if (!options.RemoveDeclaration)
                {
                    var _version = "1.0";
                    var _encoding = Encoding.UTF8.BodyName;
                    var _standalone = "yes";

                    if (options.Declaration != null)
                    {
                        _version = !string.IsNullOrEmpty(options.Declaration.Version?.Trim()) ? options.Declaration.Version : "1.0";
#if NET9_0_OR_GREATER
                        _encoding = options.Declaration.Encoding?.BodyName ?? Encoding.UTF8.BodyName;
#else
                        _encoding = options.Declaration.Encoding?.BodyName;
#endif
                        _standalone = options.Declaration.Standalone ? "yes" : "no";
                    }

                    XmlDeclaration xmldecl = xmldocResult.CreateXmlDeclaration(_version, _encoding, _standalone);
                    //Add the new node to the document.
#if NET9_0_OR_GREATER
                    XmlElement? root = xmldocResult.DocumentElement;
#else
                    XmlElement root = xmldocResult.DocumentElement;
#endif
                    xmldocResult.InsertBefore(xmldecl, root);
                }

                if (options.RemoveTagCDDATA)
                {
                    xmldocResult.InnerXml = xmldocResult.InnerXml.Replace("<![CDATA[", "").Replace("]]>", "");
                }

                return xmldocResult;
            }

            return document;
        }
    }
}

