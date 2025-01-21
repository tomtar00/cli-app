namespace Backend.CommandLine;

public class CommandRegisty
{
    private List<CommandPattern> _commands = [];

    public static CommandRegisty CreateDefaultInstance()
    {
        return new()
        {
            _commands =
            [
                new()
                {
                    name = "count",
                    parameters = ["url"],
                    options = ["age-gt", "age-lt"],
                    commandHandler = typeof(CountCommand)
                },
                new()
                {
                    name = "max-age",
                    parameters = ["url"],
                    commandHandler = typeof(MaxAgeCommand)
                }
            ]
        };
    }

    public void RegisterCommand(CommandPattern commandPattern)
    {
        _commands.Add(commandPattern);
    }

    public CommandPattern? GetByName(string name)
    {
        foreach (var cmd in _commands)
        {
            if (cmd.name.Equals(name))
                return cmd;
        }
        return null;
    }
}

public struct CommandPattern
{
    public string name;
    public string[] parameters;
    public string[] options;
    public Type commandHandler;
}