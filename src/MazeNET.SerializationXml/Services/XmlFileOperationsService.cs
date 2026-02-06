using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MazeNET.SerializationXml.Abstractions;
using MazeNET.SerializationXml.Exceptions;

namespace MazeNET.SerializationXml.Services
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

            try
            {
                document.Save(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                throw new XmlSerializationException(
                    $"Failed to save XmlDocument to file '{fullPath}'.",
                    typeof(XmlDocument), "SaveToFile", ex);
            }
        }

        /// <inheritdoc/>
        public bool SaveToFile<T>(string fullPath, T objectToSerialize)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            if (objectToSerialize == null) throw new ArgumentNullException(nameof(objectToSerialize));

            try
            {
                using (TextWriter writer = new StreamWriter(fullPath))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, objectToSerialize);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new XmlSerializationException(
                    $"Failed to save object of type '{typeof(T).FullName}' to file '{fullPath}'.",
                    typeof(T), "SaveToFile", ex);
            }
        }

        /// <inheritdoc/>
        public T LoadFromFile<T>(string fullPath)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));

            try
            {
                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;

                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(memoryStream)!;
                }
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not FileNotFoundException)
            {
                throw new XmlSerializationException(
                    $"Failed to load file '{fullPath}' as type '{typeof(T).FullName}'.",
                    typeof(T), "LoadFromFile", ex);
            }
        }

        /// <inheritdoc/>
        public XmlDocument LoadXml(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("File not found: " + path);

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(stream);
                    return xmlDoc;
                }
            }
            catch (Exception ex)
            {
                throw new XmlSerializationException(
                    $"Failed to load XML file '{path}'.",
                    typeof(XmlDocument), "LoadXml", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SaveToFileAsync<T>(string fullPath, XmlDocument document, CancellationToken cancellationToken = default)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            if (document == null) throw new ArgumentNullException(nameof(document));

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    document.Save(memoryStream);
                    memoryStream.Position = 0;

                    using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        await memoryStream.CopyToAsync(fileStream, 81920, cancellationToken).ConfigureAwait(false);
                    }
                }

                return true;
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not OperationCanceledException)
            {
                throw new XmlSerializationException(
                    $"Failed to save XmlDocument to file '{fullPath}'.",
                    typeof(XmlDocument), "SaveToFileAsync", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SaveToFileAsync<T>(string fullPath, T objectToSerialize, CancellationToken cancellationToken = default)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            if (objectToSerialize == null) throw new ArgumentNullException(nameof(objectToSerialize));

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(memoryStream, objectToSerialize);
                    memoryStream.Position = 0;

                    using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        await memoryStream.CopyToAsync(fileStream, 81920, cancellationToken).ConfigureAwait(false);
                    }
                }

                return true;
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not OperationCanceledException)
            {
                throw new XmlSerializationException(
                    $"Failed to save object of type '{typeof(T).FullName}' to file '{fullPath}'.",
                    typeof(T), "SaveToFileAsync", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<T> LoadFromFileAsync<T>(string fullPath, CancellationToken cancellationToken = default)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));

            try
            {
                using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream, 81920, cancellationToken).ConfigureAwait(false);
                    memoryStream.Position = 0;

                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(memoryStream)!;
                }
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not FileNotFoundException && ex is not OperationCanceledException)
            {
                throw new XmlSerializationException(
                    $"Failed to load file '{fullPath}' as type '{typeof(T).FullName}'.",
                    typeof(T), "LoadFromFileAsync", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<XmlDocument> LoadXmlAsync(string path, CancellationToken cancellationToken = default)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("File not found: " + path);

            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream, 81920, cancellationToken).ConfigureAwait(false);
                    memoryStream.Position = 0;

                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(memoryStream);
                    return xmlDoc;
                }
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not FileNotFoundException && ex is not OperationCanceledException)
            {
                throw new XmlSerializationException(
                    $"Failed to load XML file '{path}'.",
                    typeof(XmlDocument), "LoadXmlAsync", ex);
            }
        }
    }
}
