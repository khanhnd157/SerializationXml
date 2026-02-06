using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace MazeNET.SerializationXml.Internal
{
    internal sealed class ObjectToJsonWriter
    {
        private const int MaxDepth = 64;
        private readonly StringBuilder _sb;
        private readonly bool _indent;
        private readonly string _indentChars;
        private int _depth;

        internal ObjectToJsonWriter(bool indent = true)
        {
            _sb = new StringBuilder();
            _indent = indent;
            _indentChars = indent ? "  " : string.Empty;
        }

        internal string Write(object? obj)
        {
            _sb.Clear();
            _depth = 0;
            WriteValue(obj);
            return _sb.ToString();
        }

        private void WriteValue(object? value)
        {
            if (value == null)
            {
                _sb.Append("null");
                return;
            }

            var type = value.GetType();
            var underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null) type = underlying;

            if (type == typeof(string))
            {
                WriteString((string)value);
            }
            else if (type == typeof(bool))
            {
                _sb.Append((bool)value ? "true" : "false");
            }
            else if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte))
            {
                _sb.Append(System.Convert.ToInt64(value).ToString(CultureInfo.InvariantCulture));
            }
            else if (type == typeof(double))
            {
                _sb.Append(((double)value).ToString("G17", CultureInfo.InvariantCulture));
            }
            else if (type == typeof(float))
            {
                _sb.Append(((float)value).ToString("G9", CultureInfo.InvariantCulture));
            }
            else if (type == typeof(decimal))
            {
                _sb.Append(((decimal)value).ToString(CultureInfo.InvariantCulture));
            }
            else if (type == typeof(DateTime))
            {
                WriteString(((DateTime)value).ToString("o", CultureInfo.InvariantCulture));
            }
            else if (type == typeof(DateTimeOffset))
            {
                WriteString(((DateTimeOffset)value).ToString("o", CultureInfo.InvariantCulture));
            }
            else if (type == typeof(Guid))
            {
                WriteString(((Guid)value).ToString());
            }
            else if (type == typeof(TimeSpan))
            {
                WriteString(((TimeSpan)value).ToString("c"));
            }
            else if (type.IsEnum)
            {
                WriteString(value.ToString()!);
            }
            else if (value is IEnumerable enumerable && type != typeof(string))
            {
                WriteArray(enumerable);
            }
            else if (type.IsClass || (type.IsValueType && !type.IsPrimitive))
            {
                WriteObject(value, type);
            }
            else
            {
                WriteString(value.ToString() ?? string.Empty);
            }
        }

        private void WriteObject(object obj, Type type)
        {
            if (_depth >= MaxDepth)
                throw new InvalidOperationException($"Max serialization depth ({MaxDepth}) exceeded. Possible circular reference in type '{type.FullName}'.");

            _sb.Append('{');
            _depth++;

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var first = true;

            foreach (var prop in properties)
            {
                if (!prop.CanRead) continue;

                var propValue = prop.GetValue(obj);

                if (!first) _sb.Append(',');
                first = false;

                WriteNewLine();
                WriteString(prop.Name);
                _sb.Append(':');
                if (_indent) _sb.Append(' ');

                WriteValue(propValue);
            }

            _depth--;
            WriteNewLine();
            _sb.Append('}');
        }

        private void WriteArray(IEnumerable items)
        {
            if (_depth >= MaxDepth)
                throw new InvalidOperationException($"Max serialization depth ({MaxDepth}) exceeded. Possible circular reference.");

            _sb.Append('[');
            _depth++;

            var first = true;
            foreach (var item in items)
            {
                if (!first) _sb.Append(',');
                first = false;

                WriteNewLine();
                WriteValue(item);
            }

            _depth--;
            WriteNewLine();
            _sb.Append(']');
        }

        private void WriteString(string value)
        {
            _sb.Append('"');
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
            _sb.Append('"');
        }

        private void WriteNewLine()
        {
            if (!_indent) return;
            _sb.Append('\n');
            for (int i = 0; i < _depth; i++)
                _sb.Append(_indentChars);
        }
    }
}
