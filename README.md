# softcore.xml
Custom XML Serialization library for .NET with a special emphasis on SOAP-serializable objects and the brand new C# 7 syntax.

## Introduction

I'm currently working on real-world project where I need to communicate with a third-party service. It happens that XML, and SOAP 
in particuliar, is their preferred method of exchanging information. So I started implementing their APIs' specification from a document
that I've been given. Very soon I came to realize that serializing and deserializing the objects to and from SOAP is not an easy task.

It came as a shock to me when I used the built-in .NET SOAP formatter (System.Runtime.Serialization.Formatters.Soap.SoapFormatter) when 
it was about time to do some testing. The output was simply horrifying.

After a lot of googling and diving deeply into StackOverflow, I haven't found exactly what I needed. That's why I ended up writing my
own logic to handle the dehydration (serialization) and hydration (deserialization) processes bearing in mind simplicity, flexibility,
and reusability. So here I am, trying to give it back to the community and my fellow developers like you to make our lives easier.

## Purpose

Serialize .NET objects into XML and above all SOAP formats exactly the way you want to, and deserialize them easily back into objects.

## Getting started

Here's a quick example of how to use this library. Suppose you have the following SOAP file (found on StackOverflow):

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

Let's suppose you have the equivalent business objects defined like that:

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

You can do that by creating a test project in Visual Studio with the following SampleTests.cs file and this simple Should_Deserialize_To_Object() method:

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

This is just a quick use case of how to deserialize a SOAP envelope into custom business objects. More will be coming soon in the wiki or some documentation for the library.

## Contributions

If you are interested in contributing to this project, either by submitting code, suggesting new features, or pointing out areas of improvement, please feel free to join.
