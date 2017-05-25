namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Enumerated values for content parts of a SOAP envelope.
    /// </summary>
    public enum SoapEnvelopePart
    {
        /// <summary>
        /// SOAP envelope header element.
        /// </summary>
        Header,

        /// <summary>
        /// SOAP envelope body element.
        /// </summary>
        Body,

        /// <summary>
        /// SOAP envelope fault element.
        /// </summary>
        Fault,
    }
}
