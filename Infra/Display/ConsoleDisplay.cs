namespace Practical1.Infra.Display;

public class ConsoleDisplay : IDisplay
{
    public void Write(string text)
    {
        Console.Write(text);
    }

    public void WriteLine(string text)
    {
        Console.WriteLine(text);
    }

    public string? ReadLine()
    {
        return Console.ReadLine();
    }
}