# MazeNET.SerializationXml

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

.NET XML Serialization Helper - Clean Architecture

## ✨ Features
- ✅ Clean Architecture design pattern
- ✅ Interface-based abstractions
- ✅ Multi-targeting support: .NET Framework 4.8, .NET 9.0, .NET 10.0
- ✅ Nullable reference types support for .NET 9+
- ✅ Comprehensive XML documentation
- ✅ Backward compatible API

## 🏗️ Architecture

```
MazeNET.SerializationXml/
├── Core/
│   ├── Interfaces/
│   │   ├── IXmlSerializer.cs
│   │   ├── IXmlFileOperations.cs
│   │   └── IXmlDocumentConverter.cs
│   └── Options/
│       ├── XmlOptions.cs
│       ├── XmlDeclarationOptions.cs
│       └── XmlOptionsBuilder.cs
└── Infrastructure/
    ├── Converters/
    │   ├── XmlSerializerService.cs
    │   └── XmlFileOperationsService.cs
    └── Extensions/
        └── XmlExtensions.cs
```

## 📦 Installation

Install from NUGET https://www.nuget.org/packages/MazeNET.SerializationXml/

```powershell
Install-Package MazeNET.SerializationXml
```

Or via .NET CLI:

```bash
dotnet add package MazeNET.SerializationXml
```

## 🚀 Usage

Include package into your project:
                
```csharp
using MazeNET.SerializationXml;
using MazeNET.SerializationXml.Core.Interfaces;
using MazeNET.SerializationXml.Core.Options;
```

## 📖 API Reference

### Using Facade (Simple API - Backward Compatible)

#### SerializeObject    
          
SerializeObject to XmlDocument:
    
```csharp
var doc = XmlConverter.SerializeObject(myObject);

// -- Serialize with config
var doc = XmlConverter.SerializeObject(myObject, builder =>
          builder.RootElement("Products")
                .RemoveDeclaration()
                .RemoveTagCDDATA()
                .RemoveSchema());
      
// -- OR add options with builder function:
var doc = XmlConverter.SerializeObject(myObject).Builder(builder =>
          builder.RootElement("RootName")
                 .RemoveDeclaration()
                 .RemoveTagCDDATA()
                 .RemoveSchema());
```
                 
#### DeserializeObject

```csharp
// From XML string
var myObject = XmlConverter.DeserializeObject<MyType>(xmlString);

// From XmlDocument
var myObject = XmlConverter.DeserializeObject<MyType>(xmlDocument);
```

#### ConvertToString

```csharp
// Convert XmlDocument to string
var xmlString = xmlDocument.ConvertToString();

// OR chain with serialization
var xmlString = XmlConverter.SerializeObject(myObject).ConvertToString();
```
    
#### Load file XML to XmlDocument:
    
```csharp
var path = @"C:\Invoices.xml";
var doc = XmlConverter.LoadXml(path);
```
    
#### Load file XML to Object:
    
```csharp
var path = @"C:\Invoices.xml";
var data = XmlConverter.FileToObject<Invoice>(path);
```
            
#### Save data to file XML:
    
```csharp
// Save object to file
var path = @"C:\Invoices.xml";
XmlConverter.SaveToFile(path, myObject);

// Save XmlDocument to file
var path = @"C:\Invoices.xml";
XmlConverter.SaveToFile<Invoice>(path, xmlDocument);
```

### Using Interfaces (Dependency Injection)

For modern applications using dependency injection:

```csharp
using MazeNET.SerializationXml.Core.Interfaces;
using MazeNET.SerializationXml.Infrastructure.Converters;

// Register in your DI container
services.AddSingleton<IXmlSerializer, XmlSerializerService>();
services.AddSingleton<IXmlFileOperations, XmlFileOperationsService>();

// Use in your classes
public class MyService
{
    private readonly IXmlSerializer _xmlSerializer;
    private readonly IXmlFileOperations _fileOps;
    
    public MyService(IXmlSerializer xmlSerializer, IXmlFileOperations fileOps)
    {
        _xmlSerializer = xmlSerializer;
        _fileOps = fileOps;
    }
    
    public void SaveData(MyData data, string path)
    {
        var xmlDoc = _xmlSerializer.Serialize(data);
        _fileOps.SaveToFile(path, xmlDoc);
    }
    
    public MyData LoadData(string path)
    {
        return _fileOps.LoadFromFile<MyData>(path);
    }
}
```

## 🔧 Configuration Options

### XmlOptionsBuilder Methods

- `RootElement(string name)` - Set root element name
- `AddDeclaration(XmlDeclarationOptions)` - Add XML declaration
- `RemoveDeclaration(bool)` - Remove XML declaration
- `RemoveSchema(bool)` - Remove XML schema
- `RemoveTagCDDATA(bool)` - Remove CDATA tags
- `AddPrefix(string)` - Add XML prefix

### XmlDeclarationOptions Properties

- `Version` - XML version (default: "1.0")
- `Encoding` - XML encoding (default: UTF-8)
- `Standalone` - Standalone declaration (default: true)

## 🔄 Migration from v1.x

If you're upgrading from CodeMazeNET.Serialization.Xml v1.x:

```csharp
// Old namespace (v1.x)
using CodeMazeNET.Serialization.Xml;

// New namespace (v2.x)
using MazeNET.SerializationXml;
```

The API remains the same, so your existing code will work with just the namespace change!

## 📝 Example

```csharp
using MazeNET.SerializationXml;
using MazeNET.SerializationXml.Core.Options;

public class Invoice
{
    public int Id { get; set; }
    public string Customer { get; set; }
    public decimal Amount { get; set; }
}

// Serialize
var invoice = new Invoice 
{ 
    Id = 1, 
    Customer = "John Doe", 
    Amount = 150.00m 
};

var xmlDoc = XmlConverter.SerializeObject(invoice, builder => 
    builder.RootElement("Invoice")
           .RemoveSchema()
           .AddDeclaration(new XmlDeclarationOptions
           {
               Version = "1.0",
               Encoding = Encoding.UTF8,
               Standalone = true
           }));

// Save to file
XmlConverter.SaveToFile("invoice.xml", invoice);

// Load from file
var loadedInvoice = XmlConverter.FileToObject<Invoice>("invoice.xml");

// Deserialize from XML string
var xmlString = xmlDoc.ConvertToString();
var deserializedInvoice = XmlConverter.DeserializeObject<Invoice>(xmlString);
```

## 🎯 Benefits of Clean Architecture

1. **Separation of Concerns** - Core business logic separated from infrastructure
2. **Testability** - Easy to mock interfaces for unit testing
3. **Maintainability** - Clear structure makes code easier to understand and modify
4. **Flexibility** - Easy to swap implementations without changing client code
5. **Dependency Inversion** - High-level modules don't depend on low-level modules

# Thanks
### Thanks for use, if it's helpful for you please send me 1 star! ⭐
