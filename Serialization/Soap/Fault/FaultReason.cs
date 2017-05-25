using System.Xml.Serialization;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Represents a fault reason. This class cannot be inherited.
    /// </summary>
    [XmlRoot("Reason", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
    public sealed class FaultReason
    {
        private ReasonTextList _textList;

        /// <summary>
        /// Initializes a new instance of the <see cref="FaultReason"/> class.
        /// </summary>
        public FaultReason()
        {
            _textList = new ReasonTextList();
        }

        /// <summary>
        /// Gets or sets a list of <see cref="Items"/> objects.
        /// </summary>
        [XmlElement("Text", Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public ReasonText[] Items { get => _textList.ToArray(); set => _textList.AddRange(value ?? new ReasonText[0]); }
    }
}
