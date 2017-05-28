using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Softcore.Xml.Serialization
{
    /// <summary>
    /// Represents the ultimate base class for XML serializable classes.
    /// </summary>
    [Serializable]
    public abstract class SerializeBase : ISerialize
    {
        #region constructors

        /// <summary>
        /// Initializes an instance of the <see cref="SerializeBase"/> class.
        /// </summary>
        protected SerializeBase()
        {
        }

        #endregion

        #region non-serialized properties

        /// <summary>
        /// Gets or sets the XML serializer namespaces used for serialization.
        /// </summary>
        [SoapIgnore] [XmlIgnore] public virtual XmlSerializerNamespaces Namespaces { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to sort namespaces when they are set.
        /// </summary>
        [SoapIgnore] [XmlIgnore] public virtual bool NamespacesSorted { get; set; }

        /// <summary>
        /// Gets or sets the encoding to use when serializing this instance.
        /// </summary>
        [SoapIgnore] [XmlIgnore] public virtual System.Text.Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets the attributes dictionary.
        /// </summary>
        [SoapIgnore] [XmlIgnore] public virtual IDictionary<string, string> Attributes
        {
            get => (_attributes ?? (_attributes = new Dictionary<string, string>()));
        }
        Dictionary<string, string> _attributes;

        #endregion

        #region serialized properties

        #endregion

        #region public methods

        /// <summary>
        /// Adds an array of <see cref="XmlQualifiedName"/> objects to the namespaces used by the current instance.
        /// </summary>
        /// <param name="args">A one-dimensional array of <see cref="XmlQualifiedName"/> objects.</param>
        public virtual void SetNamespaces(params XmlQualifiedName[] args)
        {
            if (args != null && args.Length > 0)
            {
                if (Namespaces != null)
                {
                    args = MergeNamespaces(Namespaces.ToArray(), args);
                }
                if (NamespacesSorted)
                {
                    var list = args.ToList();
                    list.Sort(NamespaceComparison);
                    Namespaces = new XmlSerializerNamespaces(list.ToArray());
                }
                else
                {
                    Namespaces = new XmlSerializerNamespaces(args);
                }
            }
        }
        
        /// <summary>
        /// Serializes the current instance and returns the XML document as a string.
        /// </summary>
        /// <returns></returns>
        public virtual string SerializeXml()
        {
            return this.XSerialize(Namespaces, Encoding);
        }

        /// <summary>
        /// Serializes the current instance as an XML fragment using the specified namespaces.
        /// </summary>
        /// <param name="namespaces">The namespaces referenced by the current instance.</param>
        /// <returns>A string that represents the XML-fragment produced by the current instance.</returns>
        public virtual string SerializeXmlFragment(params XmlQualifiedName[] namespaces)
        {
            SetNamespaces(namespaces);
            return this.XSerializeFragment(Namespaces, Encoding);
        }

        #endregion

        #region protected methods

        /// <summary>
        /// Merges the XML qualified name with the provided array.
        /// </summary>
        /// <param name="first">The XML qualified name to merge with the array.</param>
        /// <param name="second">The array to merge with <paramref name="first"/>.</param>
        /// <returns></returns>
        protected virtual XmlQualifiedName[] MergeNamespaces(XmlQualifiedName first, XmlQualifiedName[] second)
        {
            return MergeNamespaces(new[] { first }, second);
        }

        /// <summary>
        /// Merges two collections of <see cref="XmlQualifiedName"/> objects.
        /// </summary>
        /// <param name="first">The collection to merge with <paramref name="second"/>.</param>
        /// <param name="second">The array to merge with <paramref name="first"/> collection.</param>
        /// <returns></returns>
        protected virtual XmlQualifiedName[] MergeNamespaces(IEnumerable<XmlQualifiedName> first, XmlQualifiedName[] second)
        {
            var list = new List<XmlQualifiedName>();

            if (first != null)
            {
                list.AddRange(first.Where(x => x != null));
            }

            if (second != null && second.Length > 0)
            {
                list.AddRange(second.Where(x => x != null));
            }
            
            return list.Distinct().ToArray();
        }

        /// <summary>
        /// Performs a comparison between two <see cref="XmlQualifiedName"/> objects.
        /// </summary>
        /// <param name="a">The first object to compare with <paramref name="b"/>.</param>
        /// <param name="b">The second object to compare with <paramref name="a"/>.</param>
        /// <returns></returns>
        protected virtual int NamespaceComparison(XmlQualifiedName a, XmlQualifiedName b)

            => $"{a.Name}{a.Namespace}".CompareTo($"{b.Name}{b.Namespace}");

        /// <summary>
        /// Writes and returns the attributes contained in the <see cref="Attributes"/> dictionary.
        /// </summary>
        /// <returns>A string that contains the attribute names and values contained in the current <see cref="Attributes"/> dictionary.</returns>
        protected virtual string GetAttributes()
        {
            if (_attributes == null)
            {
                return string.Empty;
            }

            return GetAttributes(_attributes);
        }

        /// <summary>
        /// Merges the current <see cref="Attributes"/> dictionary into the <paramref name="target"/> attributes.
        /// </summary>
        /// <param name="target">The destination dictionary of the merge operation.</param>
        protected virtual void MergeAttributes(IDictionary<string, string> target)
        {
            MergeAttributes(Attributes, target);
        }

        #endregion

        #region static methods

        /// <summary>
        /// Merges <paramref name="source"/> attributes into <paramref name="target"/> attributes.
        /// </summary>
        /// <param name="source">The dictionary to merge into the <paramref name="target"/> dictionary.</param>
        /// <param name="target">The destination dictionary of the merge operation.</param>
        public static void MergeAttributes(IDictionary<string, string> source, IDictionary<string, string> target)
        {
            foreach (var key in source.Keys)
            {
                target[key] = source[key];
            }
        }

        /// <summary>
        /// Writes and returns the attributes contained in the specified <paramref name="attributes"/> dictionary.
        /// </summary>
        /// <param name="attributes">A collection of attributes to write using an instance of the <see cref="XmlTextWriter"/> class.</param>
        /// <returns>A string that contains the attribute names and values contained in the specified <paramref name="attributes"/> dictionary.</returns>
        public static string GetAttributes(IDictionary<string, string> attributes)
        {
            var sb = new System.Text.StringBuilder();

            using (var sr = new System.IO.StringWriter(sb))
            {
                using (var xw = new XmlTextWriter(sr))
                {
                    WriteAttributes(xw, attributes);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Writes the attributes contained in the specified <paramref name="attributes"/> dictionary.
        /// </summary>
        /// <param name="writer">The XML writer used to write.</param>
        /// <param name="attributes">A dictionary of key/value pairs to write as key="value".</param>
        public static void WriteAttributes(XmlWriter writer, IDictionary<string, string> attributes)
        {
            foreach (var key in attributes.Keys)
            {
                var value = attributes[key];
                writer.WriteRaw($" {key}=\"{value}\"");
            }
        }

        #endregion
    }
}
