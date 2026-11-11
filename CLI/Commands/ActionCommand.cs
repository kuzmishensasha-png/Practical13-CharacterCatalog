namespace Practical1.CLI;

public sealed class ActionCommand : ICommand
{
    private readonly Action<CommandRequest> action;

    public ActionCommand(Action<CommandRequest> action)
    {
        this.action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public void Execute(CommandRequest request) => action(request);
}
