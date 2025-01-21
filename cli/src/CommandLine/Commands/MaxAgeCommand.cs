using Backend.DTO;
using Backend.Files;

namespace Backend.CommandLine;

public class MaxAgeCommand : IExecutable
{
    public async Task Execute(CommandArguments args)
    {
        string url = CommandParser.GetParamValue<string>(args.parameters, 0);

        var file = await FileDownloader.Download(url);
        var participantsDto = file.Deserialize<ParticipantsDTO>();

        int maxAge = participantsDto.Participants.Max(p => p.Age);

        Console.WriteLine(maxAge);
    }
}