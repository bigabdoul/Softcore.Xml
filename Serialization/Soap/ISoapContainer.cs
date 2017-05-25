namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Specifies members for classes that implement containable SOAP message parts (Header, Body).
    /// </summary>
    public interface ISoapContainer : ISerializeXml
    {
        /// <summary>
        /// When implemented by a class, gets or sets the contents of the SOAP message header or body. Can be null 
        /// (header only), of type <see cref="ISerializeXmlFragment"/>, a collection of objects, or a single object.
        /// </summary>
        object Content { get; set; }
    }
}