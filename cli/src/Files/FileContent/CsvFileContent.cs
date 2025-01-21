using Backend.Exceptions;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
using System.Reflection;

namespace Backend.Files;

public class CsvFileContent(byte[] content) : FileContent(content)
{
    public override TContent Deserialize<TContent>()
    {
        using MemoryStream memoryStream = new(_content);
        using var reader = new StreamReader(memoryStream);

        System.Type targetType;
        string listPropertyName = "";

        var config = CsvConfiguration.FromAttributes<TContent>(CultureInfo.InvariantCulture);

        if (typeof(TContent) == typeof(IList<>)) // if TContent is a list...
        {
            // target type is the generic argument of the list
            targetType = typeof(TContent).GetGenericArguments()[0];
        }
        else // if not..
        {
            // there must be a list property with a name that is the same as the value before slash in the first row
            // so we read the header row
            var headerRow = reader.ReadLine() ??
                throw new FileProcessException("No header row found");

            // and get the value before the slash
            var mappedListPropertyName = headerRow.Contains('/') ? headerRow.Split('/')[0] :
                throw new FileProcessException("No list property name found");

            // we need to remove the list property name from the header
            config.PrepareHeaderForMatch = (arg) => arg.Header.Replace($"{mappedListPropertyName}/", "");

            // find property with the name attribute that is the same as the value before slash in the first row
            var property = typeof(TContent).GetProperties().First(p => p.GetCustomAttribute<NameAttribute>()?.Names[0] == mappedListPropertyName) ??
                throw new FileProcessException($"Property with mapped name '{mappedListPropertyName}' not found in type {typeof(TContent).Name}");
            listPropertyName = property.Name;

            // target type is the generic argument of the list property
            targetType = property.PropertyType.GetGenericArguments()[0];

            // making sure that stream is reset to the beginning
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        using var csv = new CsvReader(reader, config);

        // traditional ToList doesn't work with dynamic types
        // so we need to use reflection to call it
        var getRecordsMethod = typeof(CsvReader).GetMethod("GetRecords", System.Type.EmptyTypes)!.MakeGenericMethod(targetType);
        var enumerable = getRecordsMethod.Invoke(csv, null);
        var toListMethod = typeof(Enumerable).GetMethod("ToList")!.MakeGenericMethod(targetType);
        var records = toListMethod.Invoke(null, [enumerable]);

        if (typeof(TContent) == typeof(IList<>))
        {
            return (TContent)records!;
        }
        else
        {
            // create object of type TContent
            var result = Activator.CreateInstance(typeof(TContent)) ??
                throw new FileProcessException($"Could not create instance of type {typeof(TContent).Name}");

            // set the list property
            var property = typeof(TContent).GetProperty(listPropertyName) ??
                throw new FileProcessException($"Property with name '{listPropertyName}' not found in type {typeof(TContent).Name}");
            property.SetValue(result, records);

            return (TContent)result;
        }
    }
}