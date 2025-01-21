using Backend.CommandLine;
using Backend.Exceptions;

namespace tests;

public class Tests
{
    private const string JSON_URL = "";
    private const string CSV_URL = "";
    private const string ZIP_URL = "";
    private CommandRegisty? _reg;

    [SetUp]
    public void Setup()
    {
        _reg = CommandRegisty.CreateDefaultInstance();
    }

    [Test]
    public void Parse()
    {
        string[] args = ["count", "url", "--age-gt", "22"];
        IExecutable command = CommandParser.Parse(_reg!, args, out var arguments);
        Assert.That(command, Is.Not.Null);
        Assert.That(command, Is.InstanceOf<CountCommand>());
        Assert.That(arguments.parameters, Is.Not.Null);
        Assert.That(arguments.parameters.Length, Is.EqualTo(1));
        Assert.That(arguments.parameters[0], Is.EqualTo("url"));
        Assert.That(arguments.options, Is.Not.Null);
        Assert.That(arguments.options.Count, Is.EqualTo(1));
        Assert.That(arguments.options["age-gt"], Is.EqualTo("22"));
    }

    // possibly too many requests to a single endpoint
    // may exceed rate limit
    [TestCase("5", "count", JSON_URL)]
    [TestCase("2", "count", JSON_URL, "--age-gt", "22")]
    [TestCase("4", "count", JSON_URL, "--age-lt", "24")]
    [TestCase("24", "max-age", JSON_URL)]
    [TestCase("5", "count", CSV_URL)]
    [TestCase("2", "count", CSV_URL, "--age-gt", "22")]
    [TestCase("4", "count", CSV_URL, "--age-lt", "24")]
    [TestCase("24", "max-age", CSV_URL)]
    [TestCase("5", "count", ZIP_URL)]
    [TestCase("2", "count", ZIP_URL, "--age-gt", "22")]
    [TestCase("4", "count", ZIP_URL, "--age-lt", "24")]
    [TestCase("24", "max-age", ZIP_URL)]
    public async Task Run(string expected, params string[] args)
    {
        // redirect console output to string writer
        using StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        IExecutable command = CommandParser.Parse(_reg!, args, out var arguments);
        await command.Execute(arguments);

        string actual = stringWriter.ToString();
        Assert.That(actual, Is.EqualTo(expected + Environment.NewLine));
    }

    [TestCase(typeof(CommandParseException), "counts", JSON_URL)]
    [TestCase(typeof(CommandParseException), "count", JSON_URL, "age-gt", "10")]
    [TestCase(typeof(CommandParseException), "count", JSON_URL, "--age", "10")]
    [TestCase(typeof(CommandParseException), "count", JSON_URL, "--age-gt", "ten")]
    [TestCase(typeof(CommandParseException), "max-age", JSON_URL, "--opt", "test")]
    [TestCase(typeof(FileGetException), "count", "https://someaddress.io/missing.json")]
    public async Task Run_ThrowsException(Type expected, params string[] args)
    {
        try
        {
            IExecutable command = CommandParser.Parse(_reg!, args, out var arguments);
            await command.Execute(arguments);
        }
        catch (Exception exception)
        {
            Assert.That(exception, Is.InstanceOf(expected));
        }
    }
}