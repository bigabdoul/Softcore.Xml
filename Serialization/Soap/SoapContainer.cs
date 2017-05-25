using Newtonsoft.Json;
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

        /// <summary>
        /// The target namespace of the SOAP message specification.
        /// </summary>
        [JsonIgnore] [SoapIgnore] [XmlIgnore] public static string TargetNamespace = "http://www.w3.org/2003/05/soap-envelope";

        /// <summary>
        /// Gets or sets the default local name for the target namespace (http://www.w3.org/2003/05/soap-envelope) of any SOAP envelope and child elements. The default is 'soap'.
        /// </summary>
        [JsonIgnore] [SoapIgnore] [XmlIgnore] public static string TargetNamespaceLocalNameDefault { get; set; } = "soap";

        /// <summary>
        /// Gets or sets the SOAP target namespace attribute prefix (i.e. 'env', 'soap', 'soapenv', etc.). The default is 'soap'.
        /// </summary>
        [JsonIgnore] [SoapIgnore] [XmlIgnore] public string TargetNamespaceLocalName { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to include the qualified target namespace.
        /// </summary>
        [JsonIgnore] [SoapIgnore] [XmlIgnore] public bool IncludeTargetNamespace { get; set; }

        /// <summary>
        /// Gets or sets the contents of the SOAP message header or body. Can be null (header only), 
        /// of type <see cref="ISerializeXmlFragment"/>, a collection of objects, or a single object.
        /// </summary>
        [JsonIgnore] [SoapIgnore] [XmlIgnore] public virtual object Content { get; set; }

        #endregion

        #region methods

        #region overridden

        /// <summary>
        /// Adds the target XML namespace for SOAP and the specified array of <see cref="XmlQualifiedName"/> 
        /// objects to the namespaces referenced by the current <see cref="SoapContainer"/> instance.
        /// </summary>
        /// <param name="args">A one-dimensional array of <see cref="XmlQualifiedName"/> objects.</param>
        public override void SetNamespaces(params XmlQualifiedName[] args)
        {
            if (IncludeTargetNamespace)
                base.SetNamespaces(MergeNamespaces(new XmlQualifiedName(GetTargetNamespacePrefix(), TargetNamespace), args));
            else
                base.SetNamespaces(args);
        }

        /// <summary>
        /// Serializes this <see cref="SoapContainer"/> instance as an XML fragment rather than a full-fledged XML document.
        /// </summary>
        /// <returns></returns>
        public override string SerializeXml()
        {
            return XSerializeExtension.XSerializeFragment(this, Namespaces);
            //return this.XSerializeFragment(Namespaces);
        }

        #endregion

        #region public

        /// <summary>
        /// Casts the <see cref="Content"/> property value to the specified <typeparamref name="TContent"/> type.
        /// </summary>
        /// <typeparam name="TContent">The type to which to cast the <see cref="Content"/> value.</typeparam>
        /// <returns></returns>
        public virtual TContent GetContent<TContent>()
        {
            return (TContent)Content;
        }

        /// <summary>
        /// Returns the default prefix of the target namespace (http://www.w3.org/2003/05/soap-envelope) for the SOAP envelope.
        /// </summary>
        /// <returns></returns>
        public virtual string GetTargetNamespacePrefix()

            => string.IsNullOrWhiteSpace(TargetNamespaceLocalName) ? TargetNamespaceLocalNameDefault : TargetNamespaceLocalName.Trim();

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
            xml = $@"<?xml version=""1.0""?><{doc} xmlns:{tns}=""{TargetNamespace}"">{xml}</{doc}>";

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
                    if (elm.ToString().XDeserialize(type, false) is object content)
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
