using System;
using System.Collections.Generic;
using System.Linq;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Represents the base class for types that can be converted to a new instance of the <see cref="SoapEnvelope"/> class.
    /// </summary>
    public abstract class SoapEnvelopeContainer : ISoapEnvelopeContainer
    {
        #region constructors

        /// <summary>
        /// Initializes an instance of the <see cref="SoapEnvelopeContainer"/> class.
        /// </summary>
        protected SoapEnvelopeContainer()
        {
        }

        #endregion

        #region abstract and virtual properties

        /// <summary>
        /// Returns an array of types expected in the SOAP header element.
        /// </summary>
        /// <returns></returns>
        public abstract Type[] HeaderTypesHint { get; }

        /// <summary>
        /// Returns a collection of types expected in the SOAP body element.
        /// </summary>
        /// <returns></returns>
        public abstract Type[] BodyTypesHint { get; }

        /// <summary>
        /// Returns an array of types expected in the SOAP fault detail element.
        /// </summary>
        public abstract Type[] FaultTypesHint { get; }

        /// <summary>
        /// Gets or sets the header contents.
        /// </summary>
        public virtual object[] HeaderContent { get; set; }

        /// <summary>
        /// Gets or sets the body contents.
        /// </summary>
        public virtual object[] BodyContent { get; set; }

        /// <summary>
        /// Gets or sets the fault detail contents.
        /// </summary>
        public virtual object[] FaultDetailContent { get; set; }

        /// <summary>
        /// Gets the error that occured in the method <see cref="TryParse(string)"/>.
        /// </summary>
        public virtual Exception ParseError { get; private set; }

        /// <summary>
        /// Gets the parsed <see cref="SoapEnvelope"/>.
        /// </summary>
        public virtual SoapEnvelope Envelope => _envelope;
        private SoapEnvelope _envelope;

        #endregion

        #region ISoapEnvelopeContainer implementation

        /// <summary>
        /// Attempts to parse the specified XML document and returns a value that indicates whether the operation succeeds.
        /// </summary>
        /// <param name="xml">A string that contains SOAP-XML.</param>
        /// <returns>true if the specified XML document has been parsed; otherwise, false.</returns>
        public virtual bool TryParse(string xml)
        {
            try
            {
                _envelope = null;

                if (SoapEnvelope.Parse(xml, part => GetTypesHintForPart(part)) is SoapEnvelope env)
                {
                    _envelope = env;

                    HeaderContent = GetContentArray(env.Header?.Content);
                    BodyContent = GetContentArray(env.Body?.Content);
                    FaultDetailContent = GetContentArray(env.Body?.Fault?.Content);

                    return true;
                }
            }
            catch (Exception ex)
            {
                ParseError = ex;
            }

            return false;
        }

        #endregion

        #region helpers

        /// <summary>
        /// Dynamically retrieves a collection of types to which custom attributes of type <typeparamref name="T"/> have been applied.
        /// </summary>
        /// <typeparam name="T">The type of attributes applied to members of this <see cref="SoapEnvelopeContainer"/>.</typeparam>
        /// <param name="inherit">true to search the current object's inheritance chain to find the attributes; otherwise, false.</param>
        /// <returns></returns>
        public virtual Type[] GetTypesForCustomAttribute<T>(bool inherit = false) where T : SoapAttributeBase
        {
            return SoapAttributeBase.GetTypesForCustomAttribute<T>(this, inherit);
        }

        /// <summary>
        /// Returns an array of types appropriate for the specified <see cref="SoapEnvelopePart"/> <paramref name="part"/>.
        /// </summary>
        /// <param name="part">The part of a SOAP envelope element for which to return types suitable for deserialization.</param>
        /// <returns></returns>
        protected virtual Type[] GetTypesHintForPart(SoapEnvelopePart part)
        {
            switch (part)
            {
                case SoapEnvelopePart.Header:
                    return HeaderTypesHint;

                case SoapEnvelopePart.Body:
                    return BodyTypesHint;

                case SoapEnvelopePart.Fault:
                    return FaultTypesHint;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts the specified object to an array of objects.
        /// </summary>
        /// <param name="content">The object to convert.</param>
        /// <returns>An array of objects, or null if <paramref name="content"/> is null.</returns>
        protected static object[] GetContentArray(object content)
        {
            if (content == null) return null;
            if (content is IEnumerable<object> faults) return faults.ToArray();
            return new object[] { content };
        }

        #endregion
    }
}
