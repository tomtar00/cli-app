using Backend.DTO;
using Backend.Files;

namespace Backend.CommandLine;

public class CountCommand : IExecutable
{
    public async Task Execute(CommandArguments args)
    {
        string url = CommandParser.GetParamValue<string>(args.parameters, 0);
        int? ageGt = CommandParser.GetOptionValue<int>(args.options, "age-gt");
        int? ageLt = CommandParser.GetOptionValue<int>(args.options, "age-lt");

        var file = await FileDownloader.Download(url);
        var participantsDto = file.Deserialize<ParticipantsDTO>();

        var participants = participantsDto.Participants;
        if (ageGt.HasValue)
        {
            participants = participants.Where(p => p.Age > ageGt.Value).ToList();
        }
        if (ageLt.HasValue)
        {
            participants = participants.Where(p => p.Age < ageLt.Value).ToList();
        }
        Console.WriteLine(participants.Count);
    }
}