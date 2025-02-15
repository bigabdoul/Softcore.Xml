# softcore.xml  
A Custom XML Serialization Library for .NET with a Focus on SOAP and Modern C# 7 Syntax  

## Introduction  
While working on a real-world project, I needed to integrate with a third-party service that relies heavily on XML—specifically SOAP—for data exchange. To accomplish this, I began implementing their API based on the provided documentation. However, I quickly realized that serializing and deserializing objects to and from SOAP was far from straightforward.  

To my surprise, when I tested the built-in .NET SOAP formatter (`System.Runtime.Serialization.Formatters.Soap.SoapFormatter`), the output was simply horrifying—far from what I expected.  

After extensive research and countless Stack Overflow threads, I couldn't find a solution that met my needs. So, I decided to create my own serialization and deserialization logic, prioritizing simplicity, flexibility, and reusability. Now, I’m sharing this library with the developer community to make SOAP serialization easier for everyone.  

## Purpose  
softcore.xml enables seamless serialization of .NET objects into XML, with a strong emphasis on SOAP formatting, giving you full control over the output. It also simplifies deserialization, ensuring that objects are accurately reconstructed.  

## Getting Started  
Here's a quick example of how to use this library. Suppose you have the following SOAP file (originally found on Stack Overflow):  

```
<?xml version="1.0" encoding="utf-8"?>
  <SOAP-ENV:Envelope xmlns:SOAPSDK1="http://www.w3.org/2001/XMLSchema" xmlns:SOAPSDK2="http://www.w3.org/2001/XMLSchema-instance" xmlns:SOAPSDK3="http://schemas.xmlsoap.org/soap/encoding/" xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/">
  <SOAP-ENV:Body SOAP-ENV:encodingStyle="http://schemas.xmlsoap.org/soap/encoding/">
    <SOAPSDK4:GetStoreInformationResponse xmlns:SOAPSDK4="http://www.example.com/message/">
      <StoreInformation>
        <StoreID>99612</StoreID>
        <BusinessDate>2016-01-28</BusinessDate>
        <Address type="Address-US">
          <Street>Via Roma 1</Street>
          <City>Milano</City>
        </Address>
      </StoreInformation>
    </SOAPSDK4:GetStoreInformationResponse>
  </SOAP-ENV:Body>
</SOAP-ENV:Envelope>
```

Let's assume you have the corresponding business objects defined as follows:

```
using Softcore.Xml.Serialization.Soap;
using System;
using System.Xml.Serialization;

namespace MyNamespace.Models
{
    [XmlRoot("GetStoreInformationResponse", Namespace = "http://www.example.com/message/")]
    public class StoreInformationResponse : SoapContainer
    {
        [XmlElement(Namespace = "")]
        public StoreInformation StoreInformation { get; set; }
    }

    public class StoreInformation
    {
        public int StoreID { get; set; }
        public DateTime BusinessDate { get; set; }
        [XmlElement(Namespace = "")]
        public Address Address { get; set; }
    }

    public class Address
    {
        [XmlAttribute("type")]
        public string Type { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
    }
}
```

To test the deserialization, create a test project in Visual Studio and add the following SampleTests.cs file. Inside, implement the simple Should_Deserialize_To_Object() method:

```
using MyNamespace.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Softcore.Xml.Serialization;
using Softcore.Xml.Serialization.Soap;
using System;
using System.Diagnostics;
using System.Xml;

namespace MyNamespace.Tests
{
    [TestClass]
    public class SampleTests
    {
        [TestMethod]
        public void Should_Deserialize_To_Object()
        {
            // ARRANGE
            
            // the xml to deserialize
            var xml = "Put the above SOAP file content here";
            
            // a filter function that returns the expected types in different parts (header, body, or fault) of the envelope
            Func<SoapEnvelopePart, Type[]> get_types = part =>
            {
                switch (part)
                {
                    case SoapEnvelopePart.Header:
                        break;
                    case SoapEnvelopePart.Body:
                        return new[] { typeof(StoreInformationResponse) };

                    case SoapEnvelopePart.Fault:
                        break;
                    default:
                        break;
                }
                return null;
            };

            // don't forget your default soap target namespace
            var tns = SoapContainer.TargetNamespace = SoapContainer.SoapVersion11TargetNamespace;

            // ACT

            // method 1: simplest one might think but it's not gonna work
            var storeInfoResponse1 = xml.XDeserialize<StoreInformationResponse>();

            // method 2: more complex and also much more flexible
            // parse the xml and expect only type 'StoreInformationResponse' in the 'Body' element
            var env = SoapEnvelope.Parse(xml, get_types);
            var response = env.Body.GetContent<StoreInformationResponse>();

            // ASSERT

            // should be null because the XML serializer cannot simply deserialize the stuff without much more info
            Assert.IsNull(storeInfoResponse1);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.StoreInformation);

            Assert.AreEqual(response.StoreInformation.StoreID, 99612);
            Assert.AreEqual(response.StoreInformation.BusinessDate, new DateTime(2016,1,28));

            Assert.IsNotNull(response.StoreInformation.Address);
            Assert.AreEqual(response.StoreInformation.Address.City, "Milano");
            Assert.AreEqual(response.StoreInformation.Address.Street, "Via Roma 1");
            Assert.AreEqual(response.StoreInformation.Address.Type, "Address-US");

            // What follows after assertions is just for the demo and shouldn't be in this test method.
            // If your StoreInformationResponse class inherits Softcore.Xml.Serialization.Soap.SoapContainer, you can do the following:
            if (response is SoapContainer container)
            {
               // serialize just your business object
                Debug.WriteLine(container.SerializeXml());
            }

            // serialize soap envelope with your business object inside
            Debug.WriteLine(env.SerializeXml());

            // now output the envelope with your local target namespace
            //env = SoapEnvelope.Create(storeInfoResponse2); // create a new SoapEnvelope or reset the existing
            env.Namespaces = null;
            env.ExcludeXmlDeclaration = true;
            var prefix = SoapContainer.TargetNamespacePrefixDefault = "SOAP-ENV";
            var encodingNs = SoapContainer.SoapVersion11EncodingNamespace;

            env.SetNamespaces(
                new XmlQualifiedName("SOAPSDK1", "http://www.w3.org/2001/XMLSchema"),
                new XmlQualifiedName("SOAPSDK2", "http://www.w3.org/2001/XMLSchema-instance"),
                new XmlQualifiedName("SOAPSDK3", encodingNs),

                // http://schemas.xmlsoap.org/soap/envelope/ is the SOAP target namespace: it will be
                // removed during serialization because it's already and always present at the envelope level
                new XmlQualifiedName(prefix, tns)
            );

            // http://schemas.xmlsoap.org/soap/encoding/
            env.Body.Attributes[$"{prefix}:encodingStyle"] = encodingNs;

            response.Namespaces = null;
            response.SetNamespaces(new XmlQualifiedName("SOAPSDK4", "http://www.example.com/message/"));

            Debug.WriteLine(env.SerializeXml());
        }
    }
}
```

This is just a basic example of how to deserialize a SOAP envelope into custom business objects. More detailed use cases and documentation will be available soon in the project's wiki.

## Contributions

If you'd like to contribute to this project—whether by submitting code, suggesting new features, or identifying areas for improvement—you're more than welcome to join!
