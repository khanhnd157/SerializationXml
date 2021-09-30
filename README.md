# SerializationXml
.NET XML Serialization Helper

Install from NUGET https://www.nuget.org/packages/CodeMazeNET.Serialization.Xml/

    Install-Package CodeMazeNET.Serialization.Xml

Include package into your project 
                
    using CodeMazeNET.Serialization.Xml;
    
## SerializeObject    
          
SerializeObject to XmlDocument: we have provider two function:
    
    var doc = XmlConverter.SerializeObject([Object]);
    
    // -- Serialize with config
    var doc = XmlConverter.SerializeObject([Object], buider =>
              buider.RootElement("Products")
                    .RemoveDeclaration()
                    .RemoveTagCDDATA()
                    .RemoveSchema());
          
    // -- OR add options with buider function:
    var doc = XmlConverter.SerializeObject([Object]).Builder(builder =>
              builder.RootElement("RootName")
                     .RemoveDeclaration()
                     .RemoveTagCDDATA()
                     .RemoveSchema());
                     
## DeserializeObject

    var object = XmlConverter.DeserializeObject<[type]>([dataxml]);       -- dataxml: string with format XML
    var object = XmlConverter.DeserializeObject<[type]>([xmlDocument]);   -- xmlDocument: XmlDocument type 
    

## ConvertToString

    var data = XmlConverter.ConvertToString([xmlDocument]);               -- xmlDocument: XmlDocument type 
    // -- OR : 
    var data = XmlConverter.SerializeObject([Object]).ConvertToString();
    // -- OR
    var data = xmlDocumnet.ConvertToString();
    
## Load file XML to XmlDocument:
    
    var path = @"C:\Invoices.xml";
    var doc = XmlConverter.LoadXml(path);
    
## Load file XML to Object:
    
    var path = @"C:\Invoices.xml";
    var data = XmlConverter.FileToObject(path);    
            
## Save data to file XML:
    
    //-- Save file from object type
    var path = @"C:\Invoices.xml";
    XmlConverter.SaveToFile(path, [object]);    
    
    // -- OR : Save to file from XmlDocument data
    var path = @"C:\Invoices.xml";
    XmlConverter.SaveToFile(path, xmlDocument);

# Thanks
### Thanks for use, if it's helpful for you please send to me 1 star!
