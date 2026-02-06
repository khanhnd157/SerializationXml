using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MazeNET.SerializationXml.Abstractions;
using MazeNET.SerializationXml.Options;
using MazeNET.SerializationXml.Services;

namespace MazeNET.SerializationXml
{
    /// <summary>
    /// Static facade for all XML operations
    /// </summary>
    public static class XmlConverter
    {
        private static readonly IXmlSerializer _serializer = new XmlSerializerService();
        private static readonly IXmlFileOperations _fileOps = new XmlFileOperationsService();
        private static readonly IXmlToJsonConverter _jsonConverter = new XmlToJsonConverterService();
        private static readonly IXmlToObjectMapper _objectMapper = new XmlToObjectMapperService();
        private static readonly IXmlTypedJsonConverter _typedJsonConverter = new XmlTypedJsonConverterService();

        /// <summary>
        /// Save XmlDocument to file
        /// </summary>
        public static bool SaveToFile<T>(string fullPath, XmlDocument document)
        {
            return _fileOps.SaveToFile<T>(fullPath, document);
        }

        /// <summary>
        /// Save object to XML file
        /// </summary>
        public static bool SaveToFile<T>(string fullPath, T objectToSerialize)
        {
            return _fileOps.SaveToFile(fullPath, objectToSerialize);
        }

        /// <summary>
        /// Load XML file to object
        /// </summary>
        public static T FileToObject<T>(string fullPath)
        {
            return _fileOps.LoadFromFile<T>(fullPath);
        }

        /// <summary>
        /// Serialize object to XmlDocument with custom options
        /// </summary>
        public static XmlDocument SerializeObject<T>(T dataObject, Func<XmlOptionsBuilder, XmlOptionsBuilder> builder)
        {
            return _serializer.Serialize(dataObject, builder);
        }

        /// <summary>
        /// Serialize object to XmlDocument with default options
        /// </summary>
        public static XmlDocument SerializeObject<T>(T dataObject)
        {
            return _serializer.Serialize(dataObject);
        }

        /// <summary>
        /// Load XML file to XmlDocument
        /// </summary>
        public static XmlDocument LoadXml(string path)
        {
            return _fileOps.LoadXml(path);
        }

        /// <summary>
        /// Deserialize XML string to object
        /// </summary>
        public static T DeserializeObject<T>(string dataxml) where T : new()
        {
            return _serializer.Deserialize<T>(dataxml);
        }

        /// <summary>
        /// Deserialize XmlDocument to object
        /// </summary>
        public static T DeserializeObject<T>(XmlDocument xmlDoc) where T : new()
        {
            return _serializer.Deserialize<T>(xmlDoc);
        }

        /// <summary>
        /// Save XmlDocument to file asynchronously
        /// </summary>
        public static Task<bool> SaveToFileAsync<T>(string fullPath, XmlDocument document, CancellationToken cancellationToken = default)
        {
            return _fileOps.SaveToFileAsync<T>(fullPath, document, cancellationToken);
        }

        /// <summary>
        /// Save object to XML file asynchronously
        /// </summary>
        public static Task<bool> SaveToFileAsync<T>(string fullPath, T objectToSerialize, CancellationToken cancellationToken = default)
        {
            return _fileOps.SaveToFileAsync(fullPath, objectToSerialize, cancellationToken);
        }

        /// <summary>
        /// Load XML file to object asynchronously
        /// </summary>
        public static Task<T> FileToObjectAsync<T>(string fullPath, CancellationToken cancellationToken = default)
        {
            return _fileOps.LoadFromFileAsync<T>(fullPath, cancellationToken);
        }

        /// <summary>
        /// Serialize object to XmlDocument with custom options asynchronously
        /// </summary>
        public static Task<XmlDocument> SerializeObjectAsync<T>(T dataObject, Func<XmlOptionsBuilder, XmlOptionsBuilder> builder, CancellationToken cancellationToken = default)
        {
            return _serializer.SerializeAsync(dataObject, builder, cancellationToken);
        }

