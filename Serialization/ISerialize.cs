namespace Softcore.Xml
{
    /// <summary>
    /// Specifies members for classes that implement both JSON and XML serializable objects.
    /// </summary>
    public interface ISerialize : ISerializeJson, ISerializeXml, ISerializeXmlFragment
    {
    }

    /// <summary>
    /// Specifies members for classes that implement both JSON and XML deserializable objects.
    /// </summary>
    /// <typeparam name="T">The type of object to return after deserialization.</typeparam>
    public interface IDeserialize<T> : IDeserializeJson<T>, IDeserializeXml<T>//, IDeserializeXmlFragment<T>
    {
    }
}