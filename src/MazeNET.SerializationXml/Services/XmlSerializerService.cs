using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MazeNET.SerializationXml.Abstractions;
using MazeNET.SerializationXml.Exceptions;
using MazeNET.SerializationXml.Internal;
using MazeNET.SerializationXml.Options;

namespace MazeNET.SerializationXml.Services
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

            try
            {
                using (var stringWriter = new StringWriter())
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stringWriter, dataObject);

                    var dataSerialize = stringWriter.ToString();

                    if (string.IsNullOrEmpty(dataSerialize)) return SafeXmlFactory.CreateDocument();

                    var xmldoc = SafeXmlFactory.CreateDocument();
                    xmldoc.LoadXml(dataSerialize);

                    return xmldoc.Builder(builder);
                }
            }
            catch (Exception ex) when (ex is not ArgumentNullException)
            {
                throw new XmlSerializationException(
                    $"Failed to serialize object of type '{typeof(T).FullName}'.",
                    typeof(T), "Serialize", ex);
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

            try
            {
                using (var stringReader = new StringReader(dataxml))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader)!;
                }
            }
            catch (Exception ex)
            {
                throw new XmlSerializationException(
                    $"Failed to deserialize XML string to type '{typeof(T).FullName}'.",
                    typeof(T), "Deserialize", ex);
            }
        }

        /// <inheritdoc/>
        public T Deserialize<T>(XmlDocument xmlDoc) where T : new()
        {
            if (xmlDoc == null)
                return new T();

            try
            {
                var data = xmlDoc.ConvertToString();

                using (var stringReader = new StringReader(data))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader)!;
                }
            }
            catch (Exception ex)
            {
                throw new XmlSerializationException(
                    $"Failed to deserialize XmlDocument to type '{typeof(T).FullName}'.",
                    typeof(T), "Deserialize", ex);
            }
        }

        /// <inheritdoc/>
        public Task<XmlDocument> SerializeAsync<T>(T dataObject, Func<XmlOptionsBuilder, XmlOptionsBuilder> builder, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Serialize(dataObject, builder));
        }

        /// <inheritdoc/>
        public Task<XmlDocument> SerializeAsync<T>(T dataObject, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Serialize(dataObject));
        }

        /// <inheritdoc/>
        public Task<T> DeserializeAsync<T>(string dataxml, CancellationToken cancellationToken = default) where T : new()
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Deserialize<T>(dataxml));
        }

        /// <inheritdoc/>
        public Task<T> DeserializeAsync<T>(XmlDocument xmlDoc, CancellationToken cancellationToken = default) where T : new()
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Deserialize<T>(xmlDoc));
        }
    }
}
