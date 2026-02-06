using System;
using System.IO;
using System.Text;
using System.Xml;
using MazeNET.SerializationXml.Core.Options;
using MazeNET.SerializationXml.Infrastructure.Converters;

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
            if (document == null) throw new ArgumentNullException(nameof(document));

            var conformanceLevel = ConformanceLevel.Auto;
            var omitDeclaration = true;

            if (document.FirstChild is XmlDeclaration)
            {
                conformanceLevel = ConformanceLevel.Fragment;
                omitDeclaration = false;
            }

            using (var memoryStream = new MemoryStream())
            using (var writer = XmlWriter.Create(memoryStream, new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = omitDeclaration,
                ConformanceLevel = conformanceLevel,
                Indent = true,
                NewLineOnAttributes = false
            }))
            {
                document.WriteContentTo(writer);
                writer.Flush();
                memoryStream.Position = 0;
                using (var reader = new StreamReader(memoryStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Build XmlDocument with custom options
        /// </summary>
        public static XmlDocument Builder(this XmlDocument document, Func<XmlOptionsBuilder, XmlOptionsBuilder> builder)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            var options = builder(new XmlOptionsBuilder()).Build();

            var xmldocResult = new XmlDocument();

            var rootNode = options.RootName?.Trim();

            if (!string.IsNullOrEmpty(rootNode))
            {
                var innerXml = document.LastChild?.InnerXml ?? document.InnerXml;

                XmlElement root = options.RemoveXmlSchema
                    ? xmldocResult.CreateElement(rootNode)
                    : xmldocResult.CreateElement(rootNode, document.LastChild?.NamespaceURI ?? string.Empty);

                root.InnerXml = innerXml;
                xmldocResult.AppendChild(root);
            }

            if (xmldocResult.FirstChild is XmlDeclaration)
            {
                xmldocResult.RemoveChild(xmldocResult.FirstChild);
            }

            if (!options.RemoveDeclaration)
            {
                var version = "1.0";
                var encoding = Encoding.UTF8.BodyName;
                var standalone = "yes";

                if (options.Declaration != null)
                {
                    version = !string.IsNullOrEmpty(options.Declaration.Version?.Trim())
                        ? options.Declaration.Version
                        : "1.0";
                    encoding = options.Declaration.Encoding?.BodyName ?? Encoding.UTF8.BodyName;
                    standalone = options.Declaration.Standalone ? "yes" : "no";
                }

                var xmldecl = xmldocResult.CreateXmlDeclaration(version, encoding, standalone);
                xmldocResult.InsertBefore(xmldecl, xmldocResult.DocumentElement);
            }

            if (options.RemoveTagCDDATA)
            {
                xmldocResult.InnerXml = xmldocResult.InnerXml
                    .Replace("<![CDATA[", "")
                    .Replace("]]>", "");
            }

            return xmldocResult;
        }

        private static readonly XmlToJsonConverterService _jsonConverter = new XmlToJsonConverterService();

        /// <summary>
        /// Convert XmlDocument to JSON string with default options
        /// </summary>
        public static string ToJson(this XmlDocument document)
        {
            return _jsonConverter.Convert(document);
        }

        /// <summary>
        /// Convert XmlDocument to JSON string with custom options
        /// </summary>
        public static string ToJson(this XmlDocument document, XmlToJsonOptions options)
        {
            return _jsonConverter.Convert(document, options);
        }

        /// <summary>
        /// Convert XmlDocument to typed object. Unmatched properties remain null/default.
        /// </summary>
        public static T ToObject<T>(this XmlDocument document) where T : new()
        {
            return _jsonConverter.ConvertTo<T>(document);
        }
    }
}
