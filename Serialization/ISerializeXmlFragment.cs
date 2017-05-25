using System.Xml;

namespace Softcore.Xml
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

    ///// <summary>
    ///// Specifies members for classes that implement an XML fragment deserializable object.
    ///// </summary>
    ///// <typeparam name="T">The type of object to return after deserialization.</typeparam>
    //public interface IDeserializeXmlFragment<T>
    //{
    //    /// <summary>
    //    /// When implemented by a class, deerializes an XML fragment string to an instance of type <typeparamref name="T"/>.
    //    /// </summary>
    //    /// <returns>An initialized instance of type <typeparamref name="T"/>, or the default value for that type.</returns>
    //    T DeserializeXmlFragment();
    //}
}