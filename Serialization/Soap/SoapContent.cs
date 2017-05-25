namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Represents a wrapper for the <see cref="SoapContainer.Content"/> property to make it serializable.
    /// </summary>
    [System.Serializable]
    public class SoapContent : SoapContainer
    {
        /// <summary>
        /// Gets or sets the contents of the SOAP message header or body. Can be null (header only), 
        /// of type <see cref="ISerializeXmlFragment"/>, a collection of objects, or a single object.
        /// </summary>
        [System.Xml.Serialization.XmlElement(Namespace = "")]
        public override object Content { get => base.Content; set => base.Content = value; }
    }
}