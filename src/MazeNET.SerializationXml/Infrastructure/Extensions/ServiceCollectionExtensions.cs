using MazeNET.SerializationXml.Core.Interfaces;
using MazeNET.SerializationXml.Infrastructure.Converters;
using Microsoft.Extensions.DependencyInjection;

namespace MazeNET.SerializationXml
{
    /// <summary>
    /// Extension methods for IServiceCollection to register XML serialization services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers IXmlSerializer, IXmlFileOperations, and IXmlToJsonConverter as singleton services
        /// </summary>
        public static IServiceCollection AddSerializationXml(this IServiceCollection services)
        {
            services.AddSingleton<IXmlSerializer, XmlSerializerService>();
            services.AddSingleton<IXmlFileOperations, XmlFileOperationsService>();
            services.AddSingleton<IXmlToJsonConverter, XmlToJsonConverterService>();
            return services;
        }
    }
}
