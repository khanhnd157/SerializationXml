# MazeNET.SerializationXml

[![NuGet](https://img.shields.io/nuget/v/MazeNET.SerializationXml.svg)](https://www.nuget.org/packages/MazeNET.SerializationXml/)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-Standard%202.0%20|%202.1%20|%204.8%20|%208%20|%209%20|%2010-blue.svg)](https://dotnet.microsoft.com/)

## A. Project Overview

**MazeNET.SerializationXml** is a lightweight .NET library for XML serialization, deserialization, file I/O, and XML-to-JSON conversion. Built with Clean Architecture and zero heavy external dependencies.

**Use cases:**

- Serialize/deserialize C# objects to/from XML
- Read and write XML files
- Convert XML to JSON (from string, `XmlDocument`, or file)
- Integrate with dependency injection in ASP.NET Core / .NET Generic Host

**Target audience:** Backend developers, desktop app developers, and teams working with legacy XML-based systems or APIs.

## B. Features

- XML serialization and deserialization (object ↔ `XmlDocument` ↔ string ↔ file)
- XML-to-JSON conversion with configurable options (zero external dependencies)
- XML-to-Object mapping (`ConvertTo<T>`) — unmatched properties remain null/default
- Typed JSON conversion (`XmlToJson<T>`) — XML → T → JSON with only T's properties
- Fluent builder for XML output options (root element, declaration, schema, CDATA)
- Async methods with `CancellationToken` support for all file operations
- Extension methods on `XmlDocument` (`.ConvertToString()`, `.Builder()`, `.ToJson()`, `.ToJson<T>()`, `.ToObject<T>()`)
- `IServiceCollection.AddSerializationXml()` for dependency injection
- Custom `XmlSerializationException` with `TargetType` and `Operation` context
- Nullable reference types enabled
- Multi-targeting: `.NET Standard 2.0`, `.NET Standard 2.1`, `.NET Framework 4.8`, `.NET 8`, `.NET 9`, `.NET 10`

## C. Tech Stack

| Component | Detail |
|---|---|
| Language | C# (LangVersion: latest) |
| Targets | `netstandard2.0`, `netstandard2.1`, `net48`, `net8.0`, `net9.0`, `net10.0` |
| Dependencies | `Microsoft.Extensions.DependencyInjection.Abstractions 8.0.2` |
| XML engine | `System.Xml` (`XmlDocument`, `XmlSerializer`) |
| JSON engine | Built-in (no Newtonsoft.Json or System.Text.Json dependency) |

This library does NOT require database or migration.

## D. Installation

**NuGet:** https://www.nuget.org/packages/MazeNET.SerializationXml/

**.NET CLI:**

```bash
dotnet add package MazeNET.SerializationXml
```

**Package Manager:**

```powershell
Install-Package MazeNET.SerializationXml
```

**PackageReference (csproj):**

```xml
<PackageReference Include="MazeNET.SerializationXml" Version="2.1.0" />
```

## E. Quick Start

```csharp
using MazeNET.SerializationXml;
using MazeNET.SerializationXml.Options;

public class Invoice
{
    public int Id { get; set; }
    public string Customer { get; set; }
    public decimal Amount { get; set; }
}

// Serialize object → XmlDocument
var invoice = new Invoice { Id = 1, Customer = "John", Amount = 100m };
var xmlDoc = XmlConverter.SerializeObject(invoice);

// Serialize with options
var xmlDoc2 = XmlConverter.SerializeObject(invoice, b => b
    .RootElement("Invoice")
    .RemoveSchema()
    .RemoveDeclaration());

// XmlDocument → string
var xmlString = xmlDoc.ConvertToString();

// Deserialize string → object
var obj = XmlConverter.DeserializeObject<Invoice>(xmlString);

// Save / Load file
XmlConverter.SaveToFile("invoice.xml", invoice);
var loaded = XmlConverter.FileToObject<Invoice>("invoice.xml");

// XML → JSON
var json = xmlDoc.ToJson();
var json2 = XmlConverter.XmlToJson("<root><name>Test</name></root>");

// XML → Typed Object (unmatched properties = null)
var dto = XmlConverter.XmlToObject<InvoiceDto>(xmlString);
```

## F. Usage Guide

### Serialize

```csharp
// Default options (UTF-8 declaration, schema removed)
XmlDocument doc = XmlConverter.SerializeObject(myObject);

// Custom options via fluent builder
XmlDocument doc = XmlConverter.SerializeObject(myObject, b => b
    .RootElement("Products")
    .AddDeclaration(new XmlDeclarationOptions
    {
        Version = "1.0",
        Encoding = Encoding.UTF8,
        Standalone = true
    })
    .RemoveSchema()
    .RemoveTagCDDATA());

// Chain builder on existing XmlDocument
XmlDocument doc = XmlConverter.SerializeObject(myObject)
    .Builder(b => b.RootElement("Root").RemoveDeclaration());
```

### Deserialize

```csharp
// From XML string
var obj = XmlConverter.DeserializeObject<MyType>(xmlString);

// From XmlDocument
var obj = XmlConverter.DeserializeObject<MyType>(xmlDocument);
```

### File Operations

```csharp
// Save object to XML file
XmlConverter.SaveToFile("data.xml", myObject);

// Save XmlDocument to file
XmlConverter.SaveToFile<MyType>("data.xml", xmlDocument);

// Load file → object
var data = XmlConverter.FileToObject<MyType>("data.xml");

// Load file → XmlDocument
var doc = XmlConverter.LoadXml("data.xml");
```

### Async File Operations

```csharp
var cts = new CancellationTokenSource();

await XmlConverter.SaveToFileAsync("data.xml", myObject, cts.Token);
var data = await XmlConverter.FileToObjectAsync<MyType>("data.xml", cts.Token);
var doc = await XmlConverter.LoadXmlAsync("data.xml", cts.Token);
```

### XML to JSON

```csharp
// From XmlDocument
string json = xmlDoc.ToJson();

// From XML string
string json = XmlConverter.XmlToJson("<root><item>1</item></root>");

// From file
string json = XmlConverter.XmlFileToJson("data.xml");

// With options
string json = XmlConverter.XmlToJson(xmlDoc, new XmlToJsonOptions
{
    Indent = true,
    OmitRootObject = true,
    OmitXmlDeclaration = true,
    AttributePrefix = "@",
    TextNodeKey = "#text",
    CDataNodeKey = "#cdata-section",
    IncludeNamespaces = false
});

// Typed JSON — XML → map to T → JSON (only T's properties, unmatched = null)
string json = XmlConverter.XmlToJson<InvoiceDto>(xmlString);
string json = XmlConverter.XmlToJson<InvoiceDto>(xmlDocument);
string json = XmlConverter.XmlFileToJson<InvoiceDto>("data.xml");
string json = xmlDoc.ToJson<InvoiceDto>(indent: true);
```

### XML to Typed Object

Map XML directly to a C# object. Properties in `T` that don't match any XML element/attribute will remain `null` (or default value).

```csharp
public class InvoiceDto
{
    public int? Id { get; set; }
    public string? Customer { get; set; }
    public decimal? Total { get; set; }
    public string? Note { get; set; }        // not in XML → null
    public DateTime? DueDate { get; set; }   // not in XML → null
}

// From XML string
var dto = XmlConverter.XmlToObject<InvoiceDto>(xmlString);

// From XmlDocument
var dto = XmlConverter.XmlToObject<InvoiceDto>(xmlDocument);

// From file
var dto = XmlConverter.XmlFileToObject<InvoiceDto>("invoice.xml");

// Extension method
var dto = xmlDocument.ToObject<InvoiceDto>();
```

Supports: nested objects, collections (`List<T>`, `T[]`), all primitive types, enums, `DateTime`, `Guid`, `TimeSpan`, case-insensitive matching.

### Dependency Injection

```csharp
// Register all services
services.AddSerializationXml();

// Inject interfaces
public class MyService
{
    private readonly IXmlSerializer _serializer;
    private readonly IXmlFileOperations _fileOps;
    private readonly IXmlToJsonConverter _jsonConverter;
    private readonly IXmlToObjectMapper _objectMapper;
    private readonly IXmlTypedJsonConverter _typedJsonConverter;

    public MyService(
        IXmlSerializer serializer,
        IXmlFileOperations fileOps,
        IXmlToJsonConverter jsonConverter,
        IXmlToObjectMapper objectMapper,
        IXmlTypedJsonConverter typedJsonConverter)
    {
        _serializer = serializer;
        _fileOps = fileOps;
        _jsonConverter = jsonConverter;
        _objectMapper = objectMapper;
        _typedJsonConverter = typedJsonConverter;
    }
}
```

### Error Handling

All serialization and file errors are wrapped in `XmlSerializationException`:

```csharp
try
{
    var obj = XmlConverter.DeserializeObject<MyType>(badXml);
}
catch (XmlSerializationException ex)
{
    Console.WriteLine(ex.Message);     // Descriptive error message
    Console.WriteLine(ex.TargetType);  // typeof(MyType)
    Console.WriteLine(ex.Operation);   // "Deserialize"
    Console.WriteLine(ex.InnerException); // Original exception
}
```

`ArgumentNullException`, `FileNotFoundException`, and `OperationCanceledException` are **not** wrapped — they propagate directly.

## G. Configuration

This library does not require configuration files or app settings. All behavior is controlled through method parameters and options objects (`XmlOptionsBuilder`, `XmlToJsonOptions`, `XmlDeclarationOptions`).

### XmlOptionsBuilder

| Method | Description |
|---|---|
| `RootElement(string)` | Set root element name (validated as valid XML name) |
| `AddDeclaration(XmlDeclarationOptions)` | Add XML declaration |
| `RemoveDeclaration(bool)` | Remove XML declaration |
| `RemoveSchema(bool)` | Remove XML namespace schema |
| `RemoveTagCDDATA(bool)` | Remove CDATA tags |
| `AddPrefix(string)` | Add XML prefix |

### XmlToJsonOptions

| Property | Default | Description |
|---|---|---|
| `Indent` | `true` | Pretty-print JSON |
| `OmitRootObject` | `false` | Skip root object wrapper |
| `OmitXmlDeclaration` | `true` | Exclude `<?xml?>` from JSON |
| `AttributePrefix` | `"@"` | Prefix for XML attribute keys |
| `TextNodeKey` | `"#text"` | Key for text content |
| `CDataNodeKey` | `"#cdata-section"` | Key for CDATA content |
| `IncludeNamespaces` | `false` | Include xmlns attributes |

## H. Database & Migration

This is a pure XML utility library. No database or migration is required.

## I. Versioning & Compatibility

| Target | Supported |
|---|---|
| .NET Standard 2.0 | Yes |
| .NET Standard 2.1 | Yes |
| .NET Framework 4.8 | Yes |
| .NET 8.0 | Yes |
| .NET 9.0 | Yes |
| .NET 10.0 | Yes |

**Current version:** 2.1.0

**Migration from v1.x (CodeMazeNET.Serialization.Xml):**

```csharp
// v1.x
using CodeMazeNET.Serialization.Xml;

// v2.x
using MazeNET.SerializationXml;
using MazeNET.SerializationXml.Options;           // XmlOptionsBuilder, XmlToJsonOptions, etc.
using MazeNET.SerializationXml.Abstractions;      // IXmlSerializer, IXmlToJsonConverter, etc.
using MazeNET.SerializationXml.Exceptions;        // XmlSerializationException
```

The public API remains backward compatible. Namespaces changed from `Core.*` / `Infrastructure.*` to flat structure.

## J. Troubleshooting

| Problem | Solution |
|---|---|
| `XmlSerializationException` on serialize | Check that the object is serializable (public class, parameterless constructor, public properties) |
| `XmlSerializationException` on deserialize | Verify the XML string matches the target type structure |
| `FileNotFoundException` | Verify the file path exists before calling `LoadXml` / `FileToObject` |
| `ArgumentException` on `RootElement()` | The name must be a valid XML element name (no spaces, special chars) |
| Encoding issues | Use `XmlDeclarationOptions.Encoding` to set the desired encoding (default: UTF-8) |
| JSON output includes `?xml` | Set `OmitXmlDeclaration = true` in `XmlToJsonOptions` (default) |
| `XmlToObject<T>` returns null properties | Properties not matching any XML element/attribute remain null — use nullable types (`int?`, `string?`) |

## K. Contributing

**Build:**

```bash
cd src
dotnet build
```

**Run tests:**

> TODO: No test project found in repo. Test project should be added.

**Project structure:**

```
MazeNET.SerializationXml/
├── Abstractions/          # IXmlSerializer, IXmlFileOperations, IXmlToJsonConverter,
│                          # IXmlToObjectMapper, IXmlTypedJsonConverter
├── Exceptions/            # XmlSerializationException
├── Options/               # XmlOptions, XmlOptionsBuilder, XmlDeclarationOptions, XmlToJsonOptions
├── Services/              # XmlSerializerService, XmlFileOperationsService,
│                          # XmlToJsonConverterService, XmlToObjectMapperService,
│                          # XmlTypedJsonConverterService
├── Internal/              # XmlJsonWriter, ObjectToJsonWriter, XmlToObjectMapper
├── Extensions/            # XmlDocumentExtensions, ServiceCollectionExtensions
├── Assets/                # Logo.ico, Logo.png
└── XmlConverter.cs        # Static facade
```

## L. License

[MIT License](LICENSE) - Copyright (c) 2021 Duy Khanh
