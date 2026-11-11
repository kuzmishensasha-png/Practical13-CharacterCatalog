namespace Practical1.CLI;

public class CommandRequest
{
    public string Name { get; }
    public List<string> Arguments { get; }
    public Dictionary<string, string?> Options { get; }

    public CommandRequest(
        string name,
        List<string> arguments,
        Dictionary<string, string?> options)
    {
        Name = name;
        Arguments = arguments;
        Options = options;
    }
}