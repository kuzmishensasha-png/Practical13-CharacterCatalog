using Practical1.TextSystem;
using Practical1.Infra.Data;

namespace Practical1.CLI;

public class TextCommandService
{
    private readonly DocumentSession session = DocumentSession.Instance;
    private Text document => session.Document;

    private readonly IDataStorage storage;
    private readonly IPresenter presenter;
    private readonly TextRenderer renderer;

    private ContainerElement current;

    public TextCommandService(
        IPresenter presenter,
        IDataStorage storage,
        TextRenderer renderer)
    {
        this.presenter = presenter;
        this.storage = storage;
        this.renderer = renderer;

        current = document;
    }

    public void Pwd(CommandRequest request)
    {
        presenter.ShowLine(
            GetPath(current)
        );
    }

    public void Print(CommandRequest request)
    {
        bool whole =
            request.Options.ContainsKey("whole");

        bool showIds =
            request.Options.ContainsKey("id");

        TextElement element =
            whole ? document : current;

        presenter.ShowLine(
            renderer.Render(element)
        );

        if (showIds)
        {
            presenter.ShowLine("");
            presenter.ShowLine("IDs:");

            PrintIds(
                element,
                0
            );
        }
    }

    public void Add(CommandRequest request)
    {
        if (request.Arguments.Count < 2)
        {
            presenter.ShowLine(
                "Usage: add <container|leaf> <type>"
            );

            return;
        }

        string category =
            request.Arguments[0].ToLower();

        string type =
            request.Arguments[1].ToLower();

        TextElement? element = null;

        if (category == "container" &&
            type == "heading")
        {
            string title =
                ReadRequired("Title: ");

            presenter.Show("Level: ");

            int level;

            while (!int.TryParse(
                presenter.Read(),
                out level) || level < 1)
            {
                presenter.Show(
                    "Enter a valid level: "
                );
            }

            element =
                new Heading(title, level);
        }
        else if (category == "leaf" &&
                 type == "paragraph")
        {
            string content =
                ReadRequired("Content: ");

            element =
                new Paragraph(content);
        }
        else if (category == "leaf" &&
                 type == "link")
        {
            string text =
                ReadRequired("Text: ");

            string url =
                ReadRequired("URL: ");

            element =
                new Link(text, url);
        }
        else if (category == "leaf" &&
                 type == "image")
        {
            string source =
                ReadRequired("Source: ");

            string description =
                ReadRequired("Description: ");

            element =
                new ImageElement(
                    source,
                    description
                );
        }
        else
        {
            presenter.ShowLine(
                "Unknown element type."
            );

            return;
        }

        current.Add(element);

        presenter.ShowLine(
            $"Added {type}. ID: {element.Id}"
        );
    }

    public void Remove(CommandRequest request)
    {
        if (request.Arguments.Count > 0)
        {
            string name =
                string.Join(
                    " ",
                    request.Arguments
                );

            TextElement? element =
                current.FindChild(name);

            if (element == null)
            {
                presenter.ShowLine(
                    "Element not found."
                );

                return;
            }

            if (!Confirm(
                $"Remove '{element.Name}'?"))
            {
                return;
            }

            current.Remove(element);

            presenter.ShowLine(
                "Element removed."
            );

            return;
        }

        if (ReferenceEquals(
            current,
            document))
        {
            presenter.ShowLine(
                "The root document cannot be removed."
            );

            return;
        }

        ContainerElement? parent =
            current.Parent;

        if (parent == null)
        {
            return;
        }

        if (!Confirm(
            $"Remove current container '{current.Name}'?"))
        {
            return;
        }

        parent.Remove(current);

        current = parent;

        presenter.ShowLine(
            "Container removed."
        );
    }

    public void Up(CommandRequest request)
    {
        if (current.Parent == null)
        {
            presenter.ShowLine(
                "Already at root."
            );

            return;
        }

        current = current.Parent;

        presenter.ShowLine(
            GetPath(current)
        );
    }

    public void Cd(CommandRequest request)
    {
        if (request.Options.TryGetValue(
            "id",
            out string? id)
            && !string.IsNullOrWhiteSpace(id))
        {
            TextElement? element =
                FindById(document, id);

            if (element is not
                ContainerElement container)
            {
                presenter.ShowLine(
                    "Container not found."
                );

                return;
            }

            current = container;

            presenter.ShowLine(
                GetPath(current)
            );

            return;
        }

        if (request.Arguments.Count == 0)
        {
            presenter.ShowLine(
                "Usage: cd <path> or cd --id <container_id>"
            );

            return;
        }

        string path =
            string.Join(
                " ",
                request.Arguments
            );

        ContainerElement? target =
            ResolvePath(path);

        if (target == null)
        {
            presenter.ShowLine(
                "Path not found."
            );

            return;
        }

        current = target;

        presenter.ShowLine(
            GetPath(current)
        );
    }

    private string GetPath(
        ContainerElement element)
    {
        if (ReferenceEquals(
            element,
            document))
        {
            return "/";
        }

        List<string> parts = new();

        ContainerElement? currentElement =
            element;

        while (currentElement != null &&
               !ReferenceEquals(
                   currentElement,
                   document))
        {
            parts.Add(
                currentElement.Name
            );

            currentElement =
                currentElement.Parent;
        }

        parts.Reverse();

        return "/" +
               string.Join("/", parts);
    }

