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
        /// <inheritdoc/>
        public XmlDocument Serialize<T>(T dataObject, Func<XmlOptionsBuilder, XmlOptionsBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            using (var stringWriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringWriter, dataObject);

                var dataSerialize = stringWriter.ToString();

                if (string.IsNullOrEmpty(dataSerialize)) return new XmlDocument();

                var xmldoc = new XmlDocument();
                xmldoc.LoadXml(dataSerialize);

                return xmldoc.Builder(builder);
            }
        }

        /// <inheritdoc/>
        public XmlDocument Serialize<T>(T dataObject)
        {
            return Serialize(dataObject, b => b
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
                return new T();

            using (var stringReader = new StringReader(dataxml))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader)!;
            }
        }

        /// <inheritdoc/>
        public T Deserialize<T>(XmlDocument xmlDoc) where T : new()
        {
            if (xmlDoc == null)
                return new T();

            var data = xmlDoc.ConvertToString();

            using (var stringReader = new StringReader(data))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader)!;
            }
        }
    }
}
