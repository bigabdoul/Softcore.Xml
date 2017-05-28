using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Represents a SOAP message envelope.
    /// </summary>
    [Serializable]
    public sealed class SoapEnvelope : SoapContainer
    {
        #region constructors

        /// <summary>
        /// Initializes an instance of the <see cref="SoapEnvelope"/> class.
        /// </summary>
        public SoapEnvelope()
        {
            NamespacesSorted = true;
        }

        /// <summary>
        /// Initializes an instance of the <see cref="SoapEnvelope"/> class with the specified body.
        /// </summary>
        /// <param name="body">The SOAP message body. Cannot be null during serialization.</param>
        public SoapEnvelope(SoapBody body) : this()
        {
            Body = body;
        }

        /// <summary>
        /// Initializes an instance of the <see cref="SoapEnvelope"/> class with the specified header.
        /// </summary>
        /// <param name="header">The SOAP message header. Can be null.</param>
        public SoapEnvelope(SoapHeader header) : this()
        {
            Header = header;
        }

        /// <summary>
        /// Initializes an instance of the <see cref="SoapEnvelope"/> class with the provided parameters.
        /// </summary>
        /// <param name="header">The SOAP message header. Can be null.</param>
        /// <param name="body">The SOAP message body. Cannot be null during serialization.</param>
        public SoapEnvelope(SoapHeader header, SoapBody body) : this()
        {
            Body = body;
            Header = header;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapEnvelope"/> class with the specified content.
        /// </summary>
        /// <param name="content">The content of the SOAP message body. Cannot be null during serialization.</param>
        public SoapEnvelope(object content) : this()
        {
            Content = content;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the SOAP message header. Can be null.
        /// </summary>
        [SoapIgnore] [XmlIgnore] public SoapHeader Header { get; set; }

        /// <summary>
        /// Gets or sets the SOAP message body. Cannot be null during XML serialization.
        /// </summary>
        [SoapIgnore] [XmlIgnore] public SoapBody Body
        {
            set => _body = value;
            get => (_body ?? (_body = new SoapBody()));
        }
        SoapBody _body;

        /// <summary>
        /// Gets or sets a value that indicates whether to exclude the XML declaration at the beginning of the serialized envelope.
        /// </summary>
        [SoapIgnore] [XmlIgnore] public bool ExcludeXmlDeclaration { get; set; }

        #endregion

        #region instance methods

        #region overrides

        /// <summary>
        /// Gets or sets the contents of the SOAP message body. Cannot be null, must be one of the 
        /// following types: <see cref="ISerializeXmlFragment"/>, a collection of objects, or a single object.
        /// </summary>
        [SoapIgnore] [XmlIgnore]
        public override object Content { get => Body?.Content; set => (Body ?? (Body = new SoapBody())).Content = value; }

        /// <summary>
        /// Serializes this <see cref="SoapEnvelope"/> instance.
        /// </summary>
        /// <returns>An XML string that represents the current <see cref="SoapEnvelope"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><see cref="Body"/> is null.</exception>
        public override string SerializeXml()
        {
            if (_body == null)
            {
                throw new ArgumentNullException(nameof(Body), $"{nameof(Body)} cannot be null.");
            }

            var enc = SetContentEncoding();
            var soap = GetTargetNamespacePrefix();
            var xmlDecl = ExcludeXmlDeclaration ? string.Empty : $@"<?xml version=""1.0"" encoding=""{enc}""?>";

            return new StringBuilder(xmlDecl)
                .Append($@"<{soap}:Envelope{GetNamespacesAndAttributes(soap)}>")
                .Append($"{Header?.SerializeXml()}")
                .Append($"{_body.SerializeXml()}")
                .Append($"</{soap}:Envelope>")
                .ToString();
        }

        /// <summary>
        /// Serializes the current instance as an XML fragment using the specified namespaces.
        /// </summary>
        /// <param name="namespaces">The namespaces referenced by the current instance.</param>
        /// <returns>A string that represents the XML-fragment produced by the current instance.</returns>
        public override string SerializeXmlFragment(params XmlQualifiedName[] namespaces)
        {
            SetNamespaces(namespaces);
            return SerializeXml();
        }

        #endregion

        #region fluent api (chained method calls)

        /// <summary>
        /// Creates a SOAP message body using the specified fault object.
        /// </summary>
        /// <param name="fault">The SOAP message fault.</param>
        /// <param name="namespaces">A one-dimensional array of <see cref="XmlQualifiedName"/> namespaces referenced by the body.</param>
        /// <returns>A reference to the current <see cref="SoapEnvelope"/> instance.</returns>
        public SoapEnvelope CreateBody(SoapFault fault, params XmlQualifiedName[] namespaces)
        {
            if (fault == null) throw new ArgumentNullException($"{nameof(fault)}");
            Body = new SoapBody(fault);
            Body.SetNamespaces(namespaces);
            return this;
        }

        /// <summary>
        /// Creates a SOAP message body using the specified content object.
        /// </summary>
        /// <param name="content">The content of the SOAP message body.</param>
        /// <param name="namespaces">
        /// A one-dimensional array of <see cref="XmlQualifiedName"/> namespaces referenced by the body.
        /// </param>
        /// <returns>A reference to the current <see cref="SoapEnvelope"/> instance.</returns>
        public SoapEnvelope CreateBody(object content, params XmlQualifiedName[] namespaces)
        {
            if (content == null) throw new ArgumentNullException($"{nameof(content)}");
            Body = new SoapBody(content);
            Body.SetNamespaces(namespaces);
            return this;
        }

        /// <summary>
        /// Creates a SOAP message header using the specified content object.
        /// </summary>
        /// <param name="content">The content of the SOAP message header.</param>
        /// <param name="namespaces">
        /// A one-dimensional array of <see cref="XmlQualifiedName"/> namespaces referenced by the header.
        /// </param>
        /// <returns>A reference to the current <see cref="SoapEnvelope"/> instance.</returns>
        public SoapEnvelope CreateHeader(object content, params XmlQualifiedName[] namespaces)
        {
            if (content == null && (namespaces == null || namespaces.Length == 0))
            {
                return this;
            }

            Header = new SoapHeader(content);
            Header.SetNamespaces(namespaces);

            return this;
        }
        
        /// <summary>
        /// Sets the <see cref="Content"/> property value to the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The content to set.</param>
        /// <returns>A reference to this <see cref="SoapEnvelope"/> instance.</returns>
        public SoapEnvelope SetContent(params object[] value)
        {
            Content = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ExcludeXmlDeclaration"/> property value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns>A reference to this <see cref="SoapEnvelope"/> instance.</returns>
        public SoapEnvelope SetExcludeXmlDeclaration(bool value)
        {
            ExcludeXmlDeclaration = value;
            return this;
        }

        #endregion

        #region helpers

        /// <summary>
        /// Sets the content encoding for this <see cref="SoapEnvelope"/>, the <see cref="Header"/>, and <see cref="Body"/> properties.
        /// </summary>
        /// <returns>A string that represents the <see cref="Encoding.WebName"/> of the used encoding.</returns>
        private string SetContentEncoding()
        {
            var enc = Encoding;
            if (enc == null)
            {
                enc = Encoding.UTF8;
                Encoding = enc;
            }
            if (Header != null)
            {
                Header.Encoding = enc;
            }
            Body.Encoding = enc;
            return enc.WebName;
        }

        /// <summary>
        /// Returns the XML namespaces referenced by this SOAP message envelope.
        /// </summary>
        /// <returns></returns>
        private string GetNamespacesAndAttributes(string prefix)
        {
            var list = new List<XmlQualifiedName>
            {
                new XmlQualifiedName(prefix, GetTargetNamespace())
            };

            SetNamespaces();

            if (Namespaces?.ToArray() is XmlQualifiedName[] array && array.Length > 0)
            {
                list.AddRange(array);
            }

            if (NamespacesSorted && list.Count > 1)
            {
                list.Sort(NamespaceComparison);
            }

            var attributes = new Dictionary<string, string>();

            foreach (var ns in list)
            {
                Merge(ns.Name, ns.Namespace);
            }

            MergeAttributes(target: attributes);

            return GetAttributes(attributes);

            void Merge(string name, string ns)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = "xmlns";
                }
                else
                {
                    name = $"xmlns:{name}";
                }
                attributes[name] = ns;
            }
        }

        #endregion

        #endregion

        #region static methods

        #region factory

        /// <summary>
        /// Creates a new instance of the <see cref="SoapEnvelope"/> class.
        /// </summary>
        /// <returns>A reference to the new <see cref="SoapEnvelope"/> instance.</returns>
        public static SoapEnvelope Create() => new SoapEnvelope();

        /// <summary>
        /// Creates and returns a new instance of the <see cref="SoapEnvelope"/> class using the specified body and header.
        /// </summary>
        /// <param name="body">The SOAP message body. Cannot be null during serialization.</param>
        /// <param name="header">The SOAP message header. Can be null.</param>
        /// <returns>A reference to the new <see cref="SoapEnvelope"/> instance.</returns>
        public static SoapEnvelope Create(SoapBody body, SoapHeader header = null) => new SoapEnvelope(header, body);

        /// <summary>
        /// Creates and returns a new instance of the <see cref="SoapEnvelope"/> class using the given body and header contents.
        /// </summary>
        /// <param name="bodyContent">The content of the SOAP message body. Cannot be null during serialization.</param>
        /// <param name="headerContent">The content of the SOAP message header. Can be null.</param>
        /// <returns>A reference to the new <see cref="SoapEnvelope"/> instance.</returns>
        public static SoapEnvelope Create(object bodyContent, object headerContent = null)

            => new SoapEnvelope().CreateBody(bodyContent).CreateHeader(headerContent);

        #endregion

        #region Parse/TryParse

        /// <summary>
        /// Parses the specified XML document to an instance of <see cref="SoapEnvelope"/> using the specified type finder function.
        /// </summary>
        /// <param name="xml">A string that contains SOAP-XML.</param>
        /// <param name="typesHint">
        /// A function that finds the appropriate type for each child XML element of the body and header elements.
        /// The first argument (bool) indicates whether it's the header being parsed (true) or not (false), and the return
        /// value is an array of types that are suitable for deserialization of each content part element (header or body).
        /// </param>
        /// <param name="targetNamespace"></param>
        /// <returns></returns>
        public static SoapEnvelope Parse(string xml, Func<SoapEnvelopePart, Type[]> typesHint, string targetNamespace = null)
        {
            var doc = xml.XParseDocument();

            return new SoapEnvelope
            {
                Body = SoapBody.Parse(doc, typesHint(SoapEnvelopePart.Body), typesHint(SoapEnvelopePart.Fault), targetNamespace: targetNamespace),
                Header = SoapHeader.Parse(doc, typesHint(SoapEnvelopePart.Header), targetNamespace: targetNamespace),
            };
        }

        /// <summary>
        /// Parses the specified XML document to an instance of <see cref="SoapEnvelope"/> by parsing only the SOAP envelope body.
        /// </summary>
        /// <typeparam name="TBody">The type of the body content.</typeparam>
        /// <param name="xml">A string that contains SOAP-XML.</param>
        /// <returns></returns>
        public static SoapEnvelope Parse<TBody>(string xml) where TBody : class, new()

            => new SoapEnvelope { Body = SoapBody.Parse<TBody>(xml.XParseDocument()) };

        /// <summary>
        /// Parses the specified XML document to an instance of <see cref="SoapEnvelope"/>.
        /// </summary>
        /// <typeparam name="THeader">The type of the header content.</typeparam>
        /// <typeparam name="TBody">The type of the body content.</typeparam>
        /// <param name="xml">A string that contains SOAP-XML.</param>
        /// <returns></returns>
        public static SoapEnvelope Parse<THeader, TBody>(string xml) where THeader : class, new() where TBody : class, new()

            => Parse(xml, part => part == SoapEnvelopePart.Header ? new[] { typeof(THeader) } : new[] { typeof(TBody) });

        /// <summary>
        /// Attempts to parse the specified XML document to an instance of <see cref="SoapEnvelope"/> using the specified type finder function.
        /// </summary>
        /// <param name="xml">A string that contains SOAP-XML.</param>
        /// <param name="result">Returns an instance of <see cref="SoapEnvelope"/> if the operation succeeds, or null if it fails.</param>
        /// <param name="findTypes">
        /// A function that finds the appropriate types for each child XML element of the body, header, and fault elements.
        /// </param>
        /// <returns>true if the operation succeeds, false if it fails.</returns>
        public static bool TryParse(string xml, out SoapEnvelope result, Func<SoapEnvelopePart, Type[]> findTypes)
        {
            result = null;

            try
            {
                result = Parse(xml, findTypes);
            }
            catch (Exception)
            {
            }

            return result != null;
        }

        /// <summary>
        /// Attempts to parse the specified XML document to an instance of <see cref="SoapEnvelope"/> by parsing only the SOAP envelope body.
        /// </summary>
        /// <typeparam name="TBody">The type of the body content.</typeparam>
        /// <param name="xml">A string that contains SOAP-XML.</param>
        /// <param name="result">Returns an instance of <see cref="SoapEnvelope"/> if the operation succeeds, or null if it fails.</param>
        /// <returns>true if the operation succeeds, false if it fails.</returns>
        public static bool TryParse<TBody>(string xml, out SoapEnvelope result) where TBody : class, new()
        {
            result = null;

            try
            {
                result = Parse<TBody>(xml);
            }
            catch
            {
            }

            return result != null;
        }

        /// <summary>
        /// Attempts to parse the specified XML document to an instance of <see cref="SoapEnvelope"/>.
        /// </summary>
        /// <typeparam name="THeader">The type of the header content.</typeparam>
        /// <typeparam name="TBody">The type of the body content.</typeparam>
        /// <param name="xml">A string that contains SOAP-XML.</param>
        /// <param name="result">Returns an instance of <see cref="SoapEnvelope"/> if the operation succeeds, or null if it fails.</param>
        /// <returns>true if the operation succeeds, false if it fails.</returns>
        public static bool TryParse<THeader, TBody>(string xml, out SoapEnvelope result)
            where THeader : class, new() where TBody : class, new()
        {
            result = null;

            try
            {
                result = Parse<THeader, TBody>(xml);
            }
            catch (Exception)
            {
            }

            return result != null;
        }

        #endregion

        #endregion
    }
}
