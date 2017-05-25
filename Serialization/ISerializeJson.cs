namespace Softcore.Xml
{
    #region ISerializeJson
    /// <summary>
    /// Specifies members for classes that implement a JSON serializable object.
    /// </summary>
    public interface ISerializeJson
    {
        /// <summary>
        /// Gets or sets the text encoding to use during serialization.
        /// </summary>
        System.Text.Encoding Encoding { get; set; }

        /// <summary>
        /// When implemented by a class, serializes the current instance and returns a JSON-encoded string.
        /// </summary>
        /// <returns>A JSON-encoded string that represents the serialized object.</returns>
        string SerializeJson();
    }
    #endregion

    /// <summary>
    /// Specifies members for class that implement a JSON-deserializable object. 
    /// </summary>
    /// <typeparam name="T">The type of object to return after deserialization.</typeparam>
    public interface IDeserializeJson<T>
    {
        /// <summary>
        /// When implemented by a class, deserializes an XML string to an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An initialized instance of type <typeparamref name="T"/>, or the default value for that type.</returns>
        T DeserializeJson();
    }
}
