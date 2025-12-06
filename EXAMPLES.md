# Usage Examples

## 📚 Table of Contents

1. [Basic Usage](#basic-usage)
2. [Advanced Serialization](#advanced-serialization)
3. [File Operations](#file-operations)
4. [Using with Dependency Injection](#using-with-dependency-injection)
5. [Custom Configuration](#custom-configuration)
6. [Real-World Examples](#real-world-examples)

---

## Basic Usage

### Simple Serialization

```csharp
using MazeNET.SerializationXml;

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

// Create object
var person = new Person 
{ 
    Id = 1, 
    Name = "John Doe", 
    Age = 30 
};

// Serialize to XmlDocument
var xmlDoc = XmlConverter.SerializeObject(person);

// Convert to string
string xmlString = xmlDoc.ConvertToString();
Console.WriteLine(xmlString);
```

**Output:**
```xml
<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<Person>
  <Id>1</Id>
  <Name>John Doe</Name>
  <Age>30</Age>
</Person>
```

### Simple Deserialization

```csharp
using MazeNET.SerializationXml;

string xmlString = @"
<Person>
  <Id>1</Id>
  <Name>John Doe</Name>
  <Age>30</Age>
</Person>";

// Deserialize from string
var person = XmlConverter.DeserializeObject<Person>(xmlString);

Console.WriteLine($"Name: {person.Name}, Age: {person.Age}");
// Output: Name: John Doe, Age: 30
```

---

## Advanced Serialization

### Custom Root Element

```csharp
using MazeNET.SerializationXml;
using MazeNET.SerializationXml.Core.Options;

var person = new Person { Id = 1, Name = "Jane Doe", Age = 25 };

var xmlDoc = XmlConverter.SerializeObject(person, builder =>
    builder.RootElement("Employee"));

string xml = xmlDoc.ConvertToString();
```

**Output:**
```xml
<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<Employee>
  <Id>1</Id>
  <Name>Jane Doe</Name>
  <Age>25</Age>
</Employee>
```

### Remove XML Declaration

```csharp
var xmlDoc = XmlConverter.SerializeObject(person, builder =>
    builder.RootElement("Person")
           .RemoveDeclaration());

string xml = xmlDoc.ConvertToString();
```

**Output:**
```xml
<Person>
  <Id>1</Id>
  <Name>John Doe</Name>
  <Age>30</Age>
</Person>
```

### Custom XML Declaration

```csharp
using System.Text;

var xmlDoc = XmlConverter.SerializeObject(person, builder =>
    builder.AddDeclaration(new XmlDeclarationOptions
    {
        Version = "1.0",
        Encoding = Encoding.UTF8,
        Standalone = false
    })
    .RemoveSchema());
```

### Remove CDATA Tags

```csharp
var xmlDoc = XmlConverter.SerializeObject(person, builder =>
    builder.RemoveTagCDDATA()
           .RemoveSchema());
```

### Complete Configuration

```csharp
var xmlDoc = XmlConverter.SerializeObject(person, builder =>
    builder.RootElement("Employee")
           .AddDeclaration(new XmlDeclarationOptions
           {
               Version = "1.0",
               Encoding = Encoding.UTF8,
               Standalone = true
           })
           .RemoveSchema()
           .RemoveTagCDDATA());
```

---

## File Operations

### Save to File

```csharp
using MazeNET.SerializationXml;

var person = new Person { Id = 1, Name = "John Doe", Age = 30 };

// Save object directly to file
string filePath = @"C:\Data\person.xml";
bool success = XmlConverter.SaveToFile(filePath, person);

if (success)
{
    Console.WriteLine("File saved successfully!");
}
```

### Load from File

```csharp
using MazeNET.SerializationXml;

// Load XML file to object
string filePath = @"C:\Data\person.xml";
var person = XmlConverter.FileToObject<Person>(filePath);

Console.WriteLine($"Loaded: {person.Name}");
```

### Load XML Document

```csharp
using MazeNET.SerializationXml;

// Load XML file to XmlDocument
string filePath = @"C:\Data\person.xml";
var xmlDoc = XmlConverter.LoadXml(filePath);

// Then deserialize if needed
var person = XmlConverter.DeserializeObject<Person>(xmlDoc);
```

---

## Using with Dependency Injection

### ASP.NET Core Setup

```csharp
using MazeNET.SerializationXml.Core.Interfaces;
using MazeNET.SerializationXml.Infrastructure.Converters;

// Program.cs or Startup.cs
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Register services
        builder.Services.AddSingleton<IXmlSerializer, XmlSerializerService>();
        builder.Services.AddSingleton<IXmlFileOperations, XmlFileOperationsService>();
        
        builder.Services.AddControllers();
        
        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}
```

### Using in Service

```csharp
using MazeNET.SerializationXml.Core.Interfaces;

public class PersonService
{
    private readonly IXmlSerializer _serializer;
    private readonly IXmlFileOperations _fileOps;
    
    public PersonService(IXmlSerializer serializer, IXmlFileOperations fileOps)
    {
        _serializer = serializer;
        _fileOps = fileOps;
    }
    
    public void SavePerson(Person person, string path)
    {
        // Serialize
        var xmlDoc = _serializer.Serialize(person);
        
        // Save to file
        _fileOps.SaveToFile(path, xmlDoc);
    }
    
    public Person LoadPerson(string path)
    {
        // Load from file
        return _fileOps.LoadFromFile<Person>(path);
    }
    
    public string ExportToXml(Person person)
    {
        var xmlDoc = _serializer.Serialize(person, builder =>
            builder.RootElement("Person")
                   .RemoveSchema());
        
        return xmlDoc.ConvertToString();
    }
}
```

### Using in Controller

```csharp
using Microsoft.AspNetCore.Mvc;
using MazeNET.SerializationXml.Core.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly IXmlSerializer _serializer;
    
    public PersonController(IXmlSerializer serializer)
    {
        _serializer = serializer;
    }
    
    [HttpPost("export")]
    public IActionResult ExportToXml([FromBody] Person person)
    {
        var xmlDoc = _serializer.Serialize(person);
        var xmlString = xmlDoc.ConvertToString();
        
        return Content(xmlString, "application/xml");
    }
    
    [HttpPost("import")]
    public IActionResult ImportFromXml([FromBody] string xmlString)
    {
        var person = _serializer.Deserialize<Person>(xmlString);
        return Ok(person);
    }
}
```

---

## Custom Configuration

### Builder Pattern

```csharp
using MazeNET.SerializationXml.Core.Options;

// Create builder
var builder = new XmlOptionsBuilder();

// Configure options
var options = builder
    .RootElement("CustomRoot")
    .AddDeclaration(new XmlDeclarationOptions 
    { 
        Version = "1.0", 
        Encoding = Encoding.UTF8 
    })
    .RemoveSchema()
    .RemoveTagCDDATA()
    .Build();

// Use with serialization
var xmlDoc = person.SerializeWithOptions(builder);
```

### Reusable Configuration

```csharp
public static class XmlConfigurations
{
    public static Func<XmlOptionsBuilder, XmlOptionsBuilder> DefaultConfig =>
        builder => builder
            .RemoveSchema()
            .AddDeclaration(new XmlDeclarationOptions
            {
                Version = "1.0",
                Encoding = Encoding.UTF8,
                Standalone = true
            });
    
    public static Func<XmlOptionsBuilder, XmlOptionsBuilder> CompactConfig =>
        builder => builder
            .RemoveDeclaration()
            .RemoveSchema();
    
    public static Func<XmlOptionsBuilder, XmlOptionsBuilder> CustomRootConfig(string rootName) =>
        builder => builder
            .RootElement(rootName)
            .RemoveSchema();
}

// Usage
var xmlDoc1 = XmlConverter.SerializeObject(person, XmlConfigurations.DefaultConfig);
var xmlDoc2 = XmlConverter.SerializeObject(person, XmlConfigurations.CompactConfig);
var xmlDoc3 = XmlConverter.SerializeObject(person, XmlConfigurations.CustomRootConfig("Employee"));
```

---

## Real-World Examples

### Example 1: Invoice Management System

```csharp
using MazeNET.SerializationXml;
using MazeNET.SerializationXml.Core.Options;

public class Invoice
{
    public int InvoiceId { get; set; }
    public string CustomerName { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<InvoiceItem> Items { get; set; }
}

public class InvoiceItem
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total => Quantity * UnitPrice;
}

public class InvoiceService
{
    private readonly string _basePath = @"C:\Invoices\";
    
    public void SaveInvoice(Invoice invoice)
    {
        var fileName = $"Invoice_{invoice.InvoiceId}_{DateTime.Now:yyyyMMdd}.xml";
        var fullPath = Path.Combine(_basePath, fileName);
        
        var xmlDoc = XmlConverter.SerializeObject(invoice, builder =>
            builder.RootElement("Invoice")
                   .AddDeclaration(new XmlDeclarationOptions
                   {
                       Version = "1.0",
                       Encoding = Encoding.UTF8,
                       Standalone = true
                   })
                   .RemoveSchema());
        
        XmlConverter.SaveToFile(fullPath, invoice);
        Console.WriteLine($"Invoice saved to: {fullPath}");
    }
    
    public Invoice LoadInvoice(int invoiceId, DateTime date)
    {
        var fileName = $"Invoice_{invoiceId}_{date:yyyyMMdd}.xml";
        var fullPath = Path.Combine(_basePath, fileName);
        
        return XmlConverter.FileToObject<Invoice>(fullPath);
    }
    
    public List<Invoice> LoadAllInvoices()
    {
        var invoices = new List<Invoice>();
        var files = Directory.GetFiles(_basePath, "Invoice_*.xml");
        
        foreach (var file in files)
        {
            var invoice = XmlConverter.FileToObject<Invoice>(file);
            invoices.Add(invoice);
        }
        
        return invoices;
    }
}

// Usage
var invoice = new Invoice
{
    InvoiceId = 12345,
    CustomerName = "ACME Corp",
    InvoiceDate = DateTime.Now,
    Items = new List<InvoiceItem>
    {
        new InvoiceItem { ProductName = "Widget A", Quantity = 10, UnitPrice = 25.50m },
        new InvoiceItem { ProductName = "Widget B", Quantity = 5, UnitPrice = 45.00m }
    }
};
invoice.TotalAmount = invoice.Items.Sum(i => i.Total);

var service = new InvoiceService();
service.SaveInvoice(invoice);
```

### Example 2: Configuration Manager

```csharp
using MazeNET.SerializationXml;

public class AppConfiguration
{
    public DatabaseSettings Database { get; set; }
    public LoggingSettings Logging { get; set; }
    public EmailSettings Email { get; set; }
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; }
    public int CommandTimeout { get; set; }
    public bool EnableRetry { get; set; }
}

public class LoggingSettings
{
    public string LogLevel { get; set; }
    public string LogPath { get; set; }
}

public class EmailSettings
{
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string FromAddress { get; set; }
}

public class ConfigurationManager
{
    private const string ConfigPath = "appsettings.xml";
    
    public void SaveConfiguration(AppConfiguration config)
    {
        XmlConverter.SaveToFile(ConfigPath, config);
    }
    
    public AppConfiguration LoadConfiguration()
    {
        if (!File.Exists(ConfigPath))
        {
            return CreateDefaultConfiguration();
        }
        
        return XmlConverter.FileToObject<AppConfiguration>(ConfigPath);
    }
    
    private AppConfiguration CreateDefaultConfiguration()
    {
        var defaultConfig = new AppConfiguration
        {
            Database = new DatabaseSettings
            {
                ConnectionString = "Server=localhost;Database=MyDb;",
                CommandTimeout = 30,
                EnableRetry = true
            },
            Logging = new LoggingSettings
            {
                LogLevel = "Information",
                LogPath = "C:\\Logs\\app.log"
            },
            Email = new EmailSettings
            {
                SmtpServer = "smtp.example.com",
                SmtpPort = 587,
                FromAddress = "noreply@example.com"
            }
        };
        
        SaveConfiguration(defaultConfig);
        return defaultConfig;
    }
}

// Usage
var configManager = new ConfigurationManager();
var config = configManager.LoadConfiguration();

Console.WriteLine($"Database: {config.Database.ConnectionString}");
Console.WriteLine($"Log Level: {config.Logging.LogLevel}");
```

### Example 3: Data Export Service

```csharp
using MazeNET.SerializationXml;
using MazeNET.SerializationXml.Core.Interfaces;

public class DataExportService
{
    private readonly IXmlSerializer _serializer;
    
    public DataExportService(IXmlSerializer serializer)
    {
        _serializer = serializer;
    }
    
    public string ExportToXml<T>(T data, string rootElementName = null)
    {
        var xmlDoc = _serializer.Serialize(data, builder =>
        {
            var b = builder.RemoveSchema();
            
            if (!string.IsNullOrEmpty(rootElementName))
            {
                b = b.RootElement(rootElementName);
            }
            
            return b;
        });
        
        return xmlDoc.ConvertToString();
    }
    
    public byte[] ExportToXmlBytes<T>(T data)
    {
        var xmlString = ExportToXml(data);
        return Encoding.UTF8.GetBytes(xmlString);
    }
    
    public void ExportToFile<T>(T data, string filePath)
    {
        var xmlString = ExportToXml(data);
        File.WriteAllText(filePath, xmlString, Encoding.UTF8);
    }
    
    public async Task<Stream> ExportToStreamAsync<T>(T data)
    {
        var xmlString = ExportToXml(data);
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream, Encoding.UTF8);
        await writer.WriteAsync(xmlString);
        await writer.FlushAsync();
        stream.Position = 0;
        return stream;
    }
}

// Usage in ASP.NET Core
[HttpGet("export")]
public async Task<IActionResult> ExportData()
{
    var data = GetSomeData();
    var exportService = new DataExportService(new XmlSerializerService());
    
    var stream = await exportService.ExportToStreamAsync(data);
    return File(stream, "application/xml", "export.xml");
}
```

---

## 🎓 Best Practices

1. **Always dispose streams properly** - Use `using` statements
2. **Handle exceptions** - Wrap serialization in try-catch blocks
3. **Validate XML** - Check for null/empty before deserializing
4. **Use interfaces** - Program to interfaces for better testability
5. **Configure appropriately** - Use builder pattern for custom configurations
6. **Cache serializers** - Reuse serializer instances when possible

---

For more examples and documentation, visit:
- [GitHub Repository](https://github.com/khanhnd157/SerializationXml)
- [Architecture Guide](ARCHITECTURE.md)
- [Build Guide](BUILD.md)

