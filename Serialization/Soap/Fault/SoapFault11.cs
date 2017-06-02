using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Carries error and status information within a SOAP message version 1.2. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [XmlRoot("Fault")]
    public sealed class SoapFault11 : SoapFaultBase
    {
        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapFault11"/> class.
        /// </summary>
        public SoapFault11()
        {
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the code for this <see cref="SoapFault11"/>. The fault code identifies the type of the fault that occurred.
        /// </summary>
        [XmlElement("faultcode", Order = 0, Form = XmlSchemaForm.Unqualified)]
        public string FaultCode { get; set; }

        /// <summary>
        /// Gets or sets the description for this <see cref="SoapFault11"/>.
        /// </summary>
        [XmlElement("faultstring", Order = 1, Form = XmlSchemaForm.Unqualified)]
        public string FaultString { get; set; }

        /// <summary>
        /// Gets or sets the URI that caused the current <see cref="SoapFault11"/>.
        /// </summary>
        [XmlElement("faultactor", Order = 2, Form = XmlSchemaForm.Unqualified)]
        public string FaultActor { get; set; }

        /// <summary>
        /// Gets or sets additional information required for the <see cref="SoapFault11"/>.
        /// </summary>
        /// <remarks>
        /// This property is decorated with <see cref="XmlIgnoreAttribute"/> because we don't know its type at this moment.
        /// Therefore, it must be serialized as a separate XML fragment and appended as the last child of the 'Fault' element.
        /// This is done in the <see cref="SoapFaultBase.SerializeDetail(string, bool)"/> method.
        /// </remarks>
        [XmlElement("detail", Order = 3, Form = XmlSchemaForm.Unqualified)]
        public override object Detail { get; set; }

        #endregion

        /// <summary>
        /// Serializes this <see cref="SoapFault11"/> instance.
        /// </summary>
        /// <returns>An XML string that represents the current <see cref="SoapFault11"/> instance.</returns>
        public override string SerializeXml()
        {
            SetNamespaces();

            // because the 'Detail' property is not ignored during serialization, set null
            var detail = Detail;
            Detail = null;

            var soapFaultXml = this.XSerializeFragment();

            if (detail != null)
            {
                Detail = detail; // reset and perform separate serialization for the detail object
                soapFaultXml = SerializeDetail(soapFaultXml, true);
            }

            return soapFaultXml.XStripElementAttributes("Fault", TargetNamespacePrefixDefault);
        }

        /// <summary>
        /// Sets additional information required for the <see cref="SoapFault11"/>.
        /// </summary>
        /// <param name="value">The value to set.</param>
        /// <returns></returns>
        public SoapFault11 SetDetail(object value)
        {
            Detail = value;
            return this;
        }

        #region Parse/TryParse

        /// <summary>
        /// Parses the specified XML document and eventually returns a new instance of the <see cref="SoapFault"/> class.
        /// </summary>
        /// <param name="doc">The XML document to parse.</param>
        /// <param name="detailTypes">
        /// An array of types that can be deserialized as the 'Detail' child element of the 'Fault' element. Can be null.
        /// </param>
        /// <returns></returns>
        public static SoapFault11 Parse(XDocument doc, Type[] detailTypes = null)
        {
            if (doc.TryFindXElement("Fault", out XElement element))
            {
                return Parse(element, detailTypes);
            }
            return null;
        }

        /// <summary>
        /// Parses the specified XML element that represents the 'Fault' node.
        /// </summary>
        /// <param name="faultElement">The SOAP 'Fault' element to parse.</param>
        /// <param name="detailTypes">An array of types that can be deserialized as the 'detail' child element of the 'Fault' element. Can be null.</param>
        /// <returns></returns>
        public static SoapFault11 Parse(XElement faultElement, Type[] detailTypes = null)
        {
            return (SoapFault11)Parse(faultElement, detailTypes, SoapVersion11TargetNamespace);
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
        public static bool TryParse(XDocument doc, out SoapFault11 result, Type[] detailTypes = null)
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
        /// Attempts to parse the specified XML element that represents the 'Fault' node and eventually returns a new instance of the <see cref="SoapFault11"/> class.
        /// </summary>
        /// <param name="faultElement">The SOAP 'Fault' element to parse.</param>
        /// <param name="result">If the operation succeeds, returns a new instance of <see cref="SoapFault11"/>.</param>
        /// <param name="detailTypes">An array of types that can be deserialized as the 'Detail' child element of the 'Fault' element. Can be null.</param>
        /// <returns></returns>
        public static bool TryParse(XElement faultElement, out SoapFault11 result, Type[] detailTypes = null)
        {
            result = null;

            try
            {
                result = (SoapFault11)Parse(faultElement, detailTypes, SoapVersion11TargetNamespace);
            }
            catch
            {
            }

            return result != null;
        }

        #endregion
    }
}
