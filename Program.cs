using Practical1.CLI;
using Practical1.GameSystem;
using Practical1.Infra.Data;
using Practical1.Infra.Display;
using Practical1.Infra.Remote;
using Practical1.CLI.Catalog;

string? mode = null;
HttpClient? catalogHttpClient = null;

IDisplay display =
    new ConsoleDisplay();

IPresenter presenter =
    new ConsolePresenter(display);

ICommandParser parser =
    new CommandParser();

IDataStorage storage =
    new JsonDataStorage();

ILogger logger =
    new DisplayLogger(display);

Game game =
    new Game(logger);

TextRenderer textRenderer =
    new TextRenderer();

if (args.Contains("--catalog"))
{
    mode = "catalog";
}
else if (args.Contains("--text"))
{
    mode = "text";
}
else if (args.Contains("--chars"))
{
    mode = "chars";
}

if (mode == null)
{
    presenter.ShowLine("Select mode:");
    presenter.ShowLine("1 - Text");
    presenter.ShowLine("2 - Characters");
    presenter.ShowLine("3 - Character catalog");
    presenter.Show("> ");

    string? choice =
        presenter.Read();

    if (choice == "1")
    {
        mode = "text";
    }
    else if (choice == "2")
    {
        mode = "chars";
    }
    else if (choice == "3")
    {
        mode = "catalog";
    }
    else
    {
        presenter.ShowLine(
            "Invalid mode."
        );

        return;
    }
}

presenter.ShowLine(
    $"Mode: {mode}"
);

CommandInterpreter interpreter =
    new CommandInterpreter(
        parser,
        presenter
    );

interpreter.Register(
    "exit",
    request => interpreter.Stop()
);

