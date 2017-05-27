using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Represents the base class for the <see cref="SoapFault"/> and <see cref="SoapFault11"/> classes.
    /// </summary>
    [XmlRoot("Fault")]
    public abstract class SoapFaultBase : SoapContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoapFaultBase"/> class.
        /// </summary>
        protected SoapFaultBase()
        {
        }

        /// <summary>
        /// Gets or sets additional information required for the <see cref="SoapFaultBase"/>.
        /// </summary>
        /// <remarks>
        /// This property is decorated with <see cref="XmlIgnoreAttribute"/> because we don't know its type at this moment.
        /// Therefore, it must be serialized as a separate XML fragment and appended as the last child of the 'Fault' element.
        /// This is done in the <see cref="SerializeDetail(string)"/> method.
        /// </remarks>
        [XmlIgnore] public virtual object Detail { get; set; }

        /// <summary>
        /// Parses the specified XML element that represents the 'Fault' node.
        /// </summary>
        /// <param name="faultElement">The SOAP 'Fault' element to parse.</param>
        /// <param name="detailTypes">An array of types that can be deserialized as the 'Detail' child element of the 'Fault' element. Can be null.</param>
        /// <param name="targetNamespace">The SOAP target namespace to use. If null, the <see cref="SoapContainer.DefaultTargetNamespace"/> property value is used.</param>
        /// <returns></returns>        
        public static SoapFaultBase Parse(XElement faultElement, Type[] detailTypes = null, string targetNamespace = null)
        {
            targetNamespace = targetNamespace ?? DefaultTargetNamespace;

            var version12 = IsProtocolVersion12(targetNamespace);
            string tns, elementName;

            if (version12)
            {
                elementName = "Detail";
                tns = SoapVersion12TargetNamespace;
            }
            else
            {
                elementName = "detail";
                tns = null; // SOAP 1.1 fault subelements have unqualified xml schema form
            }

            if (// try to find the 'Detail' of the 'Fault' element
                faultElement.TryFindXElement(elementName, out var xdetail, tns) &&

                // and then parse that element using the specified types, or the default ServerFault type
                ParseContent(xdetail, detailTypes ?? new[] { typeof(ServerFault) }) is object content
            )
            {
                // '[Dd]etail' element was found and parsed into an object instance; remove it now
                // so that we can add it as the detail content of the parsed original 'faultElement'
                xdetail.Remove();
            }
            else
            {
                // variable must be initialized
                content = null;
            }

            SoapFaultBase sfb;

            if (version12)
            {
                sfb = faultElement.ToString().XDeserialize<SoapFault>(true).SetDetail(content);
            }
            else
            {
                sfb = faultElement.ToString().XDeserialize<SoapFault11>(true).SetDetail(content);
            }

            sfb.TargetNamespace = targetNamespace;
            return sfb;
        }

        #region helpers

        /// <summary>
        /// Serializes the <see cref="Detail"/> property value as an XML fragment 
        /// and appends it as the last child of <paramref name="soapFaultXml"/>.
        /// </summary>
        /// <param name="soapFaultXml">The serialized <see cref="SoapFault"/> XML fragment.</param>
        /// <returns></returns>
        protected virtual string SerializeDetail(string soapFaultXml)
        {
            var detail = Detail;

            if (detail == null) return soapFaultXml;

            // TODO: figure out a better way to handle the Detail property.
            // This just looks and feels like a hack: insert the 'Detail' element as the last child of 'Fault'; works at least :-)
            var tns = GetTargetNamespacePrefix();

            ParseFragment(soapFaultXml, out XElement elmFault, tns);

            string xmlDetail;

            if (detail is ISerializeXmlFragment serializable)
            {
                // maybe it has its own way of serializing?
                serializable.Encoding = serializable.Encoding ?? Encoding;
                xmlDetail = serializable.SerializeXmlFragment();
            }
            else
            {
                xmlDetail = detail.XSerializeFragment(enc: Encoding);
            }

            var frag = EncloseInElement(IsVersion12 ? "Detail" : "detail", xmlDetail, tns);

            ParseFragment(frag, out XElement elmDetail, tns);
            elmFault.LastNode.AddAfterSelf(elmDetail);

            return elmFault.ToString();
        }

        /// <summary>
        /// Wraps the specified XML element name, using its qualified name (the target namespace prefix), around the provided value.
        /// </summary>
        /// <param name="name">The XML element name to wrap around <paramref name="xml"/>.</param>
        /// <param name="xml">The XML string to enclose in <paramref name="name"/> element.</param>
        /// <param name="tns">The SOAP target namespace prefix to use. If SOAP version 1.1, it's simply ignored.</param>
        /// <returns></returns>
        protected override string EncloseInElement(string name, string xml, string tns = null)
        {
            if (IsVersion11)
            {
                return $@"<{name}>{xml}</{name}>";
            }

            return base.EncloseInElement(name, xml, tns);
        }

        /// <summary>
        /// Adds an array of <see cref="XmlQualifiedName"/> objects to the namespaces used by the current instance.
        /// </summary>
        /// <param name="args">A one-dimensional array of <see cref="XmlQualifiedName"/> objects.</param>
        public override void SetNamespaces(params XmlQualifiedName[] args)
        {
            base.SetNamespaces(MergeNamespaces(new XmlQualifiedName(TargetNamespacePrefixDefault, GetTargetNamespace()), args));
        }

        #endregion
    }
}
