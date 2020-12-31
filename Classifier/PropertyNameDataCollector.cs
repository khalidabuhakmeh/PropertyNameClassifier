using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public static class PropertyNameDataCollector
{
    public static async Task ScanThisApp()
    {
        const string path = "data.csv";
        foreach (var ass in AppDomain.CurrentDomain.GetAssemblies())
        {
            await ass.Scan(path);
        }
    }
    
    public static async Task Scan(this Assembly assembly, string outputFilePath, bool includeHeader = false)
    {
        var types = assembly.GetTypes();

        var properties =
            from source in types
            from info in source.GetProperties()
            let type = info.PropertyType
            let description = type.IsGenericType ? type.GetGenericTypeDefinition().FullName : type.FullName
            let location = source.Assembly.GetName().Name
            where description != null
            select new
            {
                name = info.Name,
                description = description,
                location = location
            };

        var csv = new StringBuilder();
        if (includeHeader)
        {
            csv.AppendLine("Name,Type,Location");
        }

        foreach (var record in properties) {
            var row = $"{record.name},{record.description},{record.location}";
            Console.WriteLine($"- {row}");
            csv.AppendLine(row);
        }
        
        await File.AppendAllTextAsync(outputFilePath, 
            csv.ToString(), 
            Encoding.UTF8
        );
    }
}