    private ContainerElement? ResolvePath(
        string path)
    {
        if (path == "/")
        {
            return document;
        }

        ContainerElement element =
            path.StartsWith("/")
                ? document
                : current;

        string[] parts =
            path.Split(
                '/',
                StringSplitOptions
                    .RemoveEmptyEntries
            );

        foreach (string part in parts)
        {
            if (part == "..")
            {
                if (element.Parent != null)
                {
                    element =
                        element.Parent;
                }

                continue;
            }

            TextElement? child =
                element.FindChild(part);

            if (child is not
                ContainerElement container)
            {
                return null;
            }

            element = container;
        }

        return element;
    }

    private TextElement? FindById(
        TextElement element,
        string id)
    {
        if (string.Equals(
            element.Id,
            id,
            StringComparison.OrdinalIgnoreCase))
        {
            return element;
        }

        if (element is
            ContainerElement container)
        {
            foreach (TextElement child
                     in container.Children)
            {
                TextElement? found =
                    FindById(
                        child,
                        id
                    );

                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }

    private void PrintIds(
        TextElement element,
        int level)
    {
        string indentation =
            new string(
                ' ',
                level * 2
            );

        string name =
            element is Text
                ? "/"
                : element.Name;

        presenter.ShowLine(
            $"{indentation}[{element.Id}] {name}"
        );

        if (element is
            ContainerElement container)
        {
            foreach (TextElement child
                     in container.Children)
            {
                PrintIds(
                    child,
                    level + 1
                );
            }
        }
    }

    private string ReadRequired(
        string message)
    {
        while (true)
        {
            presenter.Show(message);

            string? value =
                presenter.Read();

            if (!string.IsNullOrWhiteSpace(
                value))
            {
                return value;
            }
        }
    }

    private bool Confirm(
        string message)
    {
        presenter.Show(
            $"{message} (y/n): "
        );

        string? answer =
            presenter.Read();

        return string.Equals(
            answer,
            "y",
            StringComparison.OrdinalIgnoreCase
        );
    }

    public void Duplicate(CommandRequest request)
    {
        if (request.Arguments.Count == 0)
        {
            presenter.ShowLine("Usage: clone <element_name>");
            return;
        }

        string name = string.Join(" ", request.Arguments);
        TextElement? source = current.FindChild(name);
        if (source == null)
        {
            presenter.ShowLine("Element not found.");
            return;
        }

        TextElement copy = source.DeepClone();
        current.Add(copy);
        presenter.ShowLine($"Cloned '{source.Name}'. New ID: {copy.Id}");
    }

    public void Example(CommandRequest request)
    {
        if (!Confirm("Replace current document with an example?"))
            return;

        Text created = new TextBuilder()
            .BeginSection("Introduction", 1)
                .AddParagraph("Tree-like text document.")
                .BeginSection("Details", 2)
                    .AddParagraph("This paragraph belongs to a nested section.")
                    .AddLink("Example", "https://example.org")
                .EndSection()
            .EndSection()
            .Build();

        session.Replace(created);
        current = document;
        presenter.ShowLine("Example document created. Use 'print --whole'.");
    }

    public void Save(CommandRequest request)
    {
        string fileName = "text.json";

        if (request.Arguments.Count > 0)
        {
            fileName = request.Arguments[0];
        }

        TextElementData data =
            CreateData(document);

        storage.Save(fileName, data);

        presenter.ShowLine(
            $"Document saved to {fileName}"
        );
    }

    public void Load(CommandRequest request)
    {
        string fileName = "text.json";

        if (request.Arguments.Count > 0)
        {
            fileName = request.Arguments[0];
        }

        TextElementData? data =
            storage.Load<TextElementData>(
                fileName
            );

        if (data == null)
        {
            presenter.ShowLine(
                "Save file not found."
            );

            return;
        }

        Text loaded = new Text();

        foreach (TextElementData child in data.Children)
        {
            loaded.Add(CreateElement(child));
        }

        session.Replace(loaded);
        current = document;

        presenter.ShowLine(
            $"Document loaded from {fileName}"
        );
    }   

    private TextElementData CreateData(
        TextElement element)
    {
        TextElementData data =
            new TextElementData();

    data.Name = element.Name;

        if (element is Text)
        {
        data.Type = "text";
        }
        else if (element is Heading heading)
        {
            data.Type = "heading";
            data.Content = heading.Content;
            data.Level = heading.Level;
        }
        else if (element is Paragraph paragraph)
        {
            data.Type = "paragraph";
            data.Content = paragraph.Content;
        }
        else if (element is Link link)
        {
            data.Type = "link";
            data.Content = link.Text;
            data.Url = link.Url;
        }
        else if (element is ImageElement image)
        {
            data.Type = "image";
            data.Source = image.Source;
            data.Content = image.Description;
        }

        if (element is ContainerElement container)
        {
            foreach (TextElement child
                     in container.Children)
            {
                data.Children.Add(
                    CreateData(child)
                );
            }
        }

        return data;
    }

    private TextElement CreateElement(
        TextElementData data)
    {
        TextElement element;

        if (data.Type == "heading")
        {
            element = new Heading(
                data.Content ?? "",
                data.Level
           );
       }
       else if (data.Type == "paragraph")
       {
            element = new Paragraph(
                data.Content ?? ""
            );
        }
        else if (data.Type == "link")
        {
            element = new Link(
                data.Content ?? "",
                data.Url ?? ""
            );
        }
        else if (data.Type == "image")
        {
            element = new ImageElement(
                data.Source ?? "",
                data.Content ?? ""
            );
        }
        else
        {
            throw new InvalidOperationException(
                "Unknown text element type."
            );
        }

        if (element is ContainerElement container)
        {
            foreach (TextElementData child
                     in data.Children)
            {
                container.Add(
                    CreateElement(child)
                );
            }
        }

        return element;
    }
}