namespace Practical1.CLI;

public interface IPresenter
{
    void Show(string text);
    void ShowLine(string text);
    string? Read();
}