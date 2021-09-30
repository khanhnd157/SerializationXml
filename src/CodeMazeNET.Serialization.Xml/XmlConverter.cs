using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CodeMazeNET.Serialization.Xml
{
    public static class XmlConverter
    {
        public static bool SaveToFile<T>(string fullPath, XmlDocument document)
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

        public static bool SaveToFile<T>(string fullPath, T objectToSerialize)
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

        public static T FileToObject<T>(string fullPath)
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

        public static XmlDocument SerializeObject<T>(T dataObject, Func<XmlOptionsBuilder, XmlOptionsBuilder> builder)
        {
            if (dataObject == null) dataObject = default(T);

            try
            {
                using (StringWriter stringWriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stringWriter, dataObject);

                    var dataSerialize = stringWriter?.ToString() ?? string.Empty;

                    if (string.IsNullOrEmpty(dataSerialize)) return new XmlDocument();

                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml(dataSerialize);

                    return xmldoc.Builder(builder);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static XmlDocument SerializeObject<T>(T dataObject)
        {
            return SerializeObject<T>(dataObject, builder => builder
                .AddDeclaration(new XmlDeclarationOptions
                {
                    Encoding = Encoding.UTF8,
                    Standalone = true,
                    Version = "1.0"
                })
                .RemoveSchema());
        }

        public static XmlDocument LoadXml(string path)
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

        public static T DeserializeObject<T>(string dataxml)
             where T : new()
        {
            if (string.IsNullOrEmpty(dataxml))
            {
                return new T();
            }
            try
            {
                using (var stringReader = new StringReader(dataxml))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader);
                }
            }
            catch (Exception ex)
            {
                return new T();
            }
        }

        public static T DeserializeObject<T>(XmlDocument xmlDoc)
            where T : new()
        {
            try
            {
                if (xmlDoc == null)
                    return default(T);

                var data = xmlDoc.ConvertToString();

                using (var stringReader = new StringReader(data))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader);
                }
            }
            catch (Exception ex)
            {
                return new T();
            }
        }

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
