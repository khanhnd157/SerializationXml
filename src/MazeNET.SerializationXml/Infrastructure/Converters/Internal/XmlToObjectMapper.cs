using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace MazeNET.SerializationXml.Infrastructure.Converters.Internal
{
    /// <summary>
    /// Maps XML elements to a typed object using reflection.
    /// Unmatched properties remain null/default.
    /// </summary>
    internal static class XmlToObjectMapper
    {
        internal static T Map<T>(XmlDocument document) where T : new()
        {
            if (document == null || document.DocumentElement == null)
                return new T();

            var result = new T();
            MapElement(document.DocumentElement, result, typeof(T));
            return result;
        }

        internal static T MapFromString<T>(string xml) where T : new()
        {
            if (string.IsNullOrEmpty(xml))
                return new T();

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return Map<T>(doc);
        }

        private static void MapElement(XmlElement element, object target, Type targetType)
        {
            var properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propMap = BuildPropertyMap(properties);

            MapAttributes(element, target, propMap);
            MapChildElements(element, target, propMap);
            MapTextContent(element, target, propMap);
        }

        private static Dictionary<string, PropertyInfo> BuildPropertyMap(PropertyInfo[] properties)
        {
            var map = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in properties)
            {
                if (!prop.CanWrite) continue;
                map[prop.Name] = prop;
            }
            return map;
        }

        private static void MapAttributes(XmlElement element, object target, Dictionary<string, PropertyInfo> propMap)
        {
            if (element.Attributes == null) return;

            foreach (XmlAttribute attr in element.Attributes)
            {
                if (attr.Prefix == "xmlns" || attr.Name == "xmlns") continue;

                if (propMap.TryGetValue(attr.LocalName, out var prop))
                {
                    SetPropertyValue(target, prop, attr.Value);
                }
            }
        }

        private static void MapChildElements(XmlElement element, object target, Dictionary<string, PropertyInfo> propMap)
        {
            var grouped = GroupChildElements(element);

            foreach (var group in grouped)
            {
                if (!propMap.TryGetValue(group.Key, out var prop)) continue;

                var propType = prop.PropertyType;
                var underlyingType = Nullable.GetUnderlyingType(propType);

                if (IsCollectionType(propType))
                {
                    var itemType = GetCollectionItemType(propType);
                    if (itemType == null) continue;

                    var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;

                    foreach (var childEl in group.Value)
                    {
                        var item = ConvertElement(childEl, itemType);
                        if (item != null) list.Add(item);
                    }

                    if (propType.IsArray)
                    {
                        var array = Array.CreateInstance(itemType, list.Count);
                        list.CopyTo(array, 0);
                        prop.SetValue(target, array);
                    }
                    else
                    {
                        prop.SetValue(target, list);
                    }
                }
                else if (group.Value.Count == 1)
                {
                    var childEl = group.Value[0];
                    var actualType = underlyingType ?? propType;

                    if (IsSimpleType(actualType))
                    {
                        var text = GetElementText(childEl);
                        if (text != null)
                        {
                            SetPropertyValue(target, prop, text);
                        }
                    }
                    else if (actualType.IsClass && actualType != typeof(string))
                    {
                        var childObj = Activator.CreateInstance(actualType)!;
                        MapElement(childEl, childObj, actualType);
                        prop.SetValue(target, childObj);
                    }
                }
            }
        }

        private static void MapTextContent(XmlElement element, object target, Dictionary<string, PropertyInfo> propMap)
        {
            if (propMap.TryGetValue("Value", out var valueProp) || propMap.TryGetValue("Text", out valueProp))
            {
                var childElements = element.SelectNodes("*");
                if (childElements != null && childElements.Count > 0) return;

                var text = GetElementText(element);
                if (text != null && valueProp.GetValue(target) == null)
                {
                    SetPropertyValue(target, valueProp, text);
                }
            }
        }

        private static object? ConvertElement(XmlElement element, Type targetType)
        {
            if (IsSimpleType(targetType))
            {
                var text = GetElementText(element);
                return text != null ? ConvertValue(text, targetType) : null;
            }

            if (targetType.IsClass && targetType != typeof(string))
            {
                var obj = Activator.CreateInstance(targetType)!;
                MapElement(element, obj, targetType);
                return obj;
            }

            return null;
        }

        private static string? GetElementText(XmlElement element)
        {
            foreach (XmlNode child in element.ChildNodes)
            {
                if (child is XmlText text)
                    return text.Value;
                if (child is XmlCDataSection cdata)
                    return cdata.Value;
            }
            return null;
        }

        private static void SetPropertyValue(object target, PropertyInfo prop, string value)
        {
            var propType = prop.PropertyType;
            var underlyingType = Nullable.GetUnderlyingType(propType);
            var actualType = underlyingType ?? propType;

            var converted = ConvertValue(value, actualType);
            if (converted != null)
            {
                prop.SetValue(target, converted);
            }
        }

        private static object? ConvertValue(string value, Type targetType)
        {
            if (targetType == typeof(string))
                return value;

            if (string.IsNullOrEmpty(value))
                return null;

            try
            {
                if (targetType == typeof(int)) return int.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(long)) return long.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(short)) return short.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(byte)) return byte.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(double)) return double.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(float)) return float.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(decimal)) return decimal.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(bool)) return bool.Parse(value);
                if (targetType == typeof(DateTime)) return DateTime.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(DateTimeOffset)) return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(Guid)) return Guid.Parse(value);
                if (targetType == typeof(TimeSpan)) return TimeSpan.Parse(value, CultureInfo.InvariantCulture);
                if (targetType.IsEnum) return Enum.Parse(targetType, value, true);

                var converter = TypeDescriptor.GetConverter(targetType);
                if (converter.CanConvertFrom(typeof(string)))
                    return converter.ConvertFromInvariantString(value);
            }
            catch
            {
                return null;
            }

            return null;
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(TimeSpan)
                || type == typeof(Guid);
        }

        private static bool IsCollectionType(Type type)
        {
            if (type == typeof(string)) return false;
            if (type.IsArray) return true;
            return typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType;
        }

        private static Type? GetCollectionItemType(Type type)
        {
            if (type.IsArray) return type.GetElementType();
            if (type.IsGenericType) return type.GetGenericArguments()[0];
            return null;
        }

        private static List<KeyValuePair<string, List<XmlElement>>> GroupChildElements(XmlElement parent)
        {
            var result = new List<KeyValuePair<string, List<XmlElement>>>();
            var index = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (XmlNode child in parent.ChildNodes)
            {
                if (child is not XmlElement el) continue;

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
    }
}
