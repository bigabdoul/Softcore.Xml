using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Represents a SOAP message fault code. This class cannot be inherited.
    /// </summary>
    [XmlRoot("Code", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    public sealed class FaultCode
    {
        /// <summary>
        /// Gets or sets the enumerated fault code value.
        /// </summary>
        [XmlIgnore]
        public FaultCodeEnum Value { get; set; }

        /// <summary>
        /// Gets or sets the serialized fault code value.
        /// </summary>
        [XmlElement("Value", Form = XmlSchemaForm.Qualified)]
        public string FaultCodeValue
        {
            get => ToString();
            set
            {
                if (value?.Split(':') is string[] parts)
                {
                    var val = parts.Length == 2 ? parts[1] : parts[0];

                    if (Enum.TryParse(val, true, out FaultCodeEnum result))
                    {
                        Value = result;
                        return;
                    }
                }

                throw new ArgumentException($"{nameof(value)} cannot be converted to the FaultCodeEnum enumeration.");
            }
        }

        /// <summary>
        /// Gets or sets the fault sub code.
        /// </summary>
        [XmlElement("Subcode", Form = XmlSchemaForm.Qualified)]
        public FaultSubcode Subcode { get; set; }

        /// <summary>
        /// Returns the string representation of this <see cref="FaultCode"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", SoapContainer.TargetNamespaceLocalNameDefault, Value);
        }
    }
}
