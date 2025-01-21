namespace Backend.CommandLine;

public interface IExecutable
{
    Task Execute(CommandArguments argumentsPayload);
}