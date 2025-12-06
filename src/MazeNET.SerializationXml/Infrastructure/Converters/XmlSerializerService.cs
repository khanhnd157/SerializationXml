using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MazeNET.SerializationXml.Core.Interfaces;
using MazeNET.SerializationXml.Core.Options;
using MazeNET.SerializationXml.Infrastructure.Extensions;

namespace MazeNET.SerializationXml.Infrastructure.Converters
{
    /// <summary>
    /// Implementation of XML serializer
    /// </summary>
    public class XmlSerializerService : IXmlSerializer
    {
#if NET9_0_OR_GREATER
        /// <inheritdoc/>
        public XmlDocument Serialize<T>(T dataObject, Func<XmlOptionsBuilder, XmlOptionsBuilder> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            dataObject ??= default(T)!;

            using (StringWriter stringWriter = new StringWriter())
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

        /// <inheritdoc/>
        public XmlDocument Serialize<T>(T dataObject)
        {
            return Serialize<T>(dataObject, builder => builder
                .AddDeclaration(new XmlDeclarationOptions
                {
                    Encoding = Encoding.UTF8,
                    Standalone = true,
                    Version = "1.0"
                })
                .RemoveSchema());
        }

        /// <inheritdoc/>
        public T Deserialize<T>(string dataxml) where T : new()
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
                    return (T)serializer.Deserialize(stringReader)!;
                }
            }
            catch
            {
                return new T();
            }
        }

        /// <inheritdoc/>
        public T Deserialize<T>(XmlDocument? xmlDoc) where T : new()
        {
            try
            {
                if (xmlDoc == null)
                    return default(T)!;

                var data = xmlDoc.ConvertToString();

                using (var stringReader = new StringReader(data))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader)!;
                }
            }
            catch
            {
                return new T();
            }
        }
#else
        /// <inheritdoc/>
        public XmlDocument Serialize<T>(T dataObject, Func<XmlOptionsBuilder, XmlOptionsBuilder> builder)
        {
            if (dataObject == null) dataObject = default(T);

            try
            {
                using (StringWriter stringWriter = new StringWriter())
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

        /// <inheritdoc/>
        public XmlDocument Serialize<T>(T dataObject)
        {
            return Serialize<T>(dataObject, builder => builder
                .AddDeclaration(new XmlDeclarationOptions
                {
                    Encoding = Encoding.UTF8,
                    Standalone = true,
                    Version = "1.0"
                })
                .RemoveSchema());
        }

        /// <inheritdoc/>
        public T Deserialize<T>(string dataxml) where T : new()
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

        /// <inheritdoc/>
        public T Deserialize<T>(XmlDocument xmlDoc) where T : new()
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
#endif
    }
}

