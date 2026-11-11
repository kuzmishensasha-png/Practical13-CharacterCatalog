namespace Practical1.CLI;

public class CommandParser : ICommandParser
{
    public CommandRequest Parse(string input)
    {
        List<string> parts = SplitInput(input);

        string name = parts[0];

        List<string> arguments = new();
        Dictionary<string, string?> options = new();

        for (int i = 1; i < parts.Count; i++)
        {
            string part = parts[i];

            if (part.StartsWith("--"))
            {
                string optionName = part[2..];
                string? optionValue = null;

                if (i + 1 < parts.Count && !parts[i + 1].StartsWith("--"))
                {
                    optionValue = parts[i + 1];
                    i++;
                }

                options[optionName] = optionValue;
            }
            else
            {
                arguments.Add(part);
            }
        }

        return new CommandRequest(name, arguments, options);
    }

    private List<string> SplitInput(string input)
    {
        List<string> result = new();
        string current = "";
        bool insideQuotes = false;

        foreach (char symbol in input)
        {
            if (symbol == '"')
            {
                insideQuotes = !insideQuotes;
                continue;
            }

            if (symbol == ' ' && !insideQuotes)
            {
                if (current.Length > 0)
                {
                    result.Add(current);
                    current = "";
                }
            }
            else
            {
                current += symbol;
            }
        }

        if (current.Length > 0)
        {
            result.Add(current);
        }

        return result;
    }
}