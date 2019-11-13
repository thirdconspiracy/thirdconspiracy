using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using thirdconspiracy.Utilities.Models;

namespace thirdconspiracy.Utilities.Extensions
{
    public static class SerializationExtensions
    {

        public static string AsString<TBody>(this TBody payload)
        {
            //Empty (aka GET calls)
            if (payload == null)
            {
                return null;
            }

            //string
            var s = payload as string;
            if (s != null)
            {
                return s;
            }

            //Linq XElement
            var xel = payload as XContainer;
            if (xel != null)
            {
                return xel.ToString();
            }

            //POCO
            return SerializeObjectJson(payload);
        }

        #region XML
        public static string SerializeObject<T>(this T obj)
        {
            return SerializeObject(obj, true);
        }

        public static string SerializeObject<T>(this T obj, bool indented)
        {
            var utf8NoBom = new UTF8Encoding(false);
            using (var ms = new MemoryStream())
            {
                using (var xmlWriter = new XmlTextWriter(ms, utf8NoBom)
                {
                    Formatting = indented
                        ? System.Xml.Formatting.Indented
                        : System.Xml.Formatting.None
                })
                {
                    var xmlSer = new XmlSerializer(typeof(T));
                    xmlSer.Serialize(xmlWriter, obj);
                    return utf8NoBom.GetString(ms.ToArray());
                }
            }
        }

        public static string SerializeObject<T>(this T obj, bool indented, bool omitDeclaration, XmlSerializerNamespaces ns)
        {
            var utf8NoBom = new UTF8Encoding(false);
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = omitDeclaration,
                Indent = indented,
                Encoding = utf8NoBom
            };
            using (var ms = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(ms, settings))
                {
                    var xmlSer = new XmlSerializer(typeof(T));
                    xmlSer.Serialize(xmlWriter, obj, ns);
                    return utf8NoBom.GetString(ms.ToArray());
                }
            }
        }

        public static string ToXmlWithoutNamespaces<T>(this T payload)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var body = payload.SerializeObject(false, true, ns);
            var doc = XDocument.Parse(body);

            doc.Descendants().Attributes().Where(a => a.IsNamespaceDeclaration).Remove();
            foreach (var element in doc.Descendants())
            {
                element.Name = element.Name.LocalName;
            }

            return doc.ToString(SaveOptions.DisableFormatting);
        }

        public static T DeserializeObject<T>(this Stream stream)
        {
            var xmlSer = new XmlSerializer(typeof(T));
            return (T)xmlSer.Deserialize(stream);
        }

        public static T DeserializeObject<T>(this XElement xElement)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(xElement.CreateReader());
        }

        public static T DeserializeObject<T>(this string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return default(T);
            }

            // If we were asked to return a string, then just return the whole XML stream as a string
            if (typeof(T) == typeof(string))
            {
                return (T)(object)xml;
            }

            var serializerObj = new XmlSerializer(typeof(T));
            using (var stringReader = new StringReader(xml))
            {
                var result = (T)serializerObj.Deserialize(stringReader);
                return result;
            }
        }

        #endregion XML

        #region JSON

        public static string SerializeObjectJson(this object obj)
        {
            return obj.SerializeObjectJson(true);
        }

        public static string SerializeObjectJson(this object obj, bool indented)
        {
            return obj.SerializeObjectJson(new JsonObjectSerializationOptions() { IndentOutput = indented });
        }

        public static string SerializeObjectJson(this object obj, JsonObjectSerializationOptions options)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = options.SerializeNullProperties
                    ? NullValueHandling.Include
                    : NullValueHandling.Ignore,
                Formatting = options.IndentOutput
                    ? Newtonsoft.Json.Formatting.Indented
                    : Newtonsoft.Json.Formatting.None
            };

            if (options.SerializeEnumsAsStrings)
            {
                settings.Converters.Add(new StringEnumConverter());
            }
            if (options.UseIso8601DateFormatting)
            {
                settings.Converters.Add(new IsoDateTimeConverter());
            }

            return JsonConvert.SerializeObject(obj, settings);
        }

        public static void SerializeObjectJson(this object obj, StreamWriter writer, JsonObjectSerializationOptions options)
        {
            using (JsonWriter jsonWriter = new JsonTextWriter(writer))
            {
                var serializer = new JsonSerializer
                {
                    NullValueHandling = options.SerializeNullProperties
                        ? NullValueHandling.Include
                        : NullValueHandling.Ignore,
                    Formatting = options.IndentOutput
                        ? Newtonsoft.Json.Formatting.Indented
                        : Newtonsoft.Json.Formatting.None
                };

                if (options.SerializeEnumsAsStrings)
                {
                    serializer.Converters.Add(new StringEnumConverter());
                }
                if (options.UseIso8601DateFormatting)
                {
                    serializer.Converters.Add(new IsoDateTimeConverter());
                }
                serializer.Serialize(jsonWriter, obj);
            }
        }

        public static T DeserializeObjectJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T DeserializeObjectJson<T>(this StreamReader reader)
        {
            using (JsonReader jsonReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(jsonReader);
            }
        }

        public static IEnumerable<T> StreamDeserializeArray<T>(this string json)
        {
            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                var jsonSerializer = JsonSerializer.Create();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        T deserializedItem = jsonSerializer.Deserialize<T>(reader);
                        yield return deserializedItem;
                    }
                }
            }
        }

        public static T DeserializeSafeObjectJson<T>(this string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static T DeserializeObjectJson<T>(this MemoryStream stream)
        {
            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        public static MemoryStream SerializeObjectJsonStream<T>(this T data)
        {
            var ret = new MemoryStream();
            var serializer = new JsonSerializer();
            using (var sw = new StreamWriter(ret))
            using (var jsonTextWriter = new JsonTextWriter(sw))
            {
                serializer.Serialize(jsonTextWriter, data);
            }
            return ret;
        }

        #endregion JSON

    }
}
