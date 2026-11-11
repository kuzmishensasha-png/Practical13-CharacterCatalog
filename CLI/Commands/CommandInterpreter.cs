namespace Practical1.CLI;

public class CommandInterpreter
{
    // Command: команди зберігаються тут, щоб викликати їх за назвою.
    private readonly Dictionary<string, ICommand> commands =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly ICommandParser parser;
    private readonly IPresenter presenter;

    private bool running = true;

    public CommandInterpreter(
        ICommandParser parser,
        IPresenter presenter)
    {
        this.parser = parser;
        this.presenter = presenter;
    }

    public void Register(string name, Action<CommandRequest> action)
    {
        Register(name, new ActionCommand(action));
    }

    public void Register(string name, ICommand command)
    {
        commands[name] = command;
    }

    public void UseStrategy(ICommandStrategy strategy)
    {
        strategy.RegisterCommands(this);
    }

    public void ExecCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }

        CommandRequest request = parser.Parse(input);

        if (commands.TryGetValue(request.Name, out ICommand? command))
        {
            command.Execute(request);
        }
        else
        {
            presenter.ShowLine("Unknown command. Type help.");
        }
    }

    public void Stop()
    {
        running = false;
    }

    public void Run()
    {
        while (running)
        {
            presenter.Show("> ");

            string? input = presenter.Read();

            if (input == null)
            {
                break;
            }

            ExecCommand(input);
        }
    }
}
