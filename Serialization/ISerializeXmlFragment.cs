using System.Xml;

namespace Softcore.Xml.Serialization
{
    /// <summary>
    /// Specifies members for serializable classes that produce XML fragments.
    /// </summary>
    public interface ISerializeXmlFragment
    {
        /// <summary>
        /// Gets or sets the text encoding to use during serialization.
        /// </summary>
        System.Text.Encoding Encoding { get; set; }

        /// <summary>
        /// When implemented by a class, serializes the current instance as an XML fragment using the specified namespaces.
        /// </summary>
        /// <param name="namespaces">The namespaces referenced by the current instance.</param>
        /// <returns>A string that represents the XML-fragment produced by the current instance.</returns>
        string SerializeXmlFragment(params XmlQualifiedName[] namespaces);
    }
}