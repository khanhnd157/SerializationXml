using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using MazeNET.SerializationXml.Options;

namespace MazeNET.SerializationXml.Internal
{
    /// <summary>
    /// Internal helper to convert XML DOM to JSON string without external dependencies
    /// </summary>
    internal sealed class XmlJsonWriter
    {
        private readonly StringBuilder _sb;
        private readonly XmlToJsonOptions _options;
        private readonly string _indent;
        private int _depth;

        internal XmlJsonWriter(XmlToJsonOptions options)
        {
            _options = options;
            _sb = new StringBuilder();
            _indent = options.Indent ? "  " : string.Empty;
        }

        internal string Write(XmlDocument document)
        {
            _sb.Clear();
            _depth = 0;

            if (_options.OmitRootObject)
            {
                var root = document.DocumentElement;
                if (root == null)
                {
                    WriteRaw("{}");
                    return _sb.ToString();
                }
                WriteElementContent(root);
            }
            else
            {
                WriteStartObject();
                WriteDocumentChildren(document);
                WriteEndObject();
            }

            return _sb.ToString();
        }

        private void WriteDocumentChildren(XmlDocument document)
        {
            var first = true;
            foreach (XmlNode child in document.ChildNodes)
            {
                if (child is XmlDeclaration && _options.OmitXmlDeclaration)
                    continue;

                if (child is XmlComment)
                    continue;

                if (!first) WriteComma();
                first = false;

                if (child is XmlDeclaration decl)
                {
                    WritePropertyName("?xml");
                    WriteStartObject();
                    var declFirst = true;
                    if (!string.IsNullOrEmpty(decl.Version))
                    {
                        WritePropertyName(_options.AttributePrefix + "version");
                        WriteStringValue(decl.Version);
                        declFirst = false;
                    }
                    if (!string.IsNullOrEmpty(decl.Encoding))
                    {
                        if (!declFirst) WriteComma();
                        WritePropertyName(_options.AttributePrefix + "encoding");
                        WriteStringValue(decl.Encoding);
                        declFirst = false;
                    }
                    if (!string.IsNullOrEmpty(decl.Standalone))
                    {
                        if (!declFirst) WriteComma();
                        WritePropertyName(_options.AttributePrefix + "standalone");
                        WriteStringValue(decl.Standalone);
                    }
                    WriteEndObject();
                }
                else if (child is XmlElement element)
                {
                    WritePropertyName(element.LocalName);
                    WriteElement(element);
                }
            }
        }

        private void WriteElement(XmlElement element)
        {
            var hasAttributes = HasRelevantAttributes(element);
            var childElements = GetChildElements(element);
            var textContent = GetDirectTextContent(element);
            var cdataContent = GetDirectCDataContent(element);

            if (!hasAttributes && childElements.Count == 0)
            {
                if (textContent != null)
                {
                    WriteStringValue(textContent);
                }
                else if (cdataContent != null)
                {
                    WriteStringValue(cdataContent);
                }
                else
                {
                    WriteNull();
                }
                return;
            }

            WriteStartObject();
            var first = true;

            if (hasAttributes)
            {
                foreach (XmlAttribute attr in element.Attributes)
                {
                    if (!_options.IncludeNamespaces && IsNamespaceAttribute(attr))
                        continue;

                    if (!first) WriteComma();
                    first = false;
                    WritePropertyName(_options.AttributePrefix + attr.LocalName);
                    WriteStringValue(attr.Value);
                }
            }

            if (childElements.Count > 0)
            {
                var grouped = GroupChildElements(childElements);

                foreach (var group in grouped)
                {
                    if (!first) WriteComma();
                    first = false;
                    WritePropertyName(group.Key);

                    if (group.Value.Count == 1)
                    {
                        WriteElement(group.Value[0]);
                    }
                    else
                    {
                        WriteStartArray();
                        for (int i = 0; i < group.Value.Count; i++)
                        {
                            if (i > 0) WriteComma();
                            WriteElement(group.Value[i]);
                        }
                        WriteEndArray();
                    }
                }

                if (textContent != null)
                {
                    if (!first) WriteComma();
                    first = false;
                    WritePropertyName(_options.TextNodeKey);
                    WriteStringValue(textContent);
                }

                if (cdataContent != null)
                {
                    if (!first) WriteComma();
                    WritePropertyName(_options.CDataNodeKey);
                    WriteStringValue(cdataContent);
                }
            }
            else
            {
                if (textContent != null)
                {
                    if (!first) WriteComma();
                    first = false;
                    WritePropertyName(_options.TextNodeKey);
                    WriteStringValue(textContent);
                }

                if (cdataContent != null)
                {
                    if (!first) WriteComma();
                    WritePropertyName(_options.CDataNodeKey);
                    WriteStringValue(cdataContent);
                }
            }

            WriteEndObject();
        }

