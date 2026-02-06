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
        public T LoadFromFile<T>(string fullPath, long streamingThresholdBytes = 50 * 1024 * 1024)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            if (!File.Exists(fullPath)) throw new FileNotFoundException("File not found: " + fullPath);

            var fileSize = new FileInfo(fullPath).Length;
            if (fileSize >= streamingThresholdBytes)
                return DeserializeStream<T>(fullPath);

            try
            {
                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 65536))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stream)!;
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
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 65536))
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
        public async Task<T> LoadFromFileAsync<T>(string fullPath, long streamingThresholdBytes = 50 * 1024 * 1024, CancellationToken cancellationToken = default)
        {
            if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
            if (!File.Exists(fullPath)) throw new FileNotFoundException("File not found: " + fullPath);

            var fileSize = new FileInfo(fullPath).Length;
            if (fileSize >= streamingThresholdBytes)
                return await DeserializeStreamAsync<T>(fullPath, cancellationToken).ConfigureAwait(false);

            try
            {
                using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var serializer = new XmlSerializer(typeof(T));
                    return await Task.Run(() => (T)serializer.Deserialize(fileStream)!, cancellationToken).ConfigureAwait(false);
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
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return await Task.Run(() =>
                    {
                        var xmlDoc = new XmlDocument();
                        xmlDoc.Load(fileStream);
                        return xmlDoc;
                    }, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex) when (ex is not ArgumentNullException && ex is not FileNotFoundException && ex is not OperationCanceledException)
            {
                throw new XmlSerializationException(
                    $"Failed to load XML file '{path}'.",
                    typeof(XmlDocument), "LoadXmlAsync", ex);
            }
        }

        private static readonly XmlReaderSettings _streamReaderSettings = new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            IgnoreProcessingInstructions = true,
            CloseInput = false
        };

        /// <inheritdoc/>
        public T DeserializeStream<T>(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (!File.Exists(filePath)) throw new FileNotFoundException("File not found: " + filePath);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 65536))
                using (var reader = XmlReader.Create(stream, _streamReaderSettings))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(reader)!;
                }
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not FileNotFoundException)
            {
                throw new XmlSerializationException(
                    $"Failed to stream-deserialize file '{filePath}' as type '{typeof(T).FullName}'.",
                    typeof(T), "DeserializeStream", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<T> DeserializeStreamAsync<T>(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (!File.Exists(filePath)) throw new FileNotFoundException("File not found: " + filePath);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 65536, true))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return await Task.Run(() =>
                    {
                        using (var reader = XmlReader.Create(stream, _streamReaderSettings))
                        {
                            var serializer = new XmlSerializer(typeof(T));
                            return (T)serializer.Deserialize(reader)!;
                        }
                    }, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not FileNotFoundException && ex is not OperationCanceledException)
            {
                throw new XmlSerializationException(
                    $"Failed to stream-deserialize file '{filePath}' as type '{typeof(T).FullName}'.",
                    typeof(T), "DeserializeStreamAsync", ex);
            }
        }

    }
}
