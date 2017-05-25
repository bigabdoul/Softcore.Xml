namespace Softcore.Xml.Serialization
{
    /// <summary>
    /// Specifies members for classes that implement XML deserializable objects.
    /// </summary>
    /// <typeparam name="T">The type of object to return after deserialization.</typeparam>
    public interface IDeserialize<T> : IDeserializeXml<T>
    {
    }
}