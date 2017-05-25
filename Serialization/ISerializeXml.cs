namespace Softcore.Xml
{
    #region ISerializeXml
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
    #endregion

    #region IDeserializeXml<T>
    /// <summary>
    /// Specifies members for classes that implement an XML deserializable object.
    /// </summary>
    /// <typeparam name="T">The type of object to return after deserialization.</typeparam>
    public interface IDeserializeXml<T>
    {
        /// <summary>
        /// When implemented by a class, deserializes an XML string to an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An initialized instance of type <typeparamref name="T"/>, or the default value for that type.</returns>
        T DeserializeXml();
    } 
    #endregion
}
