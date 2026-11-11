using Practical1.CLI;

namespace Practical1.CLI.Catalog;

public class CatalogCommandStrategy : ICommandStrategy
{
    private readonly CharacterCatalog catalog;
    private readonly ICatalogRenderer renderer;
    private readonly IPresenter presenter;
    private string currentFile = "catalog.json";

    public CatalogCommandStrategy(
        CharacterCatalog catalog,
        ICatalogRenderer renderer,
        IPresenter presenter)
    {
        this.catalog = catalog;
        this.renderer = renderer;
        this.presenter = presenter;

        try
        {
            if (catalog.Load(currentFile))
            {
                presenter.ShowLine($"Catalog loaded from {currentFile}.");
            }
        }
        catch (Exception error) when (error is IOException
            or UnauthorizedAccessException or System.Text.Json.JsonException)
        {
            presenter.ShowLine($"Cannot load saved catalog: {error.Message}");
        }
    }

    public void RegisterCommands(CommandInterpreter interpreter)
    {
        interpreter.Register("help", request => Help());
        interpreter.Register("update", request => Update());
        interpreter.Register("ls", request => List());
        interpreter.Register("show", Show);
        interpreter.Register("create", request => Create());
        interpreter.Register("save", Save);
        interpreter.Register("load", Load);
    }

    private void Help()
    {
        presenter.ShowLine("Catalog commands:");
        presenter.ShowLine("update               - update list from Genshin API");
        presenter.ShowLine("ls                   - list all characters");
        presenter.ShowLine("show <id/name>       - show character details and image link");
        presenter.ShowLine("create               - create a character in a dialog");
        presenter.ShowLine("save [file]          - save catalog as JSON");
        presenter.ShowLine("load [file]          - load catalog from JSON");
        presenter.ShowLine("exit                 - close application");
        presenter.ShowLine("Put names with spaces in quotes: show \"My Hero\"");
    }

    private void Update()
    {
        try
        {
            int remoteCount = catalog.UpdateAsync().GetAwaiter().GetResult();
            catalog.Save(currentFile);
            presenter.ShowLine($"Updated: {remoteCount} API characters. Local characters kept.");
        }
        catch (Exception error) when (error is HttpRequestException
            or TaskCanceledException or IOException or UnauthorizedAccessException)
        {
            presenter.ShowLine($"Cannot update catalog: {error.Message}");
            presenter.ShowLine("You can still use a previously saved catalog.");
        }
    }

    private void List()
    {
        presenter.ShowLine(renderer.RenderList(catalog.Characters));
    }

    private void Show(CommandRequest request)
    {
        if (request.Arguments.Count == 0)
        {
            presenter.ShowLine("Usage: show <id/name>");
            return;
        }

        string name = string.Join(" ", request.Arguments);

        try
        {
            CatalogCharacter? character =
                catalog.FindAsync(name).GetAwaiter().GetResult();

            if (character == null)
            {
                presenter.ShowLine("Character not found. Try update or ls.");
                return;
            }

            presenter.ShowLine(renderer.RenderDetails(character));
        }
        catch (Exception error) when (error is HttpRequestException or TaskCanceledException)
        {
            CatalogCharacter? cached = catalog.Characters.FirstOrDefault(character =>
                string.Equals(character.Id, name, StringComparison.OrdinalIgnoreCase)
                || string.Equals(character.Name, name, StringComparison.OrdinalIgnoreCase));

            presenter.ShowLine($"API unavailable: {error.Message}");
            if (cached != null)
            {
                presenter.ShowLine(renderer.RenderDetails(cached));
            }
        }
    }

    private void Create()
    {
        try
        {
            presenter.ShowLine("Create a character:");
            string name = ReadRequired("Name: ");
            string description = ReadRequired("Description: ");
            string element = ReadRequired("Element: ");
            string weapon = ReadRequired("Weapon: ");
            int rarity = ReadRarity();
            string imageUrl = ReadImageUrl();

            CatalogCharacter character = catalog.CreateCustom(
                name, description, element, weapon, rarity, imageUrl);

            catalog.Save(currentFile);
            presenter.ShowLine($"Created {character.Name} (id: {character.Id}).");
            presenter.ShowLine($"Automatically saved to {currentFile}.");
        }
        catch (OperationCanceledException)
        {
            presenter.ShowLine("Creation cancelled.");
        }
        catch (ArgumentException error)
        {
            presenter.ShowLine(error.Message);
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            presenter.ShowLine($"Character added, but saving failed: {error.Message}");
            presenter.ShowLine("Use save to retry.");
        }
    }

    private void Save(CommandRequest request)
    {
        string file = request.Arguments.FirstOrDefault() ?? currentFile;
        try
        {
            catalog.Save(file);
            currentFile = file;
            presenter.ShowLine($"Catalog saved to {file}.");
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            presenter.ShowLine($"Cannot save catalog: {error.Message}");
        }
    }

    private void Load(CommandRequest request)
    {
        string file = request.Arguments.FirstOrDefault() ?? currentFile;
        try
        {
            if (catalog.Load(file))
            {
                currentFile = file;
                presenter.ShowLine($"Catalog loaded from {file}.");
            }
            else
            {
                presenter.ShowLine("File not found.");
            }
        }
        catch (Exception error) when (error is IOException
            or UnauthorizedAccessException or System.Text.Json.JsonException)
        {
            presenter.ShowLine($"Cannot load catalog: {error.Message}");
        }
    }

    private string ReadRequired(string label)
    {
        while (true)
        {
            presenter.Show(label);
            string? value = presenter.Read();

            if (value == null || value.Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                throw new OperationCanceledException();
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            presenter.ShowLine("This field cannot be empty (or type cancel).");
        }
    }

    private int ReadRarity()
    {
        while (true)
        {
            string value = ReadRequired("Rarity (1-5): ");
            if (int.TryParse(value, out int rarity) && rarity >= 1 && rarity <= 5)
            {
                return rarity;
            }

            presenter.ShowLine("Enter a number from 1 to 5.");
        }
    }

    private string ReadImageUrl()
    {
        while (true)
        {
            presenter.Show("Image URL (optional): ");
            string? value = presenter.Read();

            if (value == null || value.Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                throw new OperationCanceledException();
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            if (Uri.TryCreate(value.Trim(), UriKind.Absolute, out Uri? url)
                && (url.Scheme == Uri.UriSchemeHttps || url.Scheme == Uri.UriSchemeHttp))
            {
                return url.ToString();
            }

            presenter.ShowLine("Enter a full http(s) URL or leave blank.");
        }
    }
}
