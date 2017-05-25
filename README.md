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

## Contributions

If you are interested in contributing to this project, either by submitting code, suggesting new features, or pointing out areas of improvement, please feel free to join.
