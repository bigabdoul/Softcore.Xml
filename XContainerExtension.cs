using System.Linq;
using System.Xml.Linq;

namespace Softcore.Xml
{
    /// <summary>
    /// Provides extension methods for an instance of a class derived from the <see cref="XContainer"/> class.
    /// </summary>
    public static class XContainerExtension
    {
        /// <summary>
        /// Attempts to retrieve an XML element using the specified XML namespace. If the first attempt 
        /// fails, another attempt with an appended forward-slash / to the namespace will be made.
        /// </summary>
        /// <param name="container">The XML document to search.</param>
        /// <param name="name">The name of the XML element to find.</param>
        /// <param name="firstElement">Returns the found XML element, if any.</param>
        /// <param name="xmlns">The XML namespace of the element to find. Can be null or empty.</param>
        /// <returns>true if <paramref name="firstElement"/>is not null; otherwise, false.</returns>
        public static bool TryFindXElement(this XContainer container, string name, out XElement firstElement, string xmlns = null)
        {
            if (string.IsNullOrWhiteSpace(xmlns))
            {
                firstElement = container.Descendants($"{name}").FirstOrDefault();
            }
            else
            {
                xmlns = xmlns.TrimEnd('/');

                firstElement =
                    container.Descendants($"{{{xmlns}}}{name}").FirstOrDefault() ??

                    // append / (forward-slash) to the xml namespace and try again
                    container.Descendants($"{{{xmlns}/}}{name}").FirstOrDefault();
            }

            return firstElement != null;
        }
    }
}
