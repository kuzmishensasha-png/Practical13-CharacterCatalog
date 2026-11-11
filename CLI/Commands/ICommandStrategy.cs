namespace Practical1.CLI;

public interface ICommandStrategy
{
    void RegisterCommands(CommandInterpreter interpreter);
}
