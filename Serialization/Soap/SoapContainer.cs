using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Softcore.Xml.Serialization.Soap
{
    using static XSerializeExtension;

    /// <summary>
    /// Represents the base class for SOAP-serializable classes.
    /// </summary>
    [System.Serializable]
    public class SoapContainer : SerializeBase, ISoapContainer
    {
        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapContainer"/> class.
        /// </summary>
        public SoapContainer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapContainer"/> class with the specified content.
        /// </summary>
        /// <param name="content">
        /// The contents of the SOAP message header or body. Can be null (header only), of type
        /// <see cref="ISerializeXmlFragment"/>, a collection of objects, or a single object.
        /// </param>
        public SoapContainer(object content)
        {
            Content = content;
        }

        #endregion

        #region non serialized fields and properties

        #region const and static

        /// <summary>
        /// The target namespace for SOAP version 1.1
        /// </summary>
        public const string SoapVersion11TargetNamespace = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// Indicates the 'encodingStyle' pattern described in the SOAP Version 1.1 specification.
        /// </summary>
        public const string SoapVersion11EncodingNamespace = "http://schemas.xmlsoap.org/soap/encoding/";

        /// <summary>
        /// The target namespace for SOAP version 1.2
        /// </summary>
        public const string SoapVersion12TargetNamespace = "http://www.w3.org/2003/05/soap-envelope";

        /// <summary>
        /// Indicates the 'encodingStyle' pattern described in the SOAP Version 1.2 Part 2: Adjuncts Recommendation.
        /// </summary>
        public const string SoapVersion12EncodingNamespace = "http://www.w3.org/2003/05/soap-encoding";

        /// <summary>
        /// The target namespace of the SOAP message specification. The default is the target namespace for SOAP version 1.2.
        /// </summary>
        [SoapIgnore] [XmlIgnore] public static string DefaultTargetNamespace = SoapVersion12TargetNamespace;

        /// <summary>
        /// Gets or sets the default prefix for the target namespace of any SOAP envelope and child elements. The default is 'soap'.
        /// </summary>
        [SoapIgnore] [XmlIgnore] public static string TargetNamespacePrefixDefault { get; set; } = "soap";

        #endregion

        /// <summary>
        /// Gets or sets the target namespace to use when deserializing an XML document.
        /// </summary>
        [SoapIgnore] [XmlIgnore] public string TargetNamespace { get; set; }

        /// <summary>
        /// Gets or sets the SOAP target namespace attribute prefix (i.e. 'env', 'soap', 'soapenv', etc.). The default is 'soap'.
        /// </summary>
        [SoapIgnore] [XmlIgnore] public string TargetNamespacePrefix { get; set; }

        /// <summary>
        /// Gets or sets the contents of the SOAP message header or body. Can be null (header only), 
        /// of type <see cref="ISerializeXmlFragment"/>, a collection of objects, or a single object.
        /// </summary>
        [SoapIgnore] [XmlIgnore] public virtual object Content { get; set; }

        /// <summary>
        /// Returns the SOAP version based on the <see cref="DefaultTargetNamespace"/> property value.
        /// </summary>
        /// <exception cref="NotSupportedException">Unknown SOAP target namespace.</exception>
        public static string SoapVersion
        {
            get
            {
                return GetVersion(DefaultTargetNamespace);
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="DefaultTargetNamespace"/> points to the version 1.1 of the SOAP specification. A <see cref="NotSupportedException"/> is thrown if the version is neither 1.1 nor 1.2.
        /// </summary>
        public static bool IsVersion11 { get => SoapVersion == "1.1"; }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="DefaultTargetNamespace"/> points to the version 1.2 of the SOAP specification. A <see cref="NotSupportedException"/> is thrown if the version is neither 1.1 nor 1.2.
        /// </summary>
        public static bool IsVersion12 { get => SoapVersion == "1.2"; }

        #endregion

        #region methods

        #region overridden

        /// <summary>
        /// Serializes this <see cref="SoapContainer"/> instance as an XML fragment rather than a full-fledged XML document.
        /// </summary>
        /// <returns></returns>
        public override string SerializeXml()
        {
            return this.XSerializeFragment(Namespaces);
        }

        #endregion

        #region public

        /// <summary>
        /// Filters the elements of the <see cref="Content"/> property value (assuming it's a collection) based on a specified type.
        /// </summary>
        /// <typeparam name="TContent">The type to filter the elements of the sequence on.</typeparam>
        /// <returns>
        /// A <see cref="IEnumerable{TContent}"/> that contains elements from the <see cref="Content"/> sequence of type <typeparamref name="TContent"/>.
        /// </returns>
        public virtual IEnumerable<TContent> SelectContent<TContent>()
        {
            if (Content is IEnumerable collection)
            {
                return collection.OfType<TContent>();
            }

            try
            {
                return new[] { (TContent)Content };
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Casts the <see cref="Content"/> property value to the specified <typeparamref name="TContent"/> type.
        /// </summary>
        /// <typeparam name="TContent">The type to which to cast the <see cref="Content"/> value.</typeparam>
        /// <returns></returns>
        public virtual TContent GetContent<TContent>()
        {
            if (SelectContent<TContent>() is IEnumerable<TContent> contents)
            {
                return contents.FirstOrDefault();
            }
            return default(TContent);
        }

        /// <summary>
        /// Returns the target namespace to use when serializing an object or deserializing an XML document.
        /// </summary>
        /// <returns></returns>
        public virtual string GetTargetNamespace()

            => string.IsNullOrWhiteSpace(TargetNamespace) ? DefaultTargetNamespace : TargetNamespace.Trim();

        /// <summary>
        /// Returns the default prefix of the target namespace for this <see cref="SoapContainer"/>.
        /// </summary>
        /// <returns></returns>
        public virtual string GetTargetNamespacePrefix()

            => string.IsNullOrWhiteSpace(TargetNamespacePrefix) ? TargetNamespacePrefixDefault : TargetNamespacePrefix.Trim();

        #region fluent api (chained method calls)

        /// <summary>
        /// Sets the <see cref="Content"/> property value to the specified <paramref name="content"/> and returns a reference 
        /// to this <see cref="SoapContainer"/> instance by casting it to the specified <typeparamref name="TContainer"/> type.
        /// </summary>
        /// <typeparam name="TContainer">The type to which this <see cref="SoapContainer"/> instance is cast.</typeparam>
        /// <param name="content">The content to set.</param>
        /// <returns>
        /// A reference to this <see cref="SoapContainer"/> instance with an explicit cast to <typeparamref name="TContainer"/>.
        /// </returns>
        public virtual TContainer SetContent<TContainer>(params object[] content) where TContainer : SoapContainer
        {
            Content = content;
            return (TContainer)this;
        }

        /// <summary>
        /// Adds the target XML namespace for SOAP and the specified array of <see cref="XmlQualifiedName"/> objects 
        /// to the namespaces referenced by the current <see cref="SoapContainer"/> instance and returns a reference to
        /// this <see cref="SoapContainer"/> instance by casting it to the specified <typeparamref name="TContainer"/> type.
        /// </summary>
        /// <typeparam name="TContainer">The type to which this <see cref="SoapContainer"/> instance is cast.</typeparam>
        /// <param name="args">A one-dimensional array of <see cref="XmlQualifiedName"/> objects.</param>
        /// <returns></returns>
        public virtual TContainer SetNamespaces<TContainer>(params XmlQualifiedName[] args) where TContainer : SoapContainer
        {
            SetNamespaces(args);
            return (TContainer)this;
        }

        /// <summary>
        /// Adds an array of <see cref="XmlQualifiedName"/> objects to the namespaces used by the current instance.
        /// This ensures that the <see cref="TargetNamespace"/> property value is not present in the given array.
        /// </summary>
        /// <param name="args">A one-dimensional array of <see cref="XmlQualifiedName"/> objects.</param>
        public override void SetNamespaces(params XmlQualifiedName[] args)
        {
            if (args == null || args.Length == 0)
            {
                return;
            }

            var list = args.ToList();

            // remove duplicate target namespace
            for (int i = list.Count - 1; i > -1; i--)
            {
                var n = list[i];
                if (string.Equals(n.Namespace, GetTargetNamespace(), StringComparison.Ordinal))
                {
                    list.RemoveAt(i);
                }
            }

            base.SetNamespaces(list.ToArray());
        }

        #endregion

        #region static

        /// <summary>
        /// Returns the SOAP version based on the specified <paramref name="targetNamespace"/> argument.
        /// </summary>
        /// <param name="targetNamespace">The SOAP target namespace for which to determine the version. If null, the <see cref="DefaultTargetNamespace"/> property value is used.</param>
        /// <returns>A string that represents the version of the SOAP target namespace.</returns>
        /// <exception cref="NotSupportedException">Unknown SOAP target namespace.</exception>
        public static string GetVersion(string targetNamespace = null)
        {
            targetNamespace = targetNamespace ?? DefaultTargetNamespace;

            if (targetNamespace == SoapVersion11TargetNamespace)
                return "1.1";
            if (targetNamespace == SoapVersion12TargetNamespace)
                return "1.2";

            throw new NotSupportedException($"Unknown SOAP target namespace '{targetNamespace}'. Target namespace must be either '{SoapVersion11TargetNamespace}' (for version 1.1) or '{SoapVersion12TargetNamespace}' (for version 1.2).");
        }

        /// <summary>
        /// Returns a value that indicates whether the specified <paramref name="targetNamespace"/> points to the version 1.1 of the SOAP specification. A <see cref="NotSupportedException"/> is thrown if the version is neither 1.1 nor 1.2.
        /// </summary>
        /// <param name="targetNamespace">The SOAP target namespace for which to determine the version. If null, the <see cref="DefaultTargetNamespace"/> property value is used.</param>
        /// <returns></returns>
        public static bool IsProtocolVersion11(string targetNamespace = null)
        {
            return GetVersion(targetNamespace) == "1.2";
        }

        /// <summary>
        /// Returns a value that indicates whether the specified <paramref name="targetNamespace"/> points to the version 1.2 of the SOAP specification. A <see cref="NotSupportedException"/> is thrown if the version is neither 1.1 nor 1.2.
        /// </summary>
        /// <param name="targetNamespace">The SOAP target namespace for which to determine the version. If null, the <see cref="DefaultTargetNamespace"/> property value is used.</param>
        /// <returns></returns>
        public static bool IsProtocolVersion12(string targetNamespace = null)
        {
            return GetVersion(targetNamespace) == "1.2";
        }

        #endregion

        #endregion

        #region protected

        /// <summary>
        /// Wraps the specified XML element name, using its qualified name (the target namespace prefix), around the provided value.
        /// </summary>
        /// <param name="name">The XML element name to wrap around <paramref name="xml"/>.</param>
        /// <param name="xml">The XML string to enclose in <paramref name="name"/> element.</param>
        /// <param name="tns">The SOAP target namespace prefix to use. If null, the return value of the method <see cref="GetTargetNamespacePrefix()"/> is used.</param>
        /// <returns></returns>
        protected virtual string EncloseInElement(string name, string xml, string tns = null)
        {
            tns = tns ?? GetTargetNamespacePrefix();
            var attrs = GetAttributes();
            return $@"<{tns}:{name}{attrs}>{xml}</{tns}:{name}>";
        }

        /// <summary>
        /// Creates a new <see cref="XDocument"/> from an XML fragment.
        /// </summary>
        /// <param name="xml">A string that contains XML text with no XML declaration.</param>
        /// <param name="elm">Returns the first element after the added document node.</param>
        /// <param name="tns">The SOAP target namespace prefix to use. If null, the return value 
        /// of the method <see cref="GetTargetNamespacePrefix()"/> is called.</param>
        /// <returns>An instance of <see cref="XDocument"/> populated from the string that contains XML.</returns>
        protected virtual XDocument ParseFragment(string xml, out XElement elm, string tns = null)
        {
            const string doc = "document";

            tns = tns ?? GetTargetNamespacePrefix();
            xml = $@"<?xml version=""1.0""?><{doc} xmlns:{tns}=""{GetTargetNamespace()}"">{xml}</{doc}>";

            var xdoc = XDocument.Parse(xml, LoadOptions.None);

            elm = xdoc.Element(doc).Descendants().FirstOrDefault();

            return xdoc;
        }

        /// <summary>
        /// Serializes the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">
        /// The content to serialize. Can be null, of type <see cref="ISerializeXmlFragment"/>, a collection of objects, or a single object.
        /// </param>
        /// <returns></returns>
        protected virtual string SerializeContents(object content)
        {
            if (content == null) return string.Empty;

            string xml;

            if (content is ISerializeXmlFragment serializable)
            {
                serializable.Encoding = Encoding;
                xml = serializable.SerializeXmlFragment(Namespaces?.ToArray());
            }
            else if (content is IEnumerable collection)
            {
                var enc = Encoding;
                var ns = Namespaces?.ToArray();
                var sb = new System.Text.StringBuilder();

                foreach (var item in collection)
                {
                    if (item is ISerializeXmlFragment frag)
                    {
                        frag.Encoding = enc;
                        sb.Append(frag.SerializeXmlFragment(ns));
                    }
                    else if (item != null)
                    {
                        sb.Append(item.XSerializeFragment(enc, ns));
                    }
                }

                xml = sb.ToString();
                sb.Clear();
            }
            else
            {
                xml = content?.XSerializeFragment(Namespaces);
            }

            return xml;
        }

        #endregion

        #region internal

        /// <summary>
        /// Deserializes the elements of the specified <paramref name="element"/> using the provided type information.
        /// </summary>
        /// <param name="element">The XML fragment to deserialize.</param>
        /// <param name="types">An array of types that <paramref name="element"/> can deserialize to.</param>
        /// <returns>A single object, an array of objects, or null.</returns>
        internal static object ParseContent(XElement element, Type[] types)
        {
            if (element == null || types == null) return null;

            var list = new List<object>();

            foreach (var elm in element.Elements())
            {
                var xml = elm.ToString();

                foreach (var type in types)
                {
                    if (xml.XDeserialize(type, false) is object content)
                    {
                        list.Add(content);
                        break; // exit loop if one type has been deserialized for this element?
                    }
                }
            }

            if (list.Count == 0)
            {
                return null;
            }

            return list.Count == 1 ? list[0] : list.ToArray();
        }

        #endregion

        #endregion
    }
}
