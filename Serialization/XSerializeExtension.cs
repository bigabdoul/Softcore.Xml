using Softcore.Xml.Serialization.Soap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Softcore.Xml.Serialization
{
    /// <summary>
    /// Provides extension methods for XML-related operations.
    /// </summary>
    public static class XSerializeExtension
    {
        #region Global options

        /// <summary>
        /// Gets or sets a value that indicates whether to throw <see cref="InvalidOperationException"/> when an XML document cannot be deserialized.
        /// </summary>
        public static bool ThrowIfCannotDeserialize { get; set; } = false;

        #endregion

        #region XDeserialize/XTryDeserialize SOAP

        /// <summary>
        /// Deserializes the specified XML string to an instance of <typeparamref name="T"/> 
        /// type using a <see cref="SoapFormatter"/> object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="soapXml">The XML string that represents the type to deserialize to.</param>
        /// <param name="enc">The encoding to use. If null, <see cref="Encoding.UTF8"/> will be used.</param>
        /// <returns>
        /// An initialized instance of <typeparamref name="T"/>, or the type's default value 
        /// (null for reference types) if the XML cannot be deserialized to the specified type.
        /// </returns>
        public static T XSoapDeserialize<T>(this string soapXml, Encoding enc = null)
        {
            if (soapXml == null) return default(T);

            using (var ms = new MemoryStream((enc ?? Encoding.UTF8).GetBytes(soapXml)))
            {
                var formatter = new SoapFormatter();
                return (T)formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Attempts to deserialize the given XML to an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="soapXml">The XML string that represents the type to deserialize to.</param>
        /// <param name="enc">The encoding to use. If null, <see cref="Encoding.UTF8"/> will be used.</param>
        /// <param name="result">
        /// Returns an initialized instance of <typeparamref name="T"/>, or the type's default value 
        /// (null for reference types) if the XML cannot be deserialized to the specified type.
        /// </param>
        /// <returns>true if <paramref name="soapXml"/> has been deserialized; otherwise, false.</returns>
        public static bool XTrySoapDeserialize<T>(this string soapXml, out T result, Encoding enc = null)
        {
            result = default(T);
            try
            {
                result = soapXml.XSoapDeserialize<T>(enc);
            }
            catch
            {
            }
            return !Equals(result, default(T));
        }

        #endregion

        #region XDeserialize overloads

        /// <summary>
        /// Deserializes the XML contained by the specified <paramref name="element"/>
        /// to an instance of the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize to.</typeparam>
        /// <param name="element">The XML element to deserialize.</param>
        /// <param name="throwIfCannotDeserialize">Indicates whether to throw <see cref="InvalidOperationException"/> when an XML document cannot be deserialized.</param>
        /// <returns></returns>
        public static T XDeserialize<T>(this XElement element, bool? throwIfCannotDeserialize = null)
            => (T)XDeserialize(element?.ToString(), typeof(T), throwIfCannotDeserialize);

        /// <summary>
        /// Deserializes the XML document contained by the specified <paramref name="xml"/> 
        /// string to an instance of the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize to.</typeparam>
        /// <param name="xml">A string that contains XML.</param>
        /// <param name="throwIfCannotDeserialize">Indicates whether to throw <see cref="InvalidOperationException"/> when an XML document cannot be deserialized.</param>
        /// <returns>The <see cref="object"/> of type <typeparamref name="T"/> being deserialized.</returns>
        public static T XDeserialize<T>(this string xml, bool? throwIfCannotDeserialize = null) 
            => (T)XDeserialize(xml, typeof(T), throwIfCannotDeserialize);

        /// <summary>
        /// Deserializes the XML document contained by the specified <paramref name="xml"/> string.
        /// </summary>
        /// <param name="xml">A string that contains XML.</param>
        /// <param name="type">The type of object to deserialize to.</param>
        /// <param name="throwIfCannotDeserialize">Indicates whether to throw <see cref="InvalidOperationException"/> when an XML document cannot be deserialized.</param>
        /// <returns>The <see cref="object"/> being deserialized.</returns>
        public static object XDeserialize(this string xml, Type type, bool? throwIfCannotDeserialize = null)
        {
            if (string.IsNullOrWhiteSpace(xml)) return null;

            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true,
                CheckCharacters = true
            };

            return XDeserialize(xml, type, settings, throwIfCannotDeserialize);
        }

        /// <summary>
        /// Deserializes the XML document contained by the specified <paramref name="xml"/> string using the specified settings.
        /// </summary>
        /// <param name="xml">A string that contains XML.</param>
        /// <param name="type">The type of object to deserialize to.</param>
        /// <param name="settings">
        /// The <see cref="XmlReaderSettings"/> object used to configure the new System.Xml.XmlReader. This value can be null.
        /// </param>
        /// <param name="throwIfCannotDeserialize">Indicates whether to throw <see cref="InvalidOperationException"/> when an XML document cannot be deserialized.</param>
        /// <returns></returns>
        public static object XDeserialize(this string xml, Type type, XmlReaderSettings settings, bool? throwIfCannotDeserialize = null)
        {
            var xser = new XmlSerializer(type);

            using (var sr = new StringReader(xml))
            {
                using (var xr = XmlReader.Create(sr, settings))
                {
                    if (xser.CanDeserialize(xr))
                    {
                        var xde = new XmlDeserializationEvents
                        {
                            OnUnreferencedObject = (s, e) =>
                            {
                                System.Diagnostics.Debug.WriteLine("XDeserialize: Unreferenced object: {0}", e.UnreferencedObject);
                            }
                        };
                        return xser.Deserialize(xr, xde);
                    }

                    return ThrowCannotDeserialize(type, throwIfCannotDeserialize);
                }
            }
        }

        #endregion

        #region XSerialize overloads

        /// <summary>
        /// Serializes the specified object and returns the XML document as a string 
        /// using the specified dictionary of namespaces.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="namespaces">A dictionary of namespace names and corresponding values.</param>
        /// <param name="enc">The encoding to use. Defaults to <see cref="Encoding.UTF8"/> if null.</param>
        /// <returns>An XML string that represents the serialized object.</returns>
        public static string XSerialize(this object obj, IDictionary<string, string> namespaces, Encoding enc = null)
        {
            var names = new List<XmlQualifiedName>();

            foreach (var k in namespaces.Keys)
            {
                names.Add(new XmlQualifiedName(k, namespaces[k]));
            }

            return obj.XSerialize(new XmlSerializerNamespaces(names.ToArray()), enc);
        }

        /// <summary>
        /// Serializes the specified object and returns the XML document as a string 
        /// using the specified array of <see cref="XmlQualifiedName"/> objects.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="enc">The encoding to use. Defaults to <see cref="Encoding.UTF8"/> if null.</param>
        /// <param name="namespaces">An array of <see cref="XmlQualifiedName"/> objects.</param>
        /// <returns>An XML string that represents the serialized object.</returns>
        public static string XSerialize(this object obj, Encoding enc = null, params XmlQualifiedName[] namespaces)
        {
            var ns = (namespaces == null || namespaces.Length == 0) ? null : new XmlSerializerNamespaces(namespaces);
            return obj.XSerialize(ns, enc);
        }

        /// <summary>
        /// Serializes the specified object and returns the XML document as a string.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="namespaces">The <see cref="XmlSerializerNamespaces"/> referenced by the object.</param>
        /// <param name="enc">The encoding to use. Defaults to <see cref="Encoding.UTF8"/> if null.</param>
        /// <returns>An XML string that represents the serialized object.</returns>
        public static string XSerialize(this object obj, XmlSerializerNamespaces namespaces = null, Encoding enc = null)
        {
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                enc = enc ?? Encoding.UTF8;

                using (var xtw = XmlWriter.Create(sw, new XmlWriterSettings { Encoding = enc }))
                {
                    var xser = new XmlSerializer(obj.GetType());

                    if (namespaces == null)
                        xser.Serialize(xtw, obj);
                    else
                        xser.Serialize(xtw, obj, namespaces);
                }
            }

            return sb.ToString().XStripNullableEmptyElements();
        }
        #endregion

        #region XSerializeFragment overloads

        /// <summary>
        /// Serializes the current object as an XML fragment, that is one with no declarations, using the specified namespaces.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="namespaces">A one-dimensional array of <see cref="XmlQualifiedName"/> objects to use. Can be null.</param>
        /// <returns></returns>
        public static string XSerializeFragment(this object obj, params XmlQualifiedName[] namespaces)
        {
            Encoding enc = null;
            return obj.XSerializeFragment(enc, namespaces);
        }

        /// <summary>
        /// Serializes the current object as an XML fragment, that is one with no declarations, using the specified namespaces.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="enc">The encoding to use. Defaults to <see cref="Encoding.UTF8"/> if null.</param>
        /// <param name="namespaces">A one-dimensional array of <see cref="XmlQualifiedName"/> objects to use. Can be null.</param>
        /// <returns></returns>
        public static string XSerializeFragment(this object obj, Encoding enc, params XmlQualifiedName[] namespaces)
        {
            var ns = (namespaces == null || namespaces.Length == 0) ? null : new XmlSerializerNamespaces(namespaces);
            return obj.XSerializeFragment(ns, enc);
        }

        /// <summary>
        /// Serializes the current object as an XML fragment, that is one with no declarations, using the specified namespaces.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="ns">The namespaces referenced by the object to serialize. Can be null.</param>
        /// <param name="enc">The encoding to use. Defaults to <see cref="Encoding.UTF8"/> if null.</param>
        /// <returns></returns>
        public static string XSerializeFragment(this object obj, XmlSerializerNamespaces ns = null, Encoding enc = null)
        {
            if (obj == null) return null;

            // create the serializer at an early stage; if the object cannot be serialized an exception is thrown right away
            var serializer = new XmlSerializer(obj.GetType());

            if (ns == null)
            {
                ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);
            }

            enc = enc ?? Encoding.UTF8;

            using (var ms = new MemoryStream())
            {
                var settings = new XmlWriterSettings
                {
                    Encoding = enc,
                    OmitXmlDeclaration = true,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates,
                    CheckCharacters = true
                };

                serializer.Serialize(XmlWriter.Create(ms, settings), obj, ns);

                return enc.GetString(ms.ToArray());
            }
        }
        #endregion

        #region Miscellaneous

        /// <summary>
        /// Converts the specified object to a new instance of the <see cref="SoapEnvelope"/> class using custom attributes like <see cref="SoapHeaderAttribute"/> and <see cref="SoapBodyAttribute"/>.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <param name="inherit">true to inspect the ancestors of <paramref name="obj"/>; otherwise, false.</param>
        /// <returns></returns>
        public static SoapEnvelope AsSoapEnvelope(this ISoapEnvelopeContainer obj, bool inherit = false)
        {
            if (obj == null)
                return null;

            var headers = SoapAttributeBase.GetCustomAttributeValues<SoapHeaderAttribute>(obj, inherit);
            var bodies = SoapAttributeBase.GetCustomAttributeValues<SoapBodyAttribute>(obj, inherit);
            
            return new SoapEnvelope
            {
                Header = new SoapHeader(headers),
                Body = new SoapBody(bodies)
            };
        }

        #endregion

        #region Hacks

        /// <summary>
        ///  Creates a new <see cref="XDocument"/> from a string that might not contain an XML declaration.
        /// </summary>
        /// <param name="xml">A string that contains XML.</param>
        /// <param name="enc">The encoding to use if the XML declaration has been omitted. Uses <see cref="Encoding.UTF8"/> if null.</param>
        /// <returns>An instance of <see cref="XDocument"/> populated from the string that contains XML, or null if <paramref name="xml"/> is null.</returns>
        public static XDocument XParseDocument(this string xml, Encoding enc = null)
        {
            if (xml == null) return null;

            xml = xml.Trim();

            if (!xml.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
            {
                enc = enc ?? Encoding.UTF8;
                xml = $@"<?xml version=""1.0"" encoding=""{enc.WebName}""?>" + xml;
            }

            return XDocument.Parse(xml, LoadOptions.None);
        }

        /// <summary>
        /// Removes all empty XML elements that are marked with the nil="true" attribute.
        /// </summary>
        /// <param name="input">The input for which to replace the content.</param>
        /// <param name="compactOutput">
        /// true to make the output more compact, if indentation was used; otherwise, false.
        /// </param>
        /// <returns>A string.</returns>
        public static string XStripNullableEmptyElements(this string input, bool compactOutput = false)
        {
            const RegexOptions OPTIONS =
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline;

            var result = Regex.Replace(
                input,
                @"<\w+\s+\w+:nil=""true""(\s+xmlns:\w+=""http://www.w3.org/2001/XMLSchema-instance"")?\s*/>",
                string.Empty,
                OPTIONS
            );

            if (compactOutput)
            {
                var sb = new StringBuilder();

                using (var sr = new StringReader(result))
                {
                    string ln;

                    while ((ln = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(ln))
                        {
                            sb.AppendLine(ln);
                        }
                    }
                }

                result = sb.ToString();
            }

            return result;
        }

        /// <summary>
        /// Removes all attributes from the specified XML element.
        /// </summary>
        /// <param name="xml">The XML string that contains the element.</param>
        /// <param name="elementName">The name of the XML element from which to remove the attributes.</param>
        /// <param name="nsPrefix">An optional namespace prefix.</param>
        /// <returns>A string.</returns>
        public static string XStripElementAttributes(this string xml, string elementName, string nsPrefix = null) =>
            Regex
            .Replace(xml, $@"\<{nsPrefix}{elementName}>\b([^>]+?)\s?\/?\>", $"<{elementName}>")
            .Replace($"</{nsPrefix}{elementName}>", $"</{elementName}>");

        #endregion

        #region Helpers

        private static object ThrowCannotDeserialize(Type type, bool? throwIfCannotDeserialize = null)
        {
            if (throwIfCannotDeserialize.HasValue && throwIfCannotDeserialize.Value || ThrowIfCannotDeserialize)
                throw new InvalidOperationException($"The specified XML document cannot be deserialized to an instance of the type {type.FullName}.");

            return null;
        }

        #endregion
    }
}