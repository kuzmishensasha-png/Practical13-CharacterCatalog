namespace Practical1.Infra.Display;

public interface IDisplay
{
    void Write(string text);
    void WriteLine(string text);
    string? ReadLine();
}