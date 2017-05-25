using System;
using System.Xml;

namespace Softcore.Xml.Serialization.Soap
{
    /// <summary>
    /// Contains information for a server fault.
    /// </summary>
    [Serializable]
    public class ServerFault : SoapContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerFault"/> class
        /// </summary>
        public ServerFault()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerFault"/> class with the specified parameters.
        /// </summary>
        /// <param name="exceptionType">The type of the exception that occurred on the server.</param>
        /// <param name="message">The message that accompanied the exception.</param>
        /// <param name="stackTrace">The stack trace of the thread that threw the exception on the server.</param>
        /// <param name="namespaces">A one-dimensional array of <see cref="XmlQualifiedName"/> objects.</param>
        public ServerFault(string exceptionType, string message, string stackTrace, params XmlQualifiedName[] namespaces)
        {
            ExceptionType = exceptionType;
            ExceptionMessage = message;
            StackTrace = stackTrace;
            SetNamespaces(namespaces);
        }

        /// <summary>
        /// Gets or sets the type of exception that was thrown by the server.
        /// </summary>
        /// <returns>The type of exception that was thrown by the server.</returns>
        public virtual string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the exception message that accompanied the exception thrown on the server.
        /// </summary>
        /// <returns>The exception message that accompanied the exception thrown on the server.</returns>
        public virtual string ExceptionMessage { get; set; }

        /// <summary>
        /// Gets or sets the stack trace of the thread that threw the exception on the server.
        /// </summary>
        /// <returns>The stack trace of the thread that threw the exception on the server.</returns>
        public virtual string StackTrace { get; set; }
    }
}
