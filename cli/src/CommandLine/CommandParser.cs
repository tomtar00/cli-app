namespace Backend.CommandLine;
using Backend.Exceptions;

public static class CommandParser
{
    public static IExecutable Parse(CommandRegisty reg, string[] args, out CommandArguments argumentsPayload)
    {
        if (args.Length == 0)
            throw new CommandParseException("No arguments");

        // first argument is always the command name
        var commandName = args[0];
        var cmdPatternOpt = reg.GetByName(commandName);

        if (!cmdPatternOpt.HasValue)
            throw new CommandParseException("Failed to recognize command name");

        var cmdPattern = cmdPatternOpt.Value;

        // other arguments are parameters or options
        // parameters are values that occur directly
        // after command name
        // options are always starting with '--' and
        // the next argument contains it's value
        var parameters = new List<string>();
        var options = new Dictionary<string, string>();
        for (int i = 1; i < args.Length; ++i)
        {
            var arg = args[i];
            if (!IsOption(arg))
            {
                if (parameters.Count > cmdPattern.parameters.Length)
                    throw new CommandParseException($"Numbers of parameters is larger than expected. Was: {parameters.Count} Expected: {cmdPattern.parameters.Length}");

                parameters.Add(arg);
            }
            else
            {
                var option = arg[2..];
                if (cmdPattern.options != null && !string.IsNullOrEmpty(option) && cmdPattern.options.Contains(option))
                {
                    if (options.Count > cmdPattern.options.Length)
                        throw new CommandParseException($"Numbers of options is larger than expected. Was: {options.Count} Expected: {cmdPattern.options.Length}");
                    if (i + 1 >= args.Length || IsOption(args[i + 1]))
                        throw new CommandParseException($"No value passed for option '{arg}'");

                    options[option] = args[++i];
                }
                else
                {
                    throw new CommandParseException($"Option '{arg}' not present in command '{commandName}'");
                }
            }
        }

        // check if all parameters were passed
        if (parameters.Count < cmdPattern.parameters.Length)
            throw new CommandParseException($"Not enough parameters for command '{commandName}'. Was: {parameters.Count}, expected: {cmdPattern.parameters.Length}");

        // create instance of the object that is capable of executing the command
        if (cmdPattern.commandHandler == null)
        {
            throw new CommandParseException($"Executor for command '{commandName}' is null.");
        }
        var instance = Activator.CreateInstance(cmdPattern.commandHandler) ??
            throw new CommandParseException($"Failed to create an instance of executor for command '{commandName}'.");

        argumentsPayload = new()
        {
            parameters = [.. parameters],
            options = options
        };
        return (IExecutable)instance;
    }

    public static T GetParamValue<T>(string[] parameters, int index)
    {
        if (index < 0 || index >= parameters.Length)
        {
            throw new CommandParseException($"Index {index} is out of range for parameters array.");
        }

        string value = parameters[index];
        return ParseArgumentValue<T>(value);
    }

    public static T? GetOptionValue<T>(Dictionary<string, string> options, string optionName) where T : struct
    {
        if (options.TryGetValue(optionName, out string? value) && value != null)
        {
            return ParseArgumentValue<T>(value);
        }
        return null;
    }

    private static T ParseArgumentValue<T>(string arg)
    {
        try
        {
            return typeof(T) switch
            {
                Type t when t == typeof(int) => (T)(object)int.Parse(arg),
                Type t when t == typeof(bool) => (T)(object)bool.Parse(arg),
                Type t when t == typeof(double) => (T)(object)double.Parse(arg),
                Type t when t == typeof(string) => (T)(object)arg,
                _ => throw new NotSupportedException($"Type {typeof(T)} is not supported.")
            };
        }
        catch (NotSupportedException ns)
        {
            throw new CommandParseException("", ns);
        }
        catch (Exception)
        {
            throw new CommandParseException($"Failed to parse '{arg}' into type '{typeof(T).Name}'");
        }
    }

    private static bool IsOption(string str)
    {
        return str.StartsWith("--");
    }
}