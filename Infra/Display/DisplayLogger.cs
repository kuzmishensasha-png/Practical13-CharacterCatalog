using Practical1.GameSystem;

namespace Practical1.Infra.Display;

public class DisplayLogger : ILogger
{
    private readonly IDisplay display;

    public DisplayLogger(IDisplay display)
    {
        this.display = display;
    }

    public void Log(string message)
    {
        display.WriteLine(message);
    }
}