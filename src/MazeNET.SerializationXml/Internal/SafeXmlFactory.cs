using System.Xml;

namespace MazeNET.SerializationXml.Internal
{
    internal static class SafeXmlFactory
    {
        internal static XmlDocument CreateDocument()
        {
            return new XmlDocument { XmlResolver = null };
        }
    }
}
