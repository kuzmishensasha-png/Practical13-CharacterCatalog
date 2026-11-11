using Practical1.CLI;
using Practical1.Infra.Display;

namespace Practical1.Tests.Infra;

public class DisplayTests
{
    private class FakeDisplay : IDisplay
    {
        public string WrittenText { get; private set; } = "";
        public string? Input { get; set; }

        public void Write(string text)
        {
            WrittenText += text;
        }

        public void WriteLine(string text)
        {
            WrittenText += text;
        }

        public string? ReadLine()
        {
            return Input;
        }
    }

    [Fact]
    public void Presenter_ShowLine_WritesToDisplay()
    {
        FakeDisplay display =
            new FakeDisplay();

        ConsolePresenter presenter =
            new ConsolePresenter(display);

        presenter.ShowLine(
            "Hello"
        );

        Assert.Equal(
            "Hello",
            display.WrittenText
        );
    }

    [Fact]
    public void Presenter_Read_ReturnsDisplayInput()
    {
        FakeDisplay display =
            new FakeDisplay
            {
                Input = "test"
            };

        ConsolePresenter presenter =
            new ConsolePresenter(display);

        string? result =
            presenter.Read();

        Assert.Equal(
            "test",
            result
        );
    }

    [Fact]
    public void DisplayLogger_Log_WritesToDisplay()
    {
        FakeDisplay display =
            new FakeDisplay();

        DisplayLogger logger =
            new DisplayLogger(display);

        logger.Log(
            "Attack!"
        );

        Assert.Equal(
            "Attack!",
            display.WrittenText
        );
    }
}