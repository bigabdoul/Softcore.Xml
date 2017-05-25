using System.Xml.Serialization;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Represents a descriptive localized text for a SOAP message fault. This class cannot be inherited.
    /// </summary>
    [XmlRoot("Text", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    public sealed class ReasonText
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReasonText"/> class.
        /// </summary>
        public ReasonText() : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReasonText"/> class.
        /// </summary>
        /// <param name="value">The text of the new <see cref="ReasonText"/> instance.</param>
        /// <param name="lang">The language of the new <see cref="ReasonText"/> instance. If null, the current culture name is used.</param>
        public ReasonText(string value, string lang = null)
        {
            Value = value;
            Lang = lang ?? System.Globalization.CultureInfo.CurrentCulture.Name.ToLowerInvariant();
        }

        /// <summary>
        /// The language of this <see cref="ReasonText"/>.
        /// </summary>
        [XmlAttribute("xml:lang")]
        public string Lang { get; set; }

        /// <summary>
        /// The value of this <see cref="ReasonText"/>.
        /// </summary>
        [XmlText]
        public string Value { get; set; }

        /// <summary>
        /// Returns the value of this <see cref="ReasonText"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var ns = SoapContainer.TargetNamespaceLocalNameDefault;
            return string.Format($@"<{ns}:Text xml:lang=""{Lang}"">{Value}</{ns}:Text>");
        }

        /// <summary>
        /// Converts the specified <see cref="ReasonText"/> object to a string.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        public static implicit operator string(ReasonText obj) => obj?.ToString();

        /// <summary>
        /// Converts the specified string to an instance of the <see cref="ReasonText"/> class.
        /// </summary>
        /// <param name="obj">The string to convert.</param>
        public static implicit operator ReasonText(string obj) => obj == null ? null : new ReasonText(obj);
    }
}
