using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using MazeNET.SerializationXml.Core.Interfaces;

namespace MazeNET.SerializationXml.Infrastructure.Converters
{
    /// <summary>
    /// Implementation of XML file operations
    /// </summary>
    public class XmlFileOperationsService : IXmlFileOperations
    {
        /// <inheritdoc/>
        public bool SaveToFile<T>(string fullPath, XmlDocument document)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            if (document == null) throw new ArgumentNullException(nameof(document));

            document.Save(fullPath);
            return true;
        }

        /// <inheritdoc/>
        public bool SaveToFile<T>(string fullPath, T objectToSerialize)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            if (objectToSerialize == null) throw new ArgumentNullException(nameof(objectToSerialize));

            using (TextWriter writer = new StreamWriter(fullPath))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, objectToSerialize);
                return true;
            }
        }

        /// <inheritdoc/>
        public T LoadFromFile<T>(string fullPath)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));

            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(memoryStream)!;
            }
        }

        /// <inheritdoc/>
        public XmlDocument LoadXml(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("File not found: " + path);

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(stream);
                return xmlDoc;
            }
        }
    }
}
