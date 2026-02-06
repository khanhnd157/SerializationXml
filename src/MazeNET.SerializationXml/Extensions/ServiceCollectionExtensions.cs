using MazeNET.SerializationXml.Abstractions;
using MazeNET.SerializationXml.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MazeNET.SerializationXml
{
    /// <summary>
    /// Extension methods for IServiceCollection to register XML serialization services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all XML serialization services as singletons
        /// </summary>
        public static IServiceCollection AddSerializationXml(this IServiceCollection services)
        {
            services.AddSingleton<IXmlSerializer, XmlSerializerService>();
            services.AddSingleton<IXmlFileOperations, XmlFileOperationsService>();
            services.AddSingleton<IXmlToJsonConverter, XmlToJsonConverterService>();
            services.AddSingleton<IXmlToObjectMapper, XmlToObjectMapperService>();
            services.AddSingleton<IXmlTypedJsonConverter, XmlTypedJsonConverterService>();
            return services;
        }
    }
}
