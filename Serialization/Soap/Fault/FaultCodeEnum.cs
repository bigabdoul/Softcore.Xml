namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Enumerations for a SOAP message fault code.
    /// </summary>
    public enum FaultCodeEnum
    {
        /// <summary>
        /// Data encoding unknown.
        /// </summary>
        DataEncodingUnknown,

        /// <summary>
        /// Must understand.
        /// </summary>
        MustUnderstand,

        /// <summary>
        /// Receiver.
        /// </summary>
        Receiver,

        /// <summary>
        /// Sender.
        /// </summary>
        Sender,

        /// <summary>
        /// Version mismatch.
        /// </summary>
        VersionMismatch
    }
}
