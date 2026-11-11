namespace Practical1.CLI;

public interface ICommandParser
{
    CommandRequest Parse(string input);
}