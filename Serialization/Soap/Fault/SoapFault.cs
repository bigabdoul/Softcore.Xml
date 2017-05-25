using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Carries error and status information within a SOAP message. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [XmlRoot("Fault", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    public sealed class SoapFault : SoapContainer
    {
        #region constructors

        /// <summary>
        /// Initializes an instance of the <see cref="SoapFault"/>.
        /// </summary>
        public SoapFault()
        {
            Reason = new FaultReason();
            IncludeTargetNamespace = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapFault"/> class, setting the properties to specified values.
        /// </summary>
        /// <param name="code">The fault code for the new instance of <see cref="SoapFault"/>. The fault code identifies the type of the fault that occurred.</param>
        /// <param name="reasons">A one-dimensional array of <see cref="ReasonText"/> objects.</param>
        public SoapFault(FaultCode code, params ReasonText[] reasons) : this(code, null, reasons)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapFault"/> class, setting the properties to specified values.
        /// </summary>
        /// <param name="code">The fault code for the new instance of <see cref="SoapFault"/>. The fault code identifies the type of the fault that occurred.</param>
        /// <param name="detail">The description of a common language runtime exception.</param>
        /// <param name="reasons">A one-dimensional array of <see cref="ReasonText"/> objects.</param>
        public SoapFault(FaultCode code, object detail, params ReasonText[] reasons) : this()
        {
            Code = code;
            Detail = detail;

            if (reasons != null && reasons.Length > 0)
            {
                Reason.Items = reasons;
            }
        }

        #endregion

        #region properties

        /// <summary>
        ///  Gets or sets the fault code for the <see cref="SoapFault"/>. The fault code identifies the type of the fault that occurred.
        /// </summary>
        /// <returns>The fault code for this <see cref="SoapFault"/>.</returns>
        [XmlElement(Order = 0)] public FaultCode Code { get; set; }

        /// <summary>
        /// Gets or sets the reasons for the <see cref="SoapFault"/>.
        /// </summary>
        [XmlElement(Order = 1)] public FaultReason Reason { get; set; }

        /// <summary>
        /// Gets or sets the fault node for this <see cref="SoapFault"/>. See XML reference type="xs:anyURI".
        /// </summary>
        [XmlElement(Order = 2)] public string Node { get; set; }

        /// <summary>
        /// Gets or sets the fault role for this <see cref="SoapFault"/>. See XML reference type="xs:anyURI".
        /// </summary>
        [XmlElement(Order = 3)] public string Role { get; set; }

        /// <summary>
        /// Gets or sets additional information required for the <see cref="SoapFault"/>.
        /// </summary>
        /// <returns>Additional information required for the <see cref="SoapFault"/>.</returns>
        /// <remarks>
        /// This property is decorated with <see cref="XmlIgnoreAttribute"/> because we don't know its type at this moment.
        /// Therefore, it must be serialized as a separate XML fragment and appended as the last child of the 'Fault' element.
        /// This is done in the <see cref="SerializeDetail(string)"/> method.
        /// </remarks>
        [XmlIgnore] public object Detail { get; set; }

        #endregion

        #region overridden methods

        /// <summary>
        /// Serializes this <see cref="SoapFault"/> instance.
        /// </summary>
        /// <returns>An XML string that represents the current <see cref="SoapHeader"/> instance.</returns>
        public override string SerializeXml()
        {
            SetNamespaces();

            var soapFaultXml = this.XSerializeFragment(Namespaces);

            if (Detail == null)
                return soapFaultXml;

            return SerializeDetail(soapFaultXml);
        }

        #endregion

        #region fluent api

        /// <summary>
        /// Sets the fault code for the <see cref="SoapFault"/>.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public SoapFault SetCode(FaultCode value)
        {
            Code = value;
            return this;
        }

        /// <summary>
        /// Sets the reasons for the <see cref="SoapFault"/>.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public SoapFault SetReason(params ReasonText[] value)
        {
            Reason.Items = value;
            return this;
        }

        /// <summary>
        /// Sets the fault node for this <see cref="SoapFault"/>.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public SoapFault SetNode(string value)
        {
            Node = value;
            return this;
        }

        /// <summary>
        /// Sets the fault role for this <see cref="SoapFault"/>.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public SoapFault SetRole(string value)
        {
            Role = value;
            return this;
        }

        /// <summary>
        /// Sets additional information required for the <see cref="SoapFault"/>.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public SoapFault SetDetail(object value)
        {
            Detail = value;
            return this;
        }

        #endregion

        #region Parse/TryParse

        /// <summary>
        /// Parses the specified XML document and eventually returns a new instance of the <see cref="SoapFault"/> class.
        /// </summary>
        /// <param name="doc">The XML document to parse.</param>
        /// <param name="detailTypes">
        /// An array of types that can be deserialized as the 'Detail' child element of the 'Fault' element. Can be null.
        /// </param>
        /// <returns></returns>
        public static SoapFault Parse(XDocument doc, Type[] detailTypes = null)
        {
            if (doc.TryFindXElement("Fault", out XElement element, TargetNamespace))
            {
                return Parse(element, detailTypes);
            }

            return null;
        }

        /// <summary>
        /// Parses the specified XML element that represents the 'Fault' node.
        /// </summary>
        /// <param name="faultElement">The SOAP 'Fault' element to parse.</param>
        /// <param name="detailTypes">An array of types that can be deserialized as the 'Detail' child element of the 'Fault' element. Can be null.</param>
        /// <returns></returns>
        public static SoapFault Parse(XElement faultElement, Type[] detailTypes = null)
        {
            if (// try to find the 'Detail' of the 'Fault' element
                faultElement.TryFindXElement("Detail", out var xdetail, TargetNamespace) &&

                // and then parse that element using the specified types, or the default ServerFault type
                ParseContent(xdetail, detailTypes ?? new[] { typeof(ServerFault) }) is object content
            )
            {
                // 'Detail' element was found and parsed into an object instance; remove it now
                // so that we can add it as the detail content of the parsed original 'faultElement'
                xdetail.Remove();
            }
            else
            {
                // variable must be initialized
                content = null;
            }

            return faultElement.ToString().XDeserialize<SoapFault>(true).SetDetail(content);
        }

        /// <summary>
        /// Attempts to parse the specified XML document and eventually returns a new instance of the <see cref="SoapFault"/> class.
        /// </summary>
        /// <param name="doc">The XML document to parse.</param>
        /// <param name="result">If the operation succeeds, returns a new instance of <see cref="SoapFault"/>.</param>
        /// <param name="detailTypes">
        /// An array of types that can be deserialized as the 'Detail' child element of the 'Fault' element. Can be null.
        /// </param>
        /// <returns></returns>
        public static bool TryParse(XDocument doc, out SoapFault result, Type[] detailTypes = null)
        {
            result = null;

            try
            {
                result = Parse(doc, detailTypes);
            }
            catch
            {
            }

            return result != null;
        }

        /// <summary>
        /// Attempts to parse the specified XML element that represents the 'Fault' node and eventually returns a new instance of the <see cref="SoapFault"/> class.
        /// </summary>
        /// <param name="faultElement">The SOAP 'Fault' element to parse.</param>
        /// <param name="result">If the operation succeeds, returns a new instance of <see cref="SoapFault"/>.</param>
        /// <param name="detailTypes">An array of types that can be deserialized as the 'Detail' child element of the 'Fault' element. Can be null.</param>
        /// <returns></returns>
        public static bool TryParse(XElement faultElement, out SoapFault result, Type[] detailTypes = null)
        {
            result = null;

            try
            {
                result = Parse(faultElement, detailTypes);
            }
            catch
            {
            }

            return result != null;
        }

        #endregion

        #region helpers

        /// <summary>
        /// Serializes the <see cref="Detail"/> property value as an XML fragment 
        /// and appends it as the last child of <paramref name="soapFaultXml"/>.
        /// </summary>
        /// <param name="soapFaultXml">The serialized <see cref="SoapFault"/> XML fragment.</param>
        /// <returns></returns>
        private string SerializeDetail(string soapFaultXml)
        {
            var detail = Detail;

            if (detail == null) return soapFaultXml;

            // TODO: figure out a better way to handle the Detail property.
            // This just looks and feels like a hack: insert the 'Detail' element as the last child of 'Fault'; works at least :-)
            var ns = Namespaces;
            var tns = GetTargetNamespacePrefix();

            ParseFragment(soapFaultXml, out XElement elmFault, tns);

            string xmlDetail;

            if (detail is ISerializeXmlFragment serializable)
            {
                // maybe it has its own way of serializing?
                xmlDetail = serializable.SerializeXmlFragment(Namespaces?.ToArray());
            }
            else
            {
                xmlDetail = detail.XSerializeFragment(Namespaces);
            }

            var frag = EncloseInElement("Detail", xmlDetail, tns);

            ParseFragment(frag, out XElement elmDetail, tns);
            elmFault.LastNode.AddAfterSelf(elmDetail);

            return elmFault.ToString();
        }

        #endregion
    }
}
