using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Represents the base class for SOAP header and body attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public abstract class SoapAttributeBase : Attribute
    {
        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapAttributeBase"/> class,
        /// setting a class' property representing the SOAP header or body contents.
        /// </summary>
        protected SoapAttributeBase()
        {
        }

        #endregion

        #region fields

        const BindingFlags BINDINGS = BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Public;

        private static object attrSyncLocker = new object();
        private static object discoverSyncLocker = new object();

        private static Dictionary<string, Type[]> s_discoveredTypes;
        private static Dictionary<string, SoapAttributeBase[]> s_discoveredAttributes;

        #endregion

        #region virtual

        /// <summary>
        /// Gets or sets the member info to which this attribute is applied.
        /// </summary>
        public virtual MemberInfo MemberInfo { get; protected set; }

        /// <summary>
        /// Returns an object that represents the value of the member of <paramref name="obj"/> to which the custom attribute is applied.
        /// </summary>
        /// <param name="obj">The parent object that contains the member to search for.</param>
        /// <returns>An object.</returns>
        protected virtual object GetMemberValue(object obj)
        {
            var type = obj.GetType();
            var member = type.GetMember(MemberInfo.Name).Single();

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return type.GetField(member.Name, BINDINGS).GetValue(obj);

                case MemberTypes.Property:
                    return type.GetProperty(member.Name, BINDINGS).GetValue(obj);

                default:
                    throw new NotSupportedException("Only fields and properties are supported by this attribute.");
            }
        }

        #endregion

        #region static

        /// <summary>
        /// Returns an array of all custom attributes applied to the specified object.
        /// </summary>
        /// <typeparam name="T">The type of the attributes to search for.</typeparam>
        /// <param name="obj"></param>
        /// <param name="inherit">true to search the specified object's inheritance chain to find the attributes; otherwise, false.</param>
        /// <returns>
        /// An array of custom attributes applied to specified object, or an array with zero 
        /// elements if no attributes assignable to <typeparamref name="T"/> have been applied.
        /// </returns>
        public static T[] GetCustomAttributes<T>(object obj, bool inherit = false) where T : SoapAttributeBase
        {
            lock (attrSyncLocker)
            {
                if (s_discoveredAttributes == null)
                {
                    s_discoveredAttributes = new Dictionary<string, SoapAttributeBase[]>();
                }

                // use attribute and object types as the dictionary key
                var typeNames = GetKeyForTypes(typeof(T), obj.GetType());

                if (!s_discoveredAttributes.ContainsKey(typeNames))
                {
                    var list = new List<T>();
                    foreach (var m in obj.GetType().GetProperties(BINDINGS))
                    {
                        AddAttribute(m);
                    }

                    foreach (var m in obj.GetType().GetFields(BINDINGS))
                    {
                        AddAttribute(m);
                    }

                    s_discoveredAttributes.Add(typeNames, list.ToArray());

                    void AddAttribute(MemberInfo m)
                    {
                        if (m.GetCustomAttributes(inherit).FirstOrDefault() is T value)
                        {
                            value.MemberInfo = m;
                            list.Add(value);
                        }
                    }
                }

                return (T[])s_discoveredAttributes[typeNames];
            }
        }

        /// <summary>
        ///  Retrieves a collection of custom attributes' values of a specified type that are applied to a specified member, and optionally inspects the ancestors of that member.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="obj">The object to inspect.</param>
        /// <param name="inherit">true to inspect the ancestors of element; otherwise, false.</param>
        /// <returns>
        /// A collection of the custom attributes' values that are applied to <paramref name="obj"/> 
        /// and that match <typeparamref name="T"/>, or an empty collection if no such attributes exist.
        /// </returns>
        public static object[] GetCustomAttributeValues<T>(object obj, bool inherit = false) where T : SoapAttributeBase
        {
            var list = new List<object>();
            var attributes = GetCustomAttributes<T>(obj, inherit);

            foreach (var attr in attributes)
            {
                if (attr.GetMemberValue(obj) is object value)
                {
                    list.Add(value);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Dynamically retrieves a collection of types in the specified object to which
        /// custom SOAP attributes of type <typeparamref name="T"/> have been applied.
        /// </summary>
        /// <typeparam name="T">The type of attributes applied to members of the specified object.</typeparam>
        /// <param name="obj"></param>
        /// <param name="inherit">true to search the specified object's inheritance chain to find the attributes; otherwise, false.</param>
        /// <returns></returns>
        public static Type[] GetTypesForCustomAttribute<T>(object obj, bool inherit = false) where T : SoapAttributeBase
        {
            lock (discoverSyncLocker)
            {
                // in order to speed up subsequent access to the requested types, let's store them in a dictionary.

                if (s_discoveredTypes == null)
                {
                    s_discoveredTypes = new Dictionary<string, Type[]>();
                }

                // use attribute and object types as the dictionary key
                var typeNames = GetKeyForTypes(typeof(T), obj.GetType());

                if (!s_discoveredTypes.ContainsKey(typeNames))
                {
                    var list = new List<Type>();

                    foreach (var attr in GetCustomAttributes<T>(obj, inherit))
                    {
                        var memberInfo = attr.MemberInfo;

                        switch (memberInfo.MemberType)
                        {
                            case MemberTypes.Field:
                                list.Add(((FieldInfo)memberInfo).FieldType);
                                break;

                            case MemberTypes.Property:
                                list.Add(((PropertyInfo)memberInfo).PropertyType);
                                break;

                            default:
                                break;
                        }
                    }

                    s_discoveredTypes.Add(typeNames, list.ToArray());
                }

                return s_discoveredTypes[typeNames];
            }
        }

        #region helpers

        private static string GetKeyForTypes(Type type1, Type type2) => $"{type1.FullName}:{type2.FullName}";

        #endregion

        #endregion
    }
}