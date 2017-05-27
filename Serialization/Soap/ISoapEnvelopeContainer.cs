using System;
using System.Collections.Generic;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Marker interface for types that can be converted to a new instance of the <see cref="SoapEnvelope"/> class.
    /// </summary>
    public interface ISoapEnvelopeContainer
    {
        /// <summary>
        /// Attempts to parse the specified XML document and returns a value that indicates whether the operation succeeds.
        /// </summary>
        /// <param name="xml">A string that contains SOAP-XML.</param>
        /// <returns>true if the specified XML document has been parsed; otherwise, false.</returns>
        bool TryParse(string xml);

        /// <summary>
        /// Gets or sets the SOAP target namespace to use when parsing an XML document.
        /// </summary>
        string TargetNamespace { get; set; }
    }
}
