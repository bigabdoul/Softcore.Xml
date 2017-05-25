namespace Softcore.Xml.Serialization
{
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
}
