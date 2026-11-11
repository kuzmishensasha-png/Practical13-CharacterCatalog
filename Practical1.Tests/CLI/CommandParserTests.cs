using Practical1.CLI;

namespace Practical1.Tests;

public class CommandParserTests
{
    [Fact]
    public void Parse_SimpleCommand_ReturnsCommandName()
    {

        CommandParser parser =
            new CommandParser();

        string input = "pwd";


        CommandRequest result =
            parser.Parse(input);


        Assert.Equal(
            "pwd",
            result.Name
        );
    }

    [Fact]
    public void Parse_CommandWithArguments_ReturnsArguments()
    {

        CommandParser parser =
            new CommandParser();

        string input =
            "add leaf paragraph";


        CommandRequest result =
            parser.Parse(input);


        Assert.Equal(
            "add",
            result.Name
        );

        Assert.Equal(
            2,
            result.Arguments.Count
        );

        Assert.Equal(
            "leaf",
            result.Arguments[0]
        );

        Assert.Equal(
            "paragraph",
            result.Arguments[1]
        );
    }

    [Fact]
    public void Parse_CommandWithOption_ReturnsOption()
    {

        CommandParser parser =
            new CommandParser();

        string input =
            "cd --id abc123";


        CommandRequest result =
            parser.Parse(input);


        Assert.Equal(
            "cd",
            result.Name
        );

        Assert.True(
            result.Options.ContainsKey("id")
        );

        Assert.Equal(
            "abc123",
            result.Options["id"]
        );
    }
}