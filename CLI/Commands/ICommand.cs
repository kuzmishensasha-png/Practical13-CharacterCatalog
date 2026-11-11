namespace Practical1.CLI;

public interface ICommand
{
    void Execute(CommandRequest request);
}
