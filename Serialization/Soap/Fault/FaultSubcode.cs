using System.Xml.Schema;
using System.Xml.Serialization;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Represents a SOAP message fault sub code. This class cannot be inherited.
    /// </summary>
    [XmlRoot("Subcode", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    public sealed class FaultSubcode
    {
        /// <summary>
        /// Gets or sets the name of the fault sub code.
        /// </summary>
        [XmlElement("Value", Form = XmlSchemaForm.Qualified)]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the child fault sub code for this <see cref="FaultSubcode"/>.
        /// </summary>
        [XmlElement("Subcode", Form = XmlSchemaForm.Qualified)]
        public FaultSubcode Subcode { get; set; }

        /// <summary>
        /// Returns the string representation of this <see cref="FaultSubcode"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
}
