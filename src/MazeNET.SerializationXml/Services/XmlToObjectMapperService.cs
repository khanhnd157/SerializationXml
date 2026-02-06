using System;
using System.IO;
using System.Xml;
using MazeNET.SerializationXml.Abstractions;
using MazeNET.SerializationXml.Exceptions;
using MazeNET.SerializationXml.Internal;

namespace MazeNET.SerializationXml.Services
{
    /// <summary>
    /// Implementation of XML to typed object mapping
    /// </summary>
    public class XmlToObjectMapperService : IXmlToObjectMapper
    {
        /// <inheritdoc/>
        public T MapTo<T>(XmlDocument document) where T : new()
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            try
            {
                return XmlToObjectMapper.Map<T>(document);
            }
            catch (Exception ex) when (ex is not ArgumentNullException)
            {
                throw new XmlSerializationException(
                    $"Failed to convert XmlDocument to type '{typeof(T).FullName}'.",
                    typeof(T), "MapTo", ex);
            }
        }

        /// <inheritdoc/>
        public T MapTo<T>(string xml) where T : new()
        {
            if (string.IsNullOrEmpty(xml)) throw new ArgumentException("XML string cannot be null or empty.", nameof(xml));

            try
            {
                return XmlToObjectMapper.MapFromString<T>(xml);
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                throw new XmlSerializationException(
                    $"Failed to convert XML string to type '{typeof(T).FullName}'.",
                    typeof(T), "MapTo", ex);
            }
        }

        /// <inheritdoc/>
        public T MapFileTo<T>(string filePath) where T : new()
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found: " + filePath);

            try
            {
                var doc = new XmlDocument();
                doc.Load(filePath);
                return XmlToObjectMapper.Map<T>(doc);
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not FileNotFoundException)
            {
                throw new XmlSerializationException(
                    $"Failed to convert XML file '{filePath}' to type '{typeof(T).FullName}'.",
                    typeof(T), "MapFileTo", ex);
            }
        }
    }
}
