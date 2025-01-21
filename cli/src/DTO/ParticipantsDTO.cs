using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;

namespace Backend.DTO;

public class ParticipantsDTO
{
    [Name("participants")]
    [JsonPropertyName("participants")]
    public List<ParticipantDTO> Participants { get; set; } = [];
}