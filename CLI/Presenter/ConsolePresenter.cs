using Practical1.Infra.Display;

namespace Practical1.CLI;

public class ConsolePresenter : IPresenter
{
    private readonly IDisplay display;

    public ConsolePresenter(IDisplay display)
    {
        this.display = display;
    }

    public void Show(string text)
    {
        display.Write(text);
    }

    public void ShowLine(string text)
    {
        display.WriteLine(text);
    }

    public string? Read()
    {
        return display.ReadLine();
    }
}