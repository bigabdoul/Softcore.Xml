namespace Softcore.Xml.Serialization
{
    /// <summary>
    /// Specifies members for classes that implement an XML serializable object.
    /// </summary>
    public interface ISerializeXml
    {
        /// <summary>
        /// Gets or sets the text encoding to use during serialization.
        /// </summary>
        System.Text.Encoding Encoding { get; set; }

        /// <summary>
        /// When implemented by a class, serializes the current instance and returns the XML document as a string.
        /// </summary>
        /// <returns>An XML string that represents the serialized object.</returns>
        string SerializeXml();
    }
}
