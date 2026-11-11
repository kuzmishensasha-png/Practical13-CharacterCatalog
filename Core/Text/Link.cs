namespace Practical1.TextSystem;

public class Link : TextElement
{
    public override string Name => Text;

    public string Text { get; set; }

    public string Url { get; set; }

    public override TextElement DeepClone() => new Link(Text, Url);

    public Link(
        string text,
        string url)
    {
        Text = text;
        Url = url;
    }
}