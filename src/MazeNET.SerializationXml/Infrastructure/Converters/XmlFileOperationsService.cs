using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using MazeNET.SerializationXml.Core.Interfaces;
using MazeNET.SerializationXml.Infrastructure.Extensions;

namespace MazeNET.SerializationXml.Infrastructure.Converters
{
    /// <summary>
    /// Implementation of XML file operations
    /// </summary>
    public class XmlFileOperationsService : IXmlFileOperations
    {
#if NET9_0_OR_GREATER
        /// <inheritdoc/>
        public bool SaveToFile<T>(string fullPath, XmlDocument document)
        {
            ArgumentNullException.ThrowIfNull(fullPath);
            ArgumentNullException.ThrowIfNull(document);

            using (TextWriter writeFileStream = new StreamWriter(fullPath))
            {
                XmlSerializer serializerObj = new XmlSerializer(typeof(T));
                serializerObj.Serialize(writeFileStream, document.ConvertToString());
                writeFileStream.Dispose();

                return true;
            }
        }

        /// <inheritdoc/>
        public bool SaveToFile<T>(string fullPath, T objectToSerialize)
        {
            ArgumentNullException.ThrowIfNull(fullPath);
            ArgumentNullException.ThrowIfNull(objectToSerialize);

            using (TextWriter writeFileStream = new StreamWriter(fullPath))
            {
                XmlSerializer serializerObj = new XmlSerializer(typeof(T));
                serializerObj.Serialize(writeFileStream, objectToSerialize);
                writeFileStream.Close();
                return true;
            }
        }

        /// <inheritdoc/>
        public T LoadFromFile<T>(string fullPath)
        {
            ArgumentNullException.ThrowIfNull(fullPath);

            FileStream? readFileStream = null;
            try
            {
                using (readFileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    T loadedObj = default(T)!;
                    XmlSerializer serializerObj = new XmlSerializer(typeof(T));

                    byte[] buffer = ReadByteArrayFormStream(readFileStream);

                    readFileStream?.Close();

                    Stream stream = new MemoryStream(buffer);

                    loadedObj = (T)serializerObj.Deserialize(stream)!;

                    return loadedObj;
                }
            }
            finally
            {
                readFileStream?.Close();
            }
        }

        /// <inheritdoc/>
        public XmlDocument LoadXml(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            FileStream? readFileStream = null;
            try
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException("File not found: " + path);

                using (readFileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(readFileStream);

                    return xmlDoc;
                }
            }
            finally
            {
                readFileStream?.Close();
            }
        }
#else
        /// <inheritdoc/>
        public bool SaveToFile<T>(string fullPath, XmlDocument document)
        {
            try
            {
                using (TextWriter writeFileStream = new StreamWriter(fullPath))
                {
                    XmlSerializer serializerObj = new XmlSerializer(typeof(T));
                    serializerObj.Serialize(writeFileStream, document.ConvertToString());
                    writeFileStream.Dispose();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <inheritdoc/>
        public bool SaveToFile<T>(string fullPath, T objectToSerialize)
        {
            try
            {
                using (TextWriter writeFileStream = new StreamWriter(fullPath))
                {
                    XmlSerializer serializerObj = new XmlSerializer(typeof(T));
                    serializerObj.Serialize(writeFileStream, objectToSerialize);
                    writeFileStream.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <inheritdoc/>
        public T LoadFromFile<T>(string fullPath)
        {
            FileStream readFileStream = null;
            try
            {
                using (readFileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    T loadedObj = default(T);
                    XmlSerializer serializerObj = new XmlSerializer(typeof(T));

                    byte[] buffer = ReadByteArrayFormStream(readFileStream);

                    readFileStream?.Close();

                    Stream stream = new MemoryStream(buffer);

                    loadedObj = (T)serializerObj.Deserialize(stream);

                    return loadedObj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                readFileStream?.Close();
            }
        }

        /// <inheritdoc/>
        public XmlDocument LoadXml(string path)
        {
            FileStream readFileStream = null;
            try
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException("File not found: " + path);

                using (readFileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(readFileStream);

                    return xmlDoc;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                readFileStream?.Close();
            }
        }
#endif

        private static byte[] ReadByteArrayFormStream(Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}