        private void WriteElementContent(XmlElement element)
        {
            var hasAttributes = HasRelevantAttributes(element);
            var childElements = GetChildElements(element);
            var textContent = GetDirectTextContent(element);
            var cdataContent = GetDirectCDataContent(element);

            if (!hasAttributes && childElements.Count == 0 && textContent == null && cdataContent == null)
            {
                WriteRaw("{}");
                return;
            }

            WriteStartObject();
            var first = true;

            if (hasAttributes)
            {
                foreach (XmlAttribute attr in element.Attributes)
                {
                    if (!_options.IncludeNamespaces && IsNamespaceAttribute(attr))
                        continue;

                    if (!first) WriteComma();
                    first = false;
                    WritePropertyName(_options.AttributePrefix + attr.LocalName);
                    WriteStringValue(attr.Value);
                }
            }

            var grouped = GroupChildElements(childElements);

            foreach (var group in grouped)
            {
                if (!first) WriteComma();
                first = false;
                WritePropertyName(group.Key);

                if (group.Value.Count == 1)
                {
                    WriteElement(group.Value[0]);
                }
                else
                {
                    WriteStartArray();
                    for (int i = 0; i < group.Value.Count; i++)
                    {
                        if (i > 0) WriteComma();
                        WriteElement(group.Value[i]);
                    }
                    WriteEndArray();
                }
            }

            if (textContent != null)
            {
                if (!first) WriteComma();
                first = false;
                WritePropertyName(_options.TextNodeKey);
                WriteStringValue(textContent);
            }

            if (cdataContent != null)
            {
                if (!first) WriteComma();
                WritePropertyName(_options.CDataNodeKey);
                WriteStringValue(cdataContent);
            }

            WriteEndObject();
        }

        private bool HasRelevantAttributes(XmlElement element)
        {
            if (element.Attributes == null || element.Attributes.Count == 0)
                return false;

            if (_options.IncludeNamespaces)
                return true;

            foreach (XmlAttribute attr in element.Attributes)
            {
                if (!IsNamespaceAttribute(attr))
                    return true;
            }

            return false;
        }

        private static bool IsNamespaceAttribute(XmlAttribute attr)
        {
            return attr.Prefix == "xmlns" || attr.Name == "xmlns";
        }

        private static List<XmlElement> GetChildElements(XmlElement element)
        {
            var list = new List<XmlElement>();
            foreach (XmlNode child in element.ChildNodes)
            {
                if (child is XmlElement el)
                    list.Add(el);
            }
            return list;
        }

        private static string? GetDirectTextContent(XmlElement element)
        {
            StringBuilder? sb = null;
            foreach (XmlNode child in element.ChildNodes)
            {
                if (child is XmlText text)
                {
                    if (sb == null) sb = new StringBuilder();
                    sb.Append(text.Value);
                }
            }
            return sb?.ToString();
        }

        private static string? GetDirectCDataContent(XmlElement element)
        {
            StringBuilder? sb = null;
            foreach (XmlNode child in element.ChildNodes)
            {
                if (child is XmlCDataSection cdata)
                {
                    if (sb == null) sb = new StringBuilder();
                    sb.Append(cdata.Value);
                }
            }
            return sb?.ToString();
        }

        private static List<KeyValuePair<string, List<XmlElement>>> GroupChildElements(List<XmlElement> elements)
        {
            var result = new List<KeyValuePair<string, List<XmlElement>>>();
            var index = new Dictionary<string, int>();

            foreach (var el in elements)
            {
                var name = el.LocalName;
                if (index.TryGetValue(name, out int idx))
                {
                    result[idx].Value.Add(el);
                }
                else
                {
                    index[name] = result.Count;
                    result.Add(new KeyValuePair<string, List<XmlElement>>(name, new List<XmlElement> { el }));
                }
            }

            return result;
        }

        private void WriteStartObject()
        {
            _sb.Append('{');
            _depth++;
        }

        private void WriteEndObject()
        {
            _depth--;
            WriteNewLine();
            _sb.Append('}');
        }

        private void WriteStartArray()
        {
            _sb.Append('[');
            _depth++;
        }

        private void WriteEndArray()
        {
            _depth--;
            WriteNewLine();
            _sb.Append(']');
        }

        private void WritePropertyName(string name)
        {
            WriteNewLine();
            _sb.Append('"');
            WriteEscapedString(name);
            _sb.Append('"');
            _sb.Append(':');
            if (_options.Indent) _sb.Append(' ');
        }

        private void WriteStringValue(string value)
        {
            _sb.Append('"');
            WriteEscapedString(value);
            _sb.Append('"');
        }

        private void WriteNull()
        {
            _sb.Append("null");
        }

        private void WriteComma()
        {
            _sb.Append(',');
        }

        private void WriteRaw(string value)
        {
            _sb.Append(value);
        }

        private void WriteNewLine()
        {
            if (!_options.Indent) return;
            _sb.Append('\n');
            for (int i = 0; i < _depth; i++)
                _sb.Append(_indent);
        }

        private void WriteEscapedString(string value)
        {
            foreach (var c in value)
            {
                switch (c)
                {
                    case '"': _sb.Append("\\\""); break;
                    case '\\': _sb.Append("\\\\"); break;
                    case '\b': _sb.Append("\\b"); break;
                    case '\f': _sb.Append("\\f"); break;
                    case '\n': _sb.Append("\\n"); break;
                    case '\r': _sb.Append("\\r"); break;
                    case '\t': _sb.Append("\\t"); break;
                    default:
                        if (c < ' ')
                        {
                            _sb.Append("\\u");
                            _sb.Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            _sb.Append(c);
                        }
                        break;
                }
            }
        }
    }
}
