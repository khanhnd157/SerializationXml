using System;
using System.Xml;
using MazeNET.SerializationXml.Core.Interfaces;
using MazeNET.SerializationXml.Core.Options;
using MazeNET.SerializationXml.Infrastructure.Converters;

namespace MazeNET.SerializationXml
{
    /// <summary>
    /// Facade for XML conversion operations (backward compatibility)
    /// </summary>
    public static class XmlConverter
    {
        private static readonly IXmlSerializer _serializer = new XmlSerializerService();
        private static readonly IXmlFileOperations _fileOps = new XmlFileOperationsService();

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
    }
}
