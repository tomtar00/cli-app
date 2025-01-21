using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Backend.Utils;

public class CustomCsvDateTimeConverter : CsvHelper.TypeConversion.DateTimeConverter
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        string dateString = text!.Replace("+00Z", "Z");
        return DateTime.Parse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }
}