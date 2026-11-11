namespace Practical1.TextSystem;

public class TextBuilder
{
    private readonly Text text = new Text();
    private readonly Stack<Heading> openSections = new();

    private ContainerElement Current =>
        openSections.Count == 0 ? text : openSections.Peek();

    public Heading CreateHeading(string content, int level)
    {
        Heading heading = new Heading(content, level);

        text.Add(heading);

        return heading;
    }

    public Paragraph CreateParagraph(string content)
    {
        Paragraph paragraph = new Paragraph(content);

        text.Add(paragraph);

        return paragraph;
    }

    public Link CreateLink(string linkText, string url)
    {
        Link link = new Link(linkText, url);

        text.Add(link);

        return link;
    }

    public ImageElement CreateImage(string source, string description)
    {
        ImageElement image = new ImageElement(source, description);

        text.Add(image);

        return image;
    }

    public TextBuilder BeginSection(string title, int level)
    {
        if (level < 1)
            throw new ArgumentOutOfRangeException(nameof(level), "Heading level must be positive.");

        Heading heading = new Heading(title, level);
        Current.Add(heading);
        openSections.Push(heading);
        return this;
    }

    public TextBuilder EndSection()
    {
        if (openSections.Count == 0)
            throw new InvalidOperationException("There is no open section.");

        openSections.Pop();
        return this;
    }

    public TextBuilder AddParagraph(string content)
    {
        Current.Add(new Paragraph(content));
        return this;
    }

    public TextBuilder AddLink(string label, string url)
    {
        Current.Add(new Link(label, url));
        return this;
    }

    public TextBuilder AddImage(string source, string description)
    {
        Current.Add(new ImageElement(source, description));
        return this;
    }

    public Text Build()
    {
        if (openSections.Count > 0)
            throw new InvalidOperationException("Close all sections before building the document.");

        return text;
    }
}