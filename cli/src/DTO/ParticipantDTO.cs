using System.Text.Json.Serialization;
using Backend.Utils;
using CsvHelper.Configuration.Attributes;

namespace Backend.DTO;

public struct ParticipantDTO
{
    [Name("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Name("age")]
    [JsonPropertyName("age")]
    public int Age { get; set; }

    [Name("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [Name("email")]
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [Name("workStart")]
    [TypeConverter(typeof(CustomCsvDateTimeConverter))]
    [JsonPropertyName("workStart")]
    [JsonConverter(typeof(CustomJsonDateTimeConverter))]
    public DateTime WorkStart { get; set; }

    [Name("workEnd")]
    [TypeConverter(typeof(CustomCsvDateTimeConverter))]
    [JsonPropertyName("workEnd")]
    [JsonConverter(typeof(CustomJsonDateTimeConverter))]
    public DateTime WorkEnd { get; set; }
}
