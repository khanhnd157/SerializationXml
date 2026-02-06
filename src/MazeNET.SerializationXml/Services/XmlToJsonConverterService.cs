using System;
using System.IO;
using System.Xml;
using MazeNET.SerializationXml.Abstractions;
using MazeNET.SerializationXml.Exceptions;
using MazeNET.SerializationXml.Internal;
using MazeNET.SerializationXml.Options;

namespace MazeNET.SerializationXml.Services
{
    /// <summary>
    /// Implementation of XML to JSON converter (zero external dependencies)
    /// </summary>
    public class XmlToJsonConverterService : IXmlToJsonConverter
    {
        /// <inheritdoc/>
        public string Convert(XmlDocument document)
        {
            return Convert(document, XmlToJsonOptions.Default);
        }

        /// <inheritdoc/>
        public string Convert(XmlDocument document, XmlToJsonOptions options)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (options == null) throw new ArgumentNullException(nameof(options));

            try
            {
                var writer = new XmlJsonWriter(options);
                return writer.Write(document);
            }
            catch (Exception ex) when (ex is not ArgumentNullException)
            {
                throw new XmlSerializationException(
                    "Failed to convert XmlDocument to JSON.",
                    typeof(XmlDocument), "XmlToJson", ex);
            }
        }

        /// <inheritdoc/>
        public string Convert(string xml)
        {
            return Convert(xml, XmlToJsonOptions.Default);
        }

        /// <inheritdoc/>
        public string Convert(string xml, XmlToJsonOptions options)
        {
            if (string.IsNullOrEmpty(xml)) throw new ArgumentException("XML string cannot be null or empty.", nameof(xml));
            if (options == null) throw new ArgumentNullException(nameof(options));

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                return Convert(doc, options);
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not ArgumentException && ex is not XmlSerializationException)
            {
                throw new XmlSerializationException(
                    "Failed to convert XML string to JSON.",
                    typeof(string), "XmlToJson", ex);
            }
        }

        /// <inheritdoc/>
        public string ConvertFile(string filePath)
        {
            return ConvertFile(filePath, XmlToJsonOptions.Default);
        }

        /// <inheritdoc/>
        public string ConvertFile(string filePath, XmlToJsonOptions options)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found: " + filePath);

            try
            {
                var doc = new XmlDocument();
                doc.Load(filePath);
                return Convert(doc, options);
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not ArgumentException
                && ex is not FileNotFoundException && ex is not XmlSerializationException)
            {
                throw new XmlSerializationException(
                    $"Failed to convert XML file '{filePath}' to JSON.",
                    typeof(XmlDocument), "XmlToJson", ex);
            }
        }
    }
}
