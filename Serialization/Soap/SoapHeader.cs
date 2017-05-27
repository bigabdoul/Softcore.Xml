using System;
using System.Xml.Linq;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Represents a SOAP message header wrapper. This class cannot be inherited.
    /// </summary>
    [System.Serializable]
    public sealed class SoapHeader : SoapContainer
    {
        /// <summary>
        /// Initializes an instance of the <see cref="SoapHeader"/> class.
        /// </summary>
        public SoapHeader()
        {
        }

        /// <summary>
        /// Initializes an instance of the <see cref="SoapHeader"/> class with the provided parameters.
        /// </summary>
        /// <param name="content">
        /// The contents of the SOAP header. Can be null, of type <see cref="ISerializeXmlFragment"/>, 
        /// a collection of objects, or a single object.
        /// </param>
        public SoapHeader(object content) : base(content)
        {
        }
        
        /// <summary>
        /// Serializes this <see cref="SoapHeader"/> instance.
        /// </summary>
        /// <returns>An XML string that represents the current <see cref="SoapHeader"/> instance.</returns>
        public override string SerializeXml() => EncloseInElement("Header", SerializeContents(Content));

        #region helpers

        /// <summary>
        /// Parses the specified XML document to an instance of <see cref="SoapHeader"/> by parsing only the SOAP envelope header.
        /// </summary>
        /// <typeparam name="TContent">The type of the header content.</typeparam>
        /// <param name="doc">The XML document containing the SOAP message header to extract.</param>
        /// <param name="throwIfHeaderMissing">
        /// true to throw <see cref="InvalidOperationException"/> if the document doesn't contain 
        /// a SOAP header element; false to ignore missing body. The default value is false.
        /// </param>
        /// <param name="targetNamespace">The SOAP target namespace to use. If null, the <see cref="SoapContainer.DefaultTargetNamespace"/> property value is used.</param>
        /// <returns>An initialized instance of <see cref="SoapHeader"/>, or null.</returns>
        public static SoapHeader Parse<TContent>(XDocument doc, bool throwIfHeaderMissing = false, string targetNamespace = null) where TContent : class, new()
            
            => Parse(doc, new[] { typeof(TContent) }, throwIfHeaderMissing, targetNamespace);

        /// <summary>
        /// Parses the specified XML document to an instance of <see cref="SoapBody"/> by parsing only the SOAP envelope header.
        /// </summary>
        /// <param name="doc">The XML document containing the SOAP message header to extract.</param>
        /// <param name="types">An array of types that the Header element in <paramref name="doc"/> can deserialize to.</param>
        /// <param name="throwIfHeaderMissing">
        /// true to throw <see cref="InvalidOperationException"/> if the document doesn't contain 
        /// a SOAP header element; false to ignore missing header. The default value is false.
        /// </param>
        /// <param name="targetNamespace">The SOAP target namespace to use. If null, the <see cref="SoapContainer.DefaultTargetNamespace"/> property value is used.</param>
        /// <returns>An initialized instance of <see cref="SoapHeader"/>, or null.</returns>
        public static SoapHeader Parse(XDocument doc, Type[] types, bool throwIfHeaderMissing = false, string targetNamespace = null)
        {
            targetNamespace = targetNamespace ?? DefaultTargetNamespace;

            if (doc.TryFindXElement("Header", out var element, targetNamespace) && ParseContent(element, types) is object content)
            {
                return new SoapHeader(content) { TargetNamespace = targetNamespace };
            }

            if (throwIfHeaderMissing)
            {
                throw new InvalidOperationException("The specified XML must contain a SOAP envelope header.");
            }

            return null;
        }

        #endregion
    }
}
