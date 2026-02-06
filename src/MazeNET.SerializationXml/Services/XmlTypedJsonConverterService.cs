using System;
using System.IO;
using System.Xml;
using MazeNET.SerializationXml.Abstractions;
using MazeNET.SerializationXml.Exceptions;
using MazeNET.SerializationXml.Internal;

namespace MazeNET.SerializationXml.Services
{
    /// <summary>
    /// Implementation of XML to typed JSON conversion (XML → T → JSON)
    /// </summary>
    public class XmlTypedJsonConverterService : IXmlTypedJsonConverter
    {
        /// <inheritdoc/>
        public string ConvertToJson<T>(XmlDocument document, bool indent = true) where T : new()
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            try
            {
                var obj = XmlToObjectMapper.Map<T>(document);
                return new ObjectToJsonWriter(indent).Write(obj);
            }
            catch (Exception ex) when (ex is not ArgumentNullException)
            {
                throw new XmlSerializationException(
                    $"Failed to convert XmlDocument to JSON as type '{typeof(T).FullName}'.",
                    typeof(T), "ConvertToJson", ex);
            }
        }

        /// <inheritdoc/>
        public string ConvertToJson<T>(string xml, bool indent = true) where T : new()
        {
            if (string.IsNullOrEmpty(xml)) throw new ArgumentException("XML string cannot be null or empty.", nameof(xml));

            try
            {
                var obj = XmlToObjectMapper.MapFromString<T>(xml);
                return new ObjectToJsonWriter(indent).Write(obj);
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                throw new XmlSerializationException(
                    $"Failed to convert XML string to JSON as type '{typeof(T).FullName}'.",
                    typeof(T), "ConvertToJson", ex);
            }
        }

        /// <inheritdoc/>
        public string ConvertFileToJson<T>(string filePath, bool indent = true) where T : new()
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (!File.Exists(filePath)) throw new FileNotFoundException("File not found: " + filePath);

            try
            {
                var doc = SafeXmlFactory.CreateDocument();
                doc.Load(filePath);
                var obj = XmlToObjectMapper.Map<T>(doc);
                return new ObjectToJsonWriter(indent).Write(obj);
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not FileNotFoundException)
            {
                throw new XmlSerializationException(
                    $"Failed to convert XML file '{filePath}' to JSON as type '{typeof(T).FullName}'.",
                    typeof(T), "ConvertFileToJson", ex);
            }
        }
    }
}
