using System;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Represents a SOAP message body wrapper. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class SoapBody : SoapContainer
    {
        #region constructors

        /// <summary>
        /// Initializes an instance of the <see cref="SoapBody"/> class.
        /// </summary>
        public SoapBody()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapBody"/> class.
        /// </summary>
        /// <param name="fault">The SOAP fault that occured.</param>
        public SoapBody(SoapFault fault)
        {
            Fault = fault;
        }

        /// <summary>
        /// Initializes an instance of the <see cref="SoapBody"/> class.
        /// </summary>
        /// <param name="content">Content of the SOAP message body. Cannot be null during serialization unless a fault occured.</param>
        public SoapBody(object content) : base(content)
        {
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the SOAP message fault. Can be null provided <see cref="SoapContainer.Content"/> is not null.
        /// </summary>
        public SoapFault Fault { get; set; }


        /// <summary>
        /// Gets or sets the encoding to use when serializing this instance.
        /// </summary>
        [SoapIgnore] [XmlIgnore]
        public override Encoding Encoding
        {
            get => base.Encoding;
            set
            {
                base.Encoding = value;
                if (Fault != null)
                {
                    Fault.Encoding = value;
                }
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Serializes this <see cref="SoapBody"/> instance.
        /// </summary>
        /// <returns>An XML string that represents the current <see cref="SoapBody"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Both <see cref="SoapContainer.Content"/> and <see cref="Fault"/> are null.</exception>
        public override string SerializeXml()
        {
            var content = Content;
            var fault = Fault;

            if (content == null && fault == null)
                throw new ArgumentNullException(nameof(Content), $"{nameof(Content)} and {nameof(Fault)} cannot be null simultaneously.");

            var xml = string.Concat(SerializeContents(content), fault?.SerializeXml());
            return EncloseInElement("Body", xml);
        }

        #region fluent api

        /// <summary>
        /// Sets the <see cref="SoapContainer.Content"/> property value to the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The content to set.</param>
        /// <returns>A reference to this <see cref="SoapBody"/> instance.</returns>
        public SoapBody SetContent(params object[] content)
        {
            return SetContent<SoapBody>(content);
        }

        /// <summary>
        /// Sets the <see cref="Fault"/> property value to the specified <paramref name="fault"/>.
        /// </summary>
        /// <param name="fault">The fault to set.</param>
        /// <returns>A reference to this <see cref="SoapBody"/> instance.</returns>
        public SoapBody SetFault(SoapFault fault)
        {
            Fault = fault;
            return this;
        }
        #endregion

        #region helpers

        /// <summary>
        /// Parses the specified XML document to an instance of <see cref="SoapBody"/> by parsing only the SOAP envelope body.
        /// </summary>
        /// <typeparam name="TContent">The type of the body content.</typeparam>
        /// <param name="doc">The XML document containing the SOAP message body to extract.</param>
        /// <param name="faultTypes"></param>
        /// <param name="throwIfMissing">
        /// true to throw <see cref="InvalidOperationException"/> if the document doesn't contain a SOAP body element;
        /// false to ignore missing body. The default value is true.
        /// </param>
        /// <returns>An initialized instance of <see cref="SoapBody"/>.</returns>
        /// <exception cref="InvalidOperationException">The specified XML must contain a SOAP envelope body.</exception>
        public static SoapBody Parse<TContent>(XDocument doc, Type[] faultTypes = null, bool throwIfMissing = true)
            where TContent : class, new()
            
            => Parse(doc, new[] { typeof(TContent) }, faultTypes, throwIfMissing);

        /// <summary>
        /// Parses the specified XML document to an instance of <see cref="SoapBody"/> by parsing only the SOAP envelope body.
        /// </summary>
        /// <param name="doc">The XML document containing the SOAP message body to extract.</param>
        /// <param name="bodyTypes">A function that finds the appropriate types for each child XML element of the body element.</param>
        /// <param name="faultDetailTypes">
        /// An array of types that can be deserialized as the 'Detail' child element of the 'Fault' element. Can be null.
        /// </param>
        /// <param name="throwIfMissing">
        /// true to throw <see cref="InvalidOperationException"/> if the document doesn't contain a SOAP body element;
        /// false to ignore missing body. The default value is true.
        /// </param>
        /// <returns>An initialized instance of <see cref="SoapBody"/>, or null.</returns>
        /// <exception cref="InvalidOperationException">The specified XML must contain a SOAP envelope body.</exception>
        public static SoapBody Parse(XDocument doc, Type[] bodyTypes, Type[] faultDetailTypes = null, bool throwIfMissing = true)
        {
            // get the body, for sure
            if (doc.TryFindXElement("Body", out var element, TargetNamespace))
            {
                var content = ParseContent(element, bodyTypes);
                var fault = SoapFault.Parse(doc, faultDetailTypes);

                if (content != null || fault != null)
                {
                    return new SoapBody(content).SetFault(fault);
                }
            }

            if (throwIfMissing)
            {
                throw new InvalidOperationException("The specified XML must contain a SOAP envelope body.");
            }

            return null;
        }

        #endregion

        #endregion
    }
}
