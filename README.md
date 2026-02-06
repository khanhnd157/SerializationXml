# MazeNET.SerializationXml

[![NuGet](https://img.shields.io/nuget/v/MazeNET.SerializationXml.svg)](https://www.nuget.org/packages/MazeNET.SerializationXml/)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-Standard%202.0%20|%202.1%20|%204.8%20|%208%20|%209%20|%2010-blue.svg)](https://dotnet.microsoft.com/)

A lightweight .NET XML serialization library with Clean Architecture, supporting both static facade and dependency injection patterns.

## Installation

```powershell
Install-Package MazeNET.SerializationXml
```

```bash
dotnet add package MazeNET.SerializationXml
```

## Quick Start

```csharp
using MazeNET.SerializationXml;

// Serialize
var xmlDoc = XmlConverter.SerializeObject(myObject);

// Serialize with options
var xmlDoc = XmlConverter.SerializeObject(myObject, b => b
    .RootElement("Root")
    .RemoveDeclaration()
    .RemoveSchema());

// Deserialize
var obj = XmlConverter.DeserializeObject<MyType>(xmlString);
var obj = XmlConverter.DeserializeObject<MyType>(xmlDocument);

// Convert to string
var xml = xmlDoc.ConvertToString();

// File operations
XmlConverter.SaveToFile("data.xml", myObject);
var data = XmlConverter.FileToObject<MyType>("data.xml");
var doc = XmlConverter.LoadXml("data.xml");
```

## Dependency Injection

```csharp
services.AddSingleton<IXmlSerializer, XmlSerializerService>();
services.AddSingleton<IXmlFileOperations, XmlFileOperationsService>();
```

```csharp
public class MyService(IXmlSerializer serializer, IXmlFileOperations fileOps)
{
    public void Save(MyData data, string path)
    {
        var doc = serializer.Serialize(data);
        fileOps.SaveToFile(path, doc);
    }

    public MyData Load(string path) => fileOps.LoadFromFile<MyData>(path);
}
```

## Configuration

### XmlOptionsBuilder

| Method | Description |
|---|---|
| `RootElement(string)` | Set root element name |
| `AddDeclaration(XmlDeclarationOptions)` | Add XML declaration |
| `RemoveDeclaration(bool)` | Remove XML declaration |
| `RemoveSchema(bool)` | Remove XML schema |
| `RemoveTagCDDATA(bool)` | Remove CDATA tags |
| `AddPrefix(string)` | Add XML prefix |

### XmlDeclarationOptions

| Property | Default |
|---|---|
| `Version` | `"1.0"` |
| `Encoding` | `UTF-8` |
| `Standalone` | `true` |

## Migration from v1.x

```csharp
// v1.x
using CodeMazeNET.Serialization.Xml;

// v2.x
using MazeNET.SerializationXml;
```

API remains unchanged — only the namespace has changed.

## Architecture

```
MazeNET.SerializationXml/
├── Core/
│   ├── Interfaces/        # IXmlSerializer, IXmlFileOperations, IXmlDocumentConverter
│   └── Options/           # XmlOptions, XmlDeclarationOptions, XmlOptionsBuilder
├── Infrastructure/
│   ├── Converters/        # XmlSerializerService, XmlFileOperationsService
│   └── Extensions/        # XmlExtensions
└── XmlConverter.cs        # Static facade
```

## License

[MIT](LICENSE)
