using Backend.CommandLine;
using Backend.Exceptions;

namespace Backend;

class MainClass
{
    public static async Task Main(string[] args)
    {
        try
        {
            // create command registry
            var reg = CommandRegisty.CreateDefaultInstance();

            // parse the entered command
            IExecutable command = CommandParser.Parse(reg, args, out var arguments);

            // execute the parsed command
            await command.Execute(arguments);
        }
        catch (CommandParseException pc)
        {
            Console.WriteLine("Error: Invalid command." + pc);
        }
        catch (FileGetException gf)
        {
            Console.WriteLine("Error: Cannot get file." + gf);
        }
        catch (FileProcessException pf)
        {
            Console.WriteLine("Error: Cannot process the file." + pf);
        }
    }
}