if (mode == "catalog")
{
    catalogHttpClient = new HttpClient
    {
        BaseAddress = new Uri("https://genshin.jmp.blue/"),
        Timeout = TimeSpan.FromSeconds(15)
    };

    IGenshinApiClient catalogApi = new GenshinApiClient(catalogHttpClient);
    CharacterCatalog catalog = new CharacterCatalog(catalogApi, storage);
    ICatalogRenderer catalogRenderer = new TextCatalogRenderer();

    interpreter.UseStrategy(
        new CatalogCommandStrategy(catalog, catalogRenderer, presenter));
}
else if (mode == "text")
{
    TextCommandService textCommands =
        new TextCommandService(
            presenter,
            storage,
            textRenderer
        );

    interpreter.Register(
        "pwd",
        textCommands.Pwd
    );

    interpreter.Register(
        "print",
        textCommands.Print
    );

    interpreter.Register(
        "add",
        textCommands.Add
    );

    interpreter.Register(
        "rm",
        textCommands.Remove
    );

    interpreter.Register(
        "up",
        textCommands.Up
    );

    interpreter.Register(
        "cd",
        textCommands.Cd
    );

    interpreter.Register(
        "clone",
        textCommands.Duplicate
    );

    interpreter.Register(
        "example",
        textCommands.Example
    );

    interpreter.Register(
        "save",
        textCommands.Save
    );

    interpreter.Register(
        "load",
        textCommands.Load
    );

    interpreter.Register(
        "help",
        request =>
        {
            presenter.ShowLine(
                "Text commands:"
            );

            presenter.ShowLine(
                "pwd"
            );

            presenter.ShowLine(
                "print [--whole] [--id]"
            );

            presenter.ShowLine(
                "add <container|leaf> <type>"
            );

            presenter.ShowLine(
                "rm [element_name]"
            );

            presenter.ShowLine(
                "clone <element_name>"
            );

            presenter.ShowLine(
                "example"
            );

            presenter.ShowLine(
                "up"
            );

            presenter.ShowLine(
                "cd <path>"
            );

            presenter.ShowLine(
                "cd --id <container_id>"
            );

            presenter.ShowLine(
                "save [file]"
            );

            presenter.ShowLine(
                "load [file]"
            );

            presenter.ShowLine(
                "exit"
            );
        }
    );
}
else if (mode == "chars")
{
    CharacterCommandService charCommands =
        new CharacterCommandService(
            presenter,
            storage,
            game
        );

    HttpClient httpClient =
        new HttpClient();

    httpClient.BaseAddress =
        new Uri(
            "https://genshin.jmp.blue/"
        );

    IGenshinApiClient apiClient =
        new GenshinApiClient(
            httpClient
        );

    CharacterMapper characterMapper =
        new CharacterMapper(
            game
        );

    WeaponMapper weaponMapper =
        new WeaponMapper();

    RemoteCommandService remoteCommands =
        new RemoteCommandService(
            apiClient,
            presenter
        );

    RemoteCharacterService remoteCharacters =
        new RemoteCharacterService(
            apiClient,
            characterMapper,
            weaponMapper,
            game
        );

    CharacterTextBuilder characterTextBuilder =
        new CharacterTextBuilder();

    interpreter.Register(
        "create",
        charCommands.Create
    );

    interpreter.Register(
        "add",
        charCommands.Add
    );

    interpreter.Register(
        "act",
        charCommands.Act
    );

    interpreter.Register(
        "ls",
        charCommands.List
    );

    interpreter.Register(
        "save",
        charCommands.Save
    );

    interpreter.Register(
        "load",
        charCommands.Load
    );

    interpreter.Register(
        "snapshot",
        charCommands.Snapshot
    );

    interpreter.Register(
        "restore",
        charCommands.Restore
    );

    interpreter.Register(
        "strategy",
        charCommands.Strategy
    );

    interpreter.Register(
        "remote-chars",
        request =>
        {
            remoteCommands
                .ListCharactersAsync()
                .GetAwaiter()
                .GetResult();
        }
    );

    interpreter.Register(
        "remote-weapons",
        request =>
        {
            remoteCommands
                .ListWeaponsAsync()
                .GetAwaiter()
                .GetResult();
        }
    );

    interpreter.Register(
        "remote-create",
        request =>
        {
            if (request.Arguments.Count == 0)
            {
                presenter.ShowLine(
                    "Usage: remote-create <character>"
                );

                return;
            }

            Character? character =
                remoteCharacters
                    .CreateCharacterAsync(
                        request.Arguments[0]
                    )
                    .GetAwaiter()
                    .GetResult();

            if (character == null)
            {
                presenter.ShowLine(
                    "Character not found."
                );

                return;
            }

            presenter.ShowLine(
                $"Character created: {character.Name}"
            );
        }
    );

    interpreter.Register(
        "remote-equip",
        request =>
        {
            if (request.Arguments.Count == 0)
            {
                presenter.ShowLine(
                    "Usage: remote-equip <weapon>"
                );

                return;
            }

            Item? item =
                remoteCharacters
                    .EquipWeaponAsync(
                        request.Arguments[0]
                    )
                    .GetAwaiter()
                    .GetResult();

            if (item == null)
            {
                presenter.ShowLine(
                    "Weapon could not be equipped."
                );

                return;
            }

            presenter.ShowLine(
                $"Weapon equipped: {item.Name}"
            );
        }
    );

    interpreter.Register(
        "remote-show",
        request =>
        {
            Character? character =
                remoteCharacters.CurrentCharacter;

            if (character == null)
            {
                presenter.ShowLine(
                    "Create a character first."
                );

                return;
            }

            string text =
                textRenderer.Render(
                    characterTextBuilder.Build(
                        character
                    )
                );

            presenter.ShowLine(
                text
            );
        }
    );

    interpreter.Register(
        "help",
        request =>
        {
            presenter.ShowLine(
                "Character commands:"
            );

            presenter.ShowLine(
                "create <char|item|ability>"
            );

            presenter.ShowLine(
                "add <item|ability> --char_id <id/name> --id <id/name>"
            );

            presenter.ShowLine(
                "act <attack|heal|ability> <actor> <target> [--id <ability>]"
            );

            presenter.ShowLine(
                "ls <char|item|ability> [--id <id/name>]"
            );

            presenter.ShowLine(
                "save [file]"
            );

            presenter.ShowLine(
                "load [file]"
            );

            presenter.ShowLine(
                "snapshot <character>"
            );

            presenter.ShowLine(
                "restore <character>"
            );

            presenter.ShowLine(
                "strategy <character> <normal|piercing>"
            );

            presenter.ShowLine(
                "remote-chars"
            );

            presenter.ShowLine(
                "remote-weapons"
            );

            presenter.ShowLine(
                "remote-create <character>"
            );

            presenter.ShowLine(
                "remote-equip <weapon>"
            );

            presenter.ShowLine(
                "remote-show"
            );

            presenter.ShowLine(
                "exit"
            );
        }
    );
}

interpreter.Run();
catalogHttpClient?.Dispose();