        /// <summary>
        /// Serialize object to XmlDocument with default options asynchronously
        /// </summary>
        public static Task<XmlDocument> SerializeObjectAsync<T>(T dataObject, CancellationToken cancellationToken = default)
        {
            return _serializer.SerializeAsync(dataObject, cancellationToken);
        }

        /// <summary>
        /// Load XML file to XmlDocument asynchronously
        /// </summary>
        public static Task<XmlDocument> LoadXmlAsync(string path, CancellationToken cancellationToken = default)
        {
            return _fileOps.LoadXmlAsync(path, cancellationToken);
        }

        /// <summary>
        /// Deserialize XML string to object asynchronously
        /// </summary>
        public static Task<T> DeserializeObjectAsync<T>(string dataxml, CancellationToken cancellationToken = default) where T : new()
        {
            return _serializer.DeserializeAsync<T>(dataxml, cancellationToken);
        }

        /// <summary>
        /// Deserialize XmlDocument to object asynchronously
        /// </summary>
        public static Task<T> DeserializeObjectAsync<T>(XmlDocument xmlDoc, CancellationToken cancellationToken = default) where T : new()
        {
            return _serializer.DeserializeAsync<T>(xmlDoc, cancellationToken);
        }

        /// <summary>
        /// Convert XmlDocument to JSON string
        /// </summary>
        public static string XmlToJson(XmlDocument document)
        {
            return _jsonConverter.Convert(document);
        }

        /// <summary>
        /// Convert XmlDocument to JSON string with custom options
        /// </summary>
        public static string XmlToJson(XmlDocument document, XmlToJsonOptions options)
        {
            return _jsonConverter.Convert(document, options);
        }

        /// <summary>
        /// Convert XML string to JSON string
        /// </summary>
        public static string XmlToJson(string xml)
        {
            return _jsonConverter.Convert(xml);
        }

        /// <summary>
        /// Convert XML string to JSON string with custom options
        /// </summary>
        public static string XmlToJson(string xml, XmlToJsonOptions options)
        {
            return _jsonConverter.Convert(xml, options);
        }

        /// <summary>
        /// Convert XML file to JSON string
        /// </summary>
        public static string XmlFileToJson(string filePath)
        {
            return _jsonConverter.ConvertFile(filePath);
        }

        /// <summary>
        /// Convert XML file to JSON string with custom options
        /// </summary>
        public static string XmlFileToJson(string filePath, XmlToJsonOptions options)
        {
            return _jsonConverter.ConvertFile(filePath, options);
        }

        /// <summary>
        /// Convert XmlDocument to typed object. Unmatched properties remain null/default.
        /// </summary>
        public static T XmlToObject<T>(XmlDocument document) where T : new()
        {
            return _objectMapper.MapTo<T>(document);
        }

        /// <summary>
        /// Convert XML string to typed object. Unmatched properties remain null/default.
        /// </summary>
        public static T XmlToObject<T>(string xml) where T : new()
        {
            return _objectMapper.MapTo<T>(xml);
        }

        /// <summary>
        /// Convert XML file to typed object. Unmatched properties remain null/default.
        /// </summary>
        public static T XmlFileToObject<T>(string filePath) where T : new()
        {
            return _objectMapper.MapFileTo<T>(filePath);
        }

        /// <summary>
        /// Convert XmlDocument to JSON shaped by type T. Unmatched properties are null.
        /// </summary>
        public static string XmlToJson<T>(XmlDocument document, bool indent = true) where T : new()
        {
            return _typedJsonConverter.ConvertToJson<T>(document, indent);
        }

        /// <summary>
        /// Convert XML string to JSON shaped by type T. Unmatched properties are null.
        /// </summary>
        public static string XmlToJson<T>(string xml, bool indent = true) where T : new()
        {
            return _typedJsonConverter.ConvertToJson<T>(xml, indent);
        }

        /// <summary>
        /// Convert XML file to JSON shaped by type T. Unmatched properties are null.
        /// </summary>
        public static string XmlFileToJson<T>(string filePath, bool indent = true) where T : new()
        {
            return _typedJsonConverter.ConvertFileToJson<T>(filePath, indent);
        }
    }
}
