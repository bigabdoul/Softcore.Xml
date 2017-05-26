using System;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Carries error and status information within a SOAP message version 1.2. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [XmlRoot("Fault", Namespace = SoapVersion12TargetNamespace)]
    public sealed class SoapFault : SoapFaultBase
    {
        #region constructors

        /// <summary>
        /// Initializes an instance of the <see cref="SoapFault"/>.
        /// </summary>
        public SoapFault()
        {
            Reason = new FaultReason();
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

        #endregion

        #region overridden methods

        /// <summary>
        /// Serializes this <see cref="SoapFault"/> instance.
        /// </summary>
        /// <returns>An XML string that represents the current <see cref="SoapFault"/> instance.</returns>
        public override string SerializeXml()
        {
            SetNamespaces();

            var soapFaultXml = this.XSerializeFragment(Namespaces);

            if (Detail != null)
                soapFaultXml = SerializeDetail(soapFaultXml);

            return soapFaultXml.XStripElementAttributes("Fault", TargetNamespacePrefixDefault);
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
        /// Parses the specified XML document and eventually returns a new instance of the <see cref="SoapFaultBase"/> class.
        /// </summary>
        /// <param name="doc">The XML document to parse.</param>
        /// <param name="detailTypes">
        /// An array of types that can be deserialized as the 'Detail' child element of the 'Fault' element. Can be null.
        /// </param>
        /// <returns></returns>
        public static SoapFaultBase Parse(XDocument doc, Type[] detailTypes = null)
        {
            if (IsVersion12)
            {
                if (doc.TryFindXElement("Fault", out XElement element, SoapVersion12TargetNamespace))
                {
                    return SoapFaultBase.Parse(element, detailTypes);
                }
            }
            else if (doc.TryFindXElement("Fault", out XElement element, SoapVersion11TargetNamespace))
            {
                return SoapFaultBase.Parse(element, detailTypes);
            }

            return null;
        }

        /// <summary>
        /// Parses the specified XML element that represents the 'Fault' node.
        /// </summary>
        /// <param name="faultElement">The SOAP 'Fault' element to parse.</param>
        /// <param name="detailTypes">An array of types that can be deserialized as the 'Detail' child element of the 'Fault' element. Can be null.</param>
        /// <returns></returns>
        public static new SoapFault Parse(XElement faultElement, Type[] detailTypes = null)
        {
            return (SoapFault)SoapFaultBase.Parse(faultElement, detailTypes);
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
                result = (SoapFault)Parse(doc, detailTypes);
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
                result = (SoapFault)Parse(faultElement, detailTypes);
            }
            catch
            {
            }

            return result != null;
        }

        #endregion
    }
